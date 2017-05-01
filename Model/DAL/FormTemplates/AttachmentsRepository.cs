using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace LightMethods.Survey.Models.DAL
{
    public class AttachmentsRepository : Repository<Attachment>
    {

        public static string RootFolderPath { set; get; }

        public AttachmentsRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        public override void Delete(Attachment entity)
        {
            
            var fileInfo = new FileInfo (Path.Combine(RootFolderPath, entity.RelativeFolder, entity.NameOnDisk));
            if (fileInfo.Exists)
                fileInfo.Delete();

            base.Delete(entity);
        }

        public Attachment CreateAttachment(FileInfo fileInfo, FormValue formValue)
        {
            var attachment = new Attachment();
            attachment.FormValue = formValue;
            attachment.FileName = fileInfo.FullName;
            attachment.TypeId = AttachmentTypesRepository.FromExtension(fileInfo.Extension).Id;
            attachment.FileSize = -1; // not yet copied to the file repository
            return attachment;
        }

        public void StoreFile(Attachment attachment)
        {
            var tempFileInfo = new FileInfo(attachment.FileName);
            var newFileLoc = $"{RootFolderPath}\\{attachment.RelativeFolder}";

            if (!Directory.Exists(newFileLoc))
                Directory.CreateDirectory(newFileLoc);

            var fileName = tempFileInfo.Name;
            tempFileInfo.MoveTo($"{newFileLoc}\\{ attachment.Id}{tempFileInfo.Extension}");

            attachment.FileName = fileName;
            attachment.FileSize = Convert.ToInt32(tempFileInfo.Length);


        }
    }
}
