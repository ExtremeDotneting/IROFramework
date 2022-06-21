using System;

namespace IROFramework.Web.Dto.FilesStorageDto
{
    public class UploadLogRecord
    {
        public DateTime UploadedAt { get; set; }

        public string FileName { get; set; }

        public string DownloadUrl { get; set; }
    }
}
