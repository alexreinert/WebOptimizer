﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Bundler.Taghelpers
{
    /// <summary>
    /// A TagHelper for hooking JavaScript bundles up to the HTML page.
    /// </summary>
    [HtmlTargetElement("script", Attributes = "asp-bundle")]
    public class ScriptTagHelper : BaseTagHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptTagHelper"/> class.
        /// </summary>
        public ScriptTagHelper(IHostingEnvironment env)
            : base(env)
        { }

        /// <summary>
        /// The route to the bundle file name.
        /// </summary>
        [HtmlAttributeName("asp-bundle")]
        public string Bundle { get; set; }

        /// <summary>
        /// Synchronously executes the TagHelper
        /// </summary>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Bundle))
            {
                if (Extensions.Options.Enabled)
                {
                    var transform = Extensions.Options.Transforms.FirstOrDefault(t => t.Path.Equals(Bundle));
                    string href = $"{Bundle}?v={GenerateHash(transform)}";
                    output.Attributes.SetAttribute("src", href);
                }
                else
                {
                    WriteIndividualTags(output);
                }
            }

            base.Process(context, output);
        }

        private void WriteIndividualTags(TagHelperOutput output)
        {
            var transform = Extensions.Options.Transforms.FirstOrDefault(t => t.Path.Equals(Bundle));
            output.SuppressOutput();

            var attrs = new List<string>();

            foreach (var item in output.Attributes)
            {
                string attr = item.Name;

                if (item.ValueStyle != HtmlAttributeValueStyle.Minimized)
                {
                    var quote = GetQuote(item.ValueStyle);
                    attr += "=" + quote + item.Value + quote;
                }

                attrs.Add(attr);
            }

            foreach (string file in transform.SourceFiles)
            {
                string src = $"{file}?v={GenerateHash(file)}";
                output.PostElement.AppendHtml($"<script src=\"{src}\" {string.Join(" ", attrs)}></script>");
            }
        }
    }
}
