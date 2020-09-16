using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace CrmSystem.Server.Utils
{
    public static class FileUtils
    {
        public static async Task<FileContentResult> GetFileContent(string path, string fileName, HttpContext context)
        {
            string contentType = GetMimeTypeByWindowsRegistry(Path.GetFileName(path));

            context.Response.ContentType = contentType;
            var result =
                new FileContentResult(await File.ReadAllBytesAsync(path), contentType)
                {
                    FileDownloadName = fileName
                };

            return result;
        }

        public static string GetMimeTypeByWindowsRegistry(string fileNameOrExtension)
        {
            string mimeType = "application/unknown";
            string ext = (fileNameOrExtension.Contains(".")) ? System.IO.Path.GetExtension(fileNameOrExtension).ToLower() : "." + fileNameOrExtension;
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null) mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}