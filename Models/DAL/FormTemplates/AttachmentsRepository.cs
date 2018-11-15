using LightMethods.Survey.Models.Entities;
using System;
using System.IO;

namespace LightMethods.Survey.Models.DAL
{
    public class AttachmentsRepository : Repository<Attachment>
    {
        public static string RootFolderPath { set; get; }

        public AttachmentsRepository(UnitOfWork uow) : base(uow) { }

        public override void Delete(Attachment entity)
        {
            var fileInfo = new FileInfo(Path.Combine(RootFolderPath, entity.RelativeFolder, entity.NameOnDisk));
            if (fileInfo.Exists)
                fileInfo.Delete();

            base.Delete(entity);
        }

        public Attachment CreateAttachment(FileInfo fileInfo, FormValue formValue)
        {
            var attachment = new Attachment
            {
                FormValue = formValue,
                FileName = fileInfo.FullName,
                TypeId = AttachmentTypesRepository.FromExtension(fileInfo.Extension).Id,
                FileSize = -1 // not yet copied to the file repository
            };

            return attachment;
        }

        public void StoreFile(Attachment attachment)
        {
            var tempFileInfo = new FileInfo(attachment.FileName);
            var newFileLoc = $"{RootFolderPath}\\{attachment.RelativeFolder}";

            if (!Directory.Exists(newFileLoc))
                Directory.CreateDirectory(newFileLoc);

            var fileName = tempFileInfo.Name;
            var location = $"{newFileLoc}\\{ attachment.Id}{tempFileInfo.Extension}";
            if (!System.IO.File.Exists(location))
                tempFileInfo.MoveTo(location);

            attachment.FileName = fileName;
            attachment.FileSize = Convert.ToInt32(tempFileInfo.Length);
        }
    }
}
