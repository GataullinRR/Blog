using Ganss.XSS;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Blog.Services
{
    public class PostSanitizerService
    {
        readonly IHtmlSanitizer _allowAllButNotExecutable;

        public PostSanitizerService()
        {
            _allowAllButNotExecutable = new HtmlSanitizer(
                allowedTags: "code i b s img li ul ol link p em strong tr td table tbody a br span code pre sup sub blockquote caption".Split(" "),
                allowedSchemes: "http https data".Split(" "),
                allowedAttributes: "href src style class".Split(" "),
                uriAttributes: "href src".Split(" "),
                allowedCssProperties: "list-style-type padding-left text-decoration height width border border-collapse cellspacing cellpadding data-mce-style".Split(" "),
                allowedCssClasses: "language-csharp language-markup language-javascript language-css language-php language-ruby language-python language-java language-c language-cpp".Split(" "));
        }

        public async Task<string> SanitizePostBodyAsync(string body)
        {
            await ThreadingUtils.ContinueAtThreadPull();

            var intermediate = _allowAllButNotExecutable.Sanitize(body);
            var document = new HtmlDocument();
            document.LoadHtml(intermediate);
            var nodes = document.DocumentNode.SelectNodes("//img");
            foreach (var node in nodes)
            {
                var ATTRIBUTE = "class";

                if (node.Attributes.NotContains(a => a.Name == ATTRIBUTE))
                {
                    node.Attributes.Add(ATTRIBUTE, "");
                }
                node.Attributes[ATTRIBUTE].Value += " rounded img-fluid";
            }
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            document.Save(writer);
            writer.Flush();
            ms.Position = 0;
            var sanitized = new StreamReader(ms).ReadAllText().Aggregate();
            
            return sanitized;
        }

        public string IgnoreNonTextNodes(string richHtml)
        {
            if (string.IsNullOrEmpty(richHtml)) return string.Empty;

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
