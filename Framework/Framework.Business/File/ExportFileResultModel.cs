namespace ZTR.Framework.Business.File
{
    public class ExportFileResultModel
    {
        public ExportFileResultModel()
        {
        }

        public ExportFileResultModel(string mimeType, byte[] data, string fileName)
        {
            MimeType = mimeType;
            FileName = fileName;
            Data = data;
        }

        /// <summary>
        /// Gets or sets the fileName.
        /// </summary>
        /// <value>
        /// The fileName.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the mimeType.
        /// </summary>
        /// <value>
        /// The mimeType.
        /// </value>
        public string MimeType { get; set; }
    }
}
