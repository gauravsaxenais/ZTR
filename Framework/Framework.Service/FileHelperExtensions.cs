namespace ZTR.Framework.Service
{
    using System;
    using System.IO;
    using System.Linq;
    using ZTR.Framework.Business;
    using Microsoft.AspNetCore.Http;

    public static class FileHelperExtensions
    {
        public static ErrorRecords<TEnum> ValidateFile<TEnum>(this IFormFile file, TEnum emptyFileErrorCode, TEnum invalidFileExtension, params string[] validExtensions)
            where TEnum : struct, Enum
        {
            ErrorRecords<TEnum> errorRecords = new ErrorRecords<TEnum>();

            if (file == null)
            {
                var errorRecord = new ErrorRecord<TEnum>(emptyFileErrorCode, "The file must not be empty.");
                errorRecords.Add(errorRecord);
            }
            else if (validExtensions.Any())
            {
                var extension = Path.GetExtension(file.FileName);
                if (!validExtensions.Contains(extension))
                {
                    var errorRecord = new ErrorRecord<TEnum>(invalidFileExtension, "The file must contain a valid extension.");
                    errorRecords.Add(errorRecord);
                }
            }

            return errorRecords;
        }
    }
}
