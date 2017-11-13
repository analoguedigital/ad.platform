using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.DAL;
using AutoMapper;

namespace WebApi.Models
{
    public class MetricGroupDTO
    {
        public Guid Id { set; get; }

        public String Title { get; set; }

        public int Page { get; set; }

        public string HelpContext { get; set; }

        public bool IsRepeater { set; get; }

        public bool IsDataListRepeater { set; get; }

        public bool IsAdHoc { set; get; }

        public ICollection<DataListItemDTO> AdHocItems { set; get; }

        public string Type { set; get; }

        public Guid? DataListId { set; get; }

        public int? NumberOfRows { set; get; }

        public bool CanAddMoreRows { set; get; }

        public int Order { set; get; }

        public Guid FormTemplateId { get; set; }

        public ICollection<MetricDTO> Metrics { set; get; }

        public bool isDeleted { set; get; }

        public MetricGroup Map(MetricGroup entity, UnitOfWork uow, Organisation org)
        {
            var newAdHocItems = this.AdHocItems.Where(i => !i.IsDeleted).Select(i => Mapper.Map<DataListItem>(i));
            var deletedAdHocItems = this.AdHocItems.Where(i => i.IsDeleted).Select(i => Mapper.Map<DataListItem>(i));
            var oldDataList = entity?.DataList;
            entity = this.Map(entity);
            if (IsRepeater)
            {
                if (IsDataListRepeater)
                {
                    entity.NumberOfRows = null;
                }
                else
                {
                    entity.DataListId = null;
                }
            }
            else
            {
                IsAdHoc = false;
                entity.NumberOfRows = null;
                entity.DataListId = null;
            }

            entity.UpdateDataList(uow, org, IsAdHoc, newAdHocItems, deletedAdHocItems, DataListId, oldDataList);
            return entity;
        }

        private MetricGroup Map(MetricGroup entity)
        {
            if (entity == null)
                entity = Mapper.Map<MetricGroup>(this); // new metricgroup
            else
                Mapper.Map(this, entity);

            return entity;
        }
    }
}
