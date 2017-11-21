using AutoMapper;
using LightMethods.Survey.Models.DAL;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightMethods.Survey.Models.DTO
{
    public class MultipleChoiceMetricDTO : MetricDTO
    {
        public string ViewType { set; get; }

        public Guid DataListId { set; get; }

        public bool IsAdHoc { set; get; }

        public ICollection<DataListItemDTO> AdHocItems { set; get; }

        public override Metric Map(Metric entity, UnitOfWork uow, Organisation org)
        {
            entity = base.Map(entity, uow, org);

            var newAdHocItems = this.AdHocItems.Where(i => !i.IsDeleted).Select(i => Mapper.Map<DataListItem>(i));
            var deletedAdHocItems = this.AdHocItems.Where(i => i.IsDeleted).Select(i => Mapper.Map<DataListItem>(i));
            var oldDataList = ((MultipleChoiceMetric)entity).DataList;
            entity = base.Map(entity, uow, org);

            ((MultipleChoiceMetric)entity).UpdateDataList(uow, org, IsAdHoc, newAdHocItems, deletedAdHocItems, DataListId, oldDataList);
            return entity;
        }
    }
}
