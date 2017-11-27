using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace LightMethods.Survey.Models.DAL
{
    public class AttachmentTypesRepository : Repository<AttachmentType>
    {

        public AttachmentTypesRepository(UnitOfWork uow)
            : base(uow)
        {

        }

        private static AttachmentType EnsureHasValue(ref AttachmentType prop)
        {
            if (prop != null)
                return prop;

            using (var uow = new UnitOfWork(new SurveyContext()))
            {
                var types = uow.AttachmentTypesRepository.AllAsNoTracking.ToList();
                _Document = types.Where(i => i.Name == "Document").Single();
                _Image = types.Where(i => i.Name == "Image").Single();
                _Audio = types.Where(i => i.Name == "Audio").Single();
                _Video = types.Where(i => i.Name == "Video").Single();
            }

            return prop;
        }

        public AttachmentType Parse(string name)
        {
            if (name == null) return null;

            return this.AllAsNoTracking.Where(i => i.Name == name).SingleOrDefault();
        }

        public static AttachmentType FromExtension(string ext)
        {
            ext = ext.TrimStart('.').ToLower();

            if (Document.AllowedExtensions.Contains(ext)) return Document;
            if (Image.AllowedExtensions.Contains(ext)) return Image;
            if (Audio.AllowedExtensions.Contains(ext)) return Audio;
            if (Video.AllowedExtensions.Contains(ext)) return Video;
            return null;
        }

        private static AttachmentType _Document;
        public static AttachmentType Document { get { return EnsureHasValue(ref _Document); } }

        private static AttachmentType _Image;
        public static AttachmentType Image { get { return EnsureHasValue(ref _Image); } }

        private static AttachmentType _Audio;
        public static AttachmentType Audio { get { return EnsureHasValue(ref _Audio); } }

        private static AttachmentType _Video;
        public static AttachmentType Video { get { return EnsureHasValue(ref _Video); } }
    }
}
