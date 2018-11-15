using System;

namespace LightMethods.Survey.Models.Entities
{
    public class Attachment : Entity
    {
        public Guid FormValueId { set; get; }

        public virtual FormValue FormValue { set; get; }

        public string FileName { set; get; }

        public Guid TypeId { set; get; }

        public virtual AttachmentType Type { set; get; }

        public int FileSize { set; get; }

        public bool IsTemp
        {
            get { return FileSize == -1; }
        }

        public string Url
        {
            get
            {
                return $"/attachments/{RelativeFolder.Replace('\\', '/')}/{NameOnDisk}";
            }
        }

        public string RelativeFolder
        {
            get
            {
                var formValue = this.FormValue;

                return $"{formValue.FilledForm.DateCreated.Year}\\{formValue.FilledForm.DateCreated.Month}\\{formValue.FilledFormId}\\{formValue.MetricId}";
            }
        }

        public string NameOnDisk
        {
            get
            {
                return $"{this.Id.ToString() + System.IO.Path.GetExtension(this.FileName)}";
            }
        }

    }
}
