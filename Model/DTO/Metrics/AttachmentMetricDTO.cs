using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DTO
{
    public class AttachmentMetricDTO : MetricDTO
    {
        public bool AllowMultipleFiles { set; get; }

        public IList<string> AllowedAttachmentTypes { set; get; }

        public override Metric Map(Metric entity, UnitOfWork uow, Organisation org)
        {
            entity = base.Map(entity, uow, org);

            var attachmentMetric = (AttachmentMetric)entity;

            var newList = this.AllowedAttachmentTypes
                .Select(t => uow.AttachmentTypesRepository.Parse(t))
                .Where(type => type != null)
                .ToList();

            var deletedTypes = attachmentMetric.AllowedAttachmentTypes.Except(newList, i => i.Id).ToList();
            var addedTypes = newList.Except(attachmentMetric.AllowedAttachmentTypes, i => i.Id).ToList();

            foreach (var type in deletedTypes)
                attachmentMetric.AllowedAttachmentTypes.Remove(type);

            foreach (var type in addedTypes)
            {
                if (uow.Context.Entry(type).State == System.Data.Entity.EntityState.Detached)
                    uow.Context.AttachmentTypes.Attach(type);

                attachmentMetric.AllowedAttachmentTypes.Add(type);
            }

            return entity;
        }
    }
}
