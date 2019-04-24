using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace WebApi.Services
{
    public class UploadsCleanupService
    {
        public void CleanupUploadsDirectory()
        {
            var uploadsPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/");
            if (Directory.Exists(uploadsPath))
            {
                var uploadsDir = new DirectoryInfo(uploadsPath);
                foreach(var file in uploadsDir.EnumerateFiles())
                    file.Delete();

                foreach (var subdir in uploadsDir.EnumerateDirectories())
                    subdir.Delete(true);
            }
        }
    }
}