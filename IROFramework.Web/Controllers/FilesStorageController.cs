using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IRO.Storage;
using IROFramework.Core.AppEnvironment;
using IROFramework.Web.Dto.FilesStorageDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Telegram.Bot.CloudStorage;

namespace IROFramework.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route(CommonConsts.ApiPath + "/files")]
    public class FilesStorageController : ControllerBase
    {
        const string UploadsLogCacheKey = "UploadsLogCacheKey";
        protected string LinkTemplate;

        readonly IKeyValueStorage _storage;
        readonly TelegramFilesCloud<FileMetadata> _filesCloud;

        public FilesStorageController(IKeyValueStorage storage, TelegramFilesCloud<FileMetadata> filesCloud)
        {
            _storage = storage;
            _filesCloud = filesCloud;
            LinkTemplate = $"{Env.ExternalUrl}/{CommonConsts.ApiPath}/files/";
        }

        [HttpGet("logs")]
        public async Task<IEnumerable<UploadLogRecord>> GetUploadLogs()
        {
            return await GetLogRecordsList();
        }

        /// <summary>
        /// Delete files or folders. Return deleted files metadata.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpDelete()]
        public async Task<IList<FileMetadata>> Delete([FromQuery] string path)
        {
            if (!path.StartsWith("root"))
            {
                throw new Exception("Path must start with 'root'");
            }

            path = path.Substring("root".Length);
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            var metadataList = await SearchFiles(path);
            foreach (var metadata in metadataList)
            {
                await _filesCloud.DeleteFile(metadata.FileName);
            }
            return metadataList;
        }

        /// <summary>
        /// Upload files
        /// </summary>
        /// <param name="fileName">Not required</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<UploadLogRecord> UploadFile(IFormFile uploadedFile, [FromQuery] string fileName)
        {
            if (uploadedFile != null)
            {
                var metadata = FileMetadata.FromFormFile(uploadedFile, fileName);
                using (var fileStream = uploadedFile.OpenReadStream())
                {
                    await _filesCloud.SaveFile(metadata.FileName, fileStream, metadata);
                }
                return await AddLogRecord(metadata.FileName);
            }
            else
            {
                throw new Exception("File not uploaded.");
            }
        }

        [HttpGet("{*path}")]
        public virtual async Task<IActionResult> DownloadFile([FromRoute] string path)
        {
            var metadata = await TryGetMetadata(path);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (metadata == null)
            {
                var fileNames = new List<string>();
                if (path != "root" && path != "root/")
                {
                    var metadataList = await SearchFiles(path);
                    fileNames = metadataList
                        .Select(r => r.FileName)
                        .ToList();

                }
                if (!fileNames.Any())
                {
                    throw new Exception($"File or directory '{path}' not found.");
                }
                await PrintFilesList(fileNames);
                return new OkResult();
            }
            else
            {
                var fileStream = await _filesCloud.LoadFile(path);
                var shortFileName = Path.GetFileName(metadata.FileName);
                if (metadata.CanBeOpenedInBrowser())
                {
                    Response.Headers.Add("Content-Disposition", "inline; filename=" + shortFileName);
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    return new FileStreamResult(
                        fileStream,
                        metadata.ContentType
                    );
                }
                else
                {
                    return File(
                        fileStream,
                        metadata.ContentType,
                        shortFileName,
                        metadata.LastModified,
                        null
                    );
                }

            }
        }

        async Task<UploadLogRecord> AddLogRecord(string fileName)
        {
            var urlFileName = CustomUrlEncode(fileName);

            var record = new UploadLogRecord()
            {
                UploadedAt = DateTime.UtcNow,
                FileName = fileName,
                DownloadUrl = LinkTemplate + urlFileName
            };
            var list = await GetLogRecordsList();
            list.Add(record);
            await _storage.Set(UploadsLogCacheKey, list);
            return record;
        }

        string CustomUrlEncode(string fileName)
        {
            var spaceKey = "1122SPACE2211";
            var slashKey = "1122SLASH2211";
            var backSlashKey = "1122BACKSLASH2211";
            var urlFileName = fileName
                .Replace(" ", spaceKey)
                .Replace("/", slashKey)
                .Replace("\\", backSlashKey);

            urlFileName = HttpUtility.UrlEncode(urlFileName);

            urlFileName = urlFileName
                .Replace(spaceKey, "%20")
                .Replace(slashKey, "/")
                .Replace(backSlashKey, "\\");
            return urlFileName;
        }

        async Task<List<UploadLogRecord>> GetLogRecordsList()
        {
            return await _storage.GetOrDefault<List<UploadLogRecord>>(UploadsLogCacheKey) ?? new List<UploadLogRecord>();

        }

        async Task<List<FileMetadata>> SearchFiles(string pathStartsWith)
        {
            var logRecList = await GetLogRecordsList();
            var metadataList = new List<FileMetadata>();
            var addedBefore = new HashSet<string>();
            foreach (var rec in logRecList)
            {
                if (!rec.FileName.StartsWith(pathStartsWith) || addedBefore.Contains(rec.FileName))
                {
                    continue;
                }

                var metadata = await TryGetMetadata(rec.FileName);
                if (metadata != null)
                {
                    metadataList.Add(metadata);
                }
                addedBefore.Add(rec.FileName);
            }
            return metadataList;
        }

        async Task<FileMetadata> TryGetMetadata(string key)
        {
            try
            {
                var metadata = await _filesCloud.GetFileMetadata(key);
                return metadata;
            }
            catch
            {
                return null;
            }
        }

        async Task PrintFilesList(List<string> fileNames)
        {
            var anchorsHtml = "";
            foreach (var fileName in fileNames)
            {
                var urlFileName = CustomUrlEncode(fileName);
                var url = LinkTemplate + urlFileName;
                var anchor = $"<a href=\"{url}\">{fileName}</a><br>\n";
                anchorsHtml += anchor;
            }

            var documentHtml = @"
                 <!DOCTYPE html>
                 <html>
                 <body>" +
                   anchorsHtml +
                 @"</body>
                 </html>";
            await Response.WriteAsync(documentHtml);
            Response.ContentType = "text/html";
        }
    }
}