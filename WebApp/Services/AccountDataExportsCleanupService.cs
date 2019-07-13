using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApi.Services
{
    public class AccountDataExportsCleanupService
    {
        public void CleanupAccountDataExports()
        {
            var rootFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/_AccountDataExport/");
            if (Directory.Exists(rootFolder))
            {
                var rootDir = new DirectoryInfo(rootFolder);
                var directories = rootDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (var dir in directories)
                {
                    var diff = DateTime.Now - dir.CreationTime;
                    if (diff.Minutes >= 5)
                    {
                        dir.Delete(true);
                    }
                }
            }
        }
    }
}