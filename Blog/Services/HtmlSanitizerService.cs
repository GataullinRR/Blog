using Ganss.XSS;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class HtmlSanitizerService
    {
        public IHtmlSanitizer AllowAllButNotExecutable { get; }

        public HtmlSanitizerService()
        {
            AllowAllButNotExecutable = new HtmlSanitizer(
                allowedTags: "code i b s img li ul ol link p em strong tr td table tbody a br span code pre sup sub blockquote caption".Split(" "),
                allowedSchemes: "http https data".Split(" "),
                allowedAttributes: "href src style class".Split(" "),
                uriAttributes: "href src".Split(" "),
                allowedCssProperties: "list-style-type padding-left text-decoration height width border border-collapse cellspacing cellpadding data-mce-style".Split(" "),
                allowedCssClasses: "language-csharp language-markup language-javascript language-css language-php language-ruby language-python language-java language-c language-cpp".Split(" "));
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
