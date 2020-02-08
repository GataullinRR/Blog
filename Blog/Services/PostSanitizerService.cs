using Blog.Misc;
using DBModels;
using Ganss.XSS;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class PostSanitizerService : ServiceBase
    {
        public const int MAX_IMAGE_SIZE = 3 * 1024 * 1024;

        readonly IHtmlSanitizer _allowAllButNotExecutable;

        public PostSanitizerService(ServiceLocator services) : base(services)
        {
            _allowAllButNotExecutable = new HtmlSanitizer(
                allowedTags: "h1 h2 h3 h4 h5 h6 code i b s img li ul ol link p em strong tr td table tbody a br span code pre sup sub blockquote caption".Split(" "),
                allowedSchemes: "http https data".Split(" "),
                allowedAttributes: "href src style class".Split(" "),
                uriAttributes: "href src".Split(" "),
                allowedCssProperties: "list-style-type padding-left text-decoration height width border border-collapse cellspacing cellpadding data-mce-style".Split(" "),
                allowedCssClasses: "language-csharp language-markup language-javascript language-css language-php language-ruby language-python language-java language-c language-cpp token operator punctuation keyword string number".Split(" "));
        }

        public async Task<string> SanitizePostBodyAsync(string body)
        {
            await ThreadingUtils.ContinueAtThreadPull();

            var intermediate = _allowAllButNotExecutable.Sanitize(body);
            var document = new HtmlDocument();
            document.LoadHtml(intermediate);
            var nodes = document.DocumentNode.SelectNodes("//img");
            foreach (var node in nodes.NullToEmpty())
            {
                var srcAttr = node.Attributes.Single(a => a.Name == "src");
                var src = srcAttr.Value;
                src = await trySanitizeImage(src);
                if (src == null)
                {
                    node.Remove();
                }
                else
                {
                    srcAttr.Value = src;

                    const string ATTRIBUTE = "class";
                    if (node.Attributes.NotContains(a => a.Name == ATTRIBUTE))
                    {
                        node.Attributes.Add(ATTRIBUTE, "");
                    }
                    node.Attributes[ATTRIBUTE].Value += " rounded img-fluid";
                }
            }
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            document.Save(writer);
            writer.Flush();
            ms.Position = 0;
            var sanitized = new StreamReader(ms)
                .ReadAllText()
                .Aggregate();
            
            return sanitized;
        }

        async Task<string> trySanitizeImage(string imageSrc)
        {
            var formats = new (string Prefix, string Extension, ImageFormat Format)[]
            {
                ("data:image/jpeg;base64,", "jpg", ImageFormat.Jpeg),
                ("data:image/png;base64,", "png", ImageFormat.Png),
                ("data:image/gif;base64,", "gif", ImageFormat.Gif),
            };
            foreach (var format in formats)
            {
                if (imageSrc.StartsWith(format.Prefix))
                {
                    var length = imageSrc.Length - format.Prefix.Length / 4 * 3;
                    if (length < MAX_IMAGE_SIZE)
                    {
                        try
                        {
                            var image = imageSrc
                                .Skip(format.Prefix.Length)
                                .Aggregate()
                                .FromBase64()
                                .ToMemoryStream();
                            var i = Image.FromStream(image);

                            if (format.Format.Equals(i.RawFormat))
                            {
                                image.Position = 0;
                                var localPath = await S.Storage.SavePostImageAsync(image, format.Extension);
                                
                                return S.UrlHelper.AbsoluteContent(localPath);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            return null;
        }

        public string IgnoreNonTextNodes(string richHtml)
        {
            if (string.IsNullOrEmpty(richHtml))
            {
                return string.Empty;
            }

            var document = new HtmlDocument();
            document.LoadHtml(richHtml);

            var acceptableTags = new String[] { "strong", "em", "u" };

            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
                {
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);

                }
            }

            return document.DocumentNode.InnerHtml;
        }
    }
}
