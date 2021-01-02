namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents an output file from a generator.
    /// </summary>
    public class OutputFile
    {
        /// <summary>
        /// The filepath relative to the root of the generation output folder.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// The content of the file.
        /// </summary>
        public Stream Content { get; }

        /// <summary>
        /// Creates a new <see cref="OutputFile" />
        /// </summary>
        /// <param name="relativePath">The relative path to the file from the root of the output directory.</param>
        /// <param name="content">The content to put in the file.</param>
        public OutputFile(string relativePath, Stream content)
        {
            this.RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
            this.Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
