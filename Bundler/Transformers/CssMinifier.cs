﻿using Microsoft.AspNetCore.Http;
using NUglify;
using NUglify.Css;

namespace Bundler.Transformers
{
    /// <summary>
    /// A CSS minifier.
    /// </summary>
    public class CssMinifier : BaseTransform
    {
        private CssSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CssMinifier"/> class.
        /// </summary>
        public CssMinifier(string path)
            : base(path)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CssMinifier"/> class.
        /// </summary>
        public CssMinifier(string path, CssSettings settings)
            : base(path)
        {
            _settings = settings;
        }

        /// <summary>
        /// Gets the content type produced by the transform.
        /// </summary>
        public override string ContentType => "text/css";

        /// <summary>
        /// Transforms the specified source.
        /// </summary>
        public override string Transform(HttpContext context, string source)
        {
            var settings = _settings ?? new CssSettings();
            var minified = Uglify.Css(source, settings);

            if (minified.HasErrors)
                return null;

            return minified.Code;
        }
    }
}
