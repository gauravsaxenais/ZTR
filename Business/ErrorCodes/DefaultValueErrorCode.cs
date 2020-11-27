namespace Business.ErrorCodes
{
    /// <summary>
    /// Error codes.
    /// </summary>
    public enum DefaultValueErrorCode
    {
        /// <summary>
        /// Unknown Error
        /// </summary>
        UnknownError = 0,

        /// <summary>
        /// The adobe traffic file upload error
        /// </summary>
        AdobeTrafficFileUploadError,

        /// <summary>
        /// The Reputation file delete error
        /// </summary>
        AdobeTrafficFileDeleteError,

        /// <summary>
        /// The Reputation file data import error
        /// </summary>
        AdobeTrafficFileDataImportError,

        /// <summary>
        /// The adobe make file process error
        /// </summary>
        AdobeMakeFileProcessError,

        /// <summary>
        /// The adobe provider file process error
        /// </summary>
        AdobeProviderFileProcessError,

        /// <summary>
        /// The adobe file is in import process
        /// </summary>
        AdobeFileIsInImportProcess,

        /// <summary>
        /// The adobe traffic job failed at the base level of the import process.
        /// </summary>
        AdobeTrafficJobProcessError,

        /// <summary>
        /// The adobe traffic job failed to load the dependant data.
        /// </summary>
        AdobeDependantLoadingProcessError,

        /// <summary>
        /// The adobe traffic job failed to delete failed traffic import records.
        /// </summary>
        AdobeTrafficDeletionProcessError
    }
}
