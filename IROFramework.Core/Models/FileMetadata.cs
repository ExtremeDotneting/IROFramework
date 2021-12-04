using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IROFramework.Core.Models
{
    public class FileMetadata
    {
        public string ContentDisposition { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; }

        public long Length { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public bool CanBeOpenedInBrowser()
        {
            if (FileExtension == null)
                return false;

            var hashSet = new HashSet<string>()
            {
                ".html",
                ".htm",
                ".js",
                ".json",
                ".css",
                ".txt",
                ".png",
                ".jpg",
                ".gif"
            };
            return hashSet.Contains(FileExtension);
        }

        public static FileMetadata FromFormFile(IFormFile formFile, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = formFile.FileName;
            }

            var obj = new FileMetadata()
            {
                FileName = fileName,
                ContentType = formFile.ContentType,
                ContentDisposition = formFile.ContentDisposition,
                Headers = formFile.Headers,
                Length = formFile.Length,
                LastModified = DateTimeOffset.UtcNow,
                FileExtension=Path.GetExtension(fileName).ToLower()
            };
            return obj;
        }

    }
}
