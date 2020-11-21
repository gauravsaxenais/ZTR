namespace ZTR.Framework.Business.File
{
    using System.Collections.Generic;
    using EnsureThat;

    public class ParsedFile<TModel>
    {
        public ParsedFile(string filename, IEnumerable<TModel> models, string folderPath)
        {
            EnsureArg.IsNotNullOrWhiteSpace(filename, nameof(filename));
            EnsureArg.IsNotNull(models, nameof(models));
            EnsureArg.IsNotNullOrWhiteSpace(folderPath, nameof(folderPath));
            Filename = filename;
            Models = models;
            FolderPath = folderPath;
        }

        public string Filename { get; }

        public IEnumerable<TModel> Models { get; }

        public string FolderPath { get; }
    }
}
