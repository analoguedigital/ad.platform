using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using LightMethods.Survey.Models.FilterValues;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using WebApi.Models;

namespace WebApi
{
    public class AutoMapperConfig
    {
        public static void Config()
        {

            #region Projects and Assignments

            Mapper.CreateMap<Project, ProjectDTO>().ReverseMap();
            Mapper.CreateMap<AssignmentDTO, ProjectDTO>();
            Mapper.CreateMap<Assignment, ProjectAssignmentDTO>()
                .AfterMap((src, dest) => { dest.OrgUserName = src.OrgUser.ToString(); });
            Mapper.CreateMap<ProjectAssignmentDTO, Assignment>();

            #endregion Projects and Assignments

            #region Form Templates and Categories

            Mapper.CreateMap<FormTemplateCategory, FormTemplateCategoryDTO>().ReverseMap();
            Mapper.CreateMap<FormTemplate, FormTemplateDTO>();

            Mapper.CreateMap<FormTemplateDTO, FormTemplate>()
                .ForMember(f => f.MetricGroups, opt => opt.Ignore());
            Mapper.CreateMap<Controllers.FormTemplatesController.EditBasicDetailsRequest, FormTemplate>();

            #endregion Form Templates and Categories

            #region Data Lists

            Mapper.CreateMap<DataList, DataListDTO>();
            Mapper.CreateMap<DataListDTO, DataList>()
                .ForMember(x => x.AllItems, opt => opt.MapFrom(src => src.Items));
            Mapper.CreateMap<DataListItem, DataListItemDTO>();
            Mapper.CreateMap<DataListItemDTO, DataListItem>().ForMember(f => f.Attributes, opt => opt.Ignore());
            Mapper.CreateMap<DataListItemAttr, DataListItemAttrDTO>().ReverseMap();
            Mapper.CreateMap<DataListRelationship, DataListRelationshipDTO>();
            Mapper.CreateMap<DataListRelationshipDTO, DataListRelationship>();
            //.ForMember(dest => dest.Owner, opts => opts.Ignore())
            //.ForMember(dest => dest.DataList, opts => opts.Ignore());

            Mapper.CreateMap<DataList, GetDataListsResItemDto>();

            #endregion Data Lists

            #region Metric Groups

            Mapper.CreateMap<MetricGroup, MetricGroupDTO>()
                .AfterMap((src, dest) =>
                {
                    dest.IsAdHoc = src.DataList?.IsAdHoc ?? false;
                    dest.IsDataListRepeater = src.DataListId.HasValue;
                    dest.AdHocItems = src.DataList?.IsAdHoc ?? false ? src.DataList.Items.Select(i => Mapper.Map<DataListItemDTO>(i)).ToList() : Enumerable.Empty<DataListItemDTO>().ToList();
                });
            Mapper.CreateMap<MetricGroupDTO, MetricGroup>()
                .ForMember(f => f.Metrics, opt => opt.Ignore());

            #endregion Metric Groups

            #region Metrics

            // config metrics mappings
            Mapper.CreateMap<Metric, MetricDTO>()
               .Include<DateMetric, DateMetricDTO>()
               .Include<TimeMetric, TimeMetricDTO>()
               .Include<DichotomousMetric, DichotomousMetricDTO>()
               .Include<FreeTextMetric, FreeTextMetricDTO>()
               .Include<MultipleChoiceMetric, MultipleChoiceMetricDTO>()
               .Include<NumericMetric, NumericMetricDTO>()
               .Include<RateMetric, RateMetricDTO>()
               .Include<AttachmentMetric, AttachmentMetricDTO>()
               .AfterMap((src, dest) => dest.Type = ObjectContext.GetObjectType(src.GetType()).Name);

            Mapper.CreateMap<MetricDTO, Metric>()
               .Include<DateMetricDTO, DateMetric>()
               .Include<TimeMetricDTO, TimeMetric>()
               .Include<DichotomousMetricDTO, DichotomousMetric>()
               .Include<FreeTextMetricDTO, FreeTextMetric>()
               .Include<MultipleChoiceMetricDTO, MultipleChoiceMetric>()
               .Include<NumericMetricDTO, NumericMetric>()
               .Include<RateMetricDTO, RateMetric>()
               .Include<AttachmentMetricDTO, AttachmentMetric>();

            Mapper.CreateMap<DateMetric, DateMetricDTO>().ReverseMap();
            Mapper.CreateMap<TimeMetric, TimeMetricDTO>().ReverseMap();
            Mapper.CreateMap<DichotomousMetric, DichotomousMetricDTO>().ReverseMap();
            Mapper.CreateMap<FreeTextMetric, FreeTextMetricDTO>().ReverseMap();
            Mapper.CreateMap<MultipleChoiceMetric, MultipleChoiceMetricDTO>()
                .AfterMap((src, dest) =>
                {
                    dest.IsAdHoc = src.DataList?.IsAdHoc ?? false;
                    dest.AdHocItems = src.DataList?.IsAdHoc ?? false ? src.DataList.Items.Select(i => Mapper.Map<DataListItemDTO>(i)).ToList() : Enumerable.Empty<DataListItemDTO>().ToList();
                });
            Mapper.CreateMap<MultipleChoiceMetricDTO, MultipleChoiceMetric>();
            Mapper.CreateMap<NumericMetric, NumericMetricDTO>().ReverseMap();
            Mapper.CreateMap<RateMetricDTO, RateMetric>();
            Mapper.CreateMap<RateMetric, RateMetricDTO>()
                .AfterMap((src, dest) =>
                {
                    dest.IsAdHoc = src.DataList?.IsAdHoc ?? false;
                    dest.AdHocItems = src.DataList?.IsAdHoc ?? false ? src.DataList.Items.Select(i => Mapper.Map<DataListItemDTO>(i)).ToList() : Enumerable.Empty<DataListItemDTO>().ToList();
                });
            Mapper.CreateMap<AttachmentDTO, Attachment>();
            Mapper.CreateMap<Attachment, AttachmentDTO>()
                .AfterMap((src, dest) => { dest.TypeString = src.Type.Name; });

            Mapper.CreateMap<AttachmentMetric, AttachmentMetricDTO>();
            Mapper.CreateMap<AttachmentMetricDTO, AttachmentMetric>()
                .ForMember(m => m.AllowedAttachmentTypes, opt => opt.Ignore());

            #endregion Metrics

            #region Form Values and Filled Forms

            Mapper.CreateMap<FormValue, FormValueDTO>().ForMember(m => m.TimeValue, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (!src.TimeValue.HasValue) return;
                    dest.TimeValue = DateTime.Today.AddMinutes(src.TimeValue.Value.TotalMinutes);
                });
            Mapper.CreateMap<FormValueDTO, FormValue>()
                .ForMember(m => m.Attachments, opt => opt.Ignore())
                .ForMember(m => m.TimeValue, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (!src.TimeValue.HasValue) return;
                    dest.TimeValue = new System.TimeSpan(src.TimeValue.Value.Hour, src.TimeValue.Value.Minute, 0);
                });
            Mapper.CreateMap<FilledForm, FilledFormDTO>();
            Mapper.CreateMap<FilledFormDTO, FilledForm>()
                .ForMember(m => m.FilledBy, opt => opt.Ignore());

            Mapper.CreateMap<FilledFormLocation, FilledFormLocationDTO>().ReverseMap();

            #endregion Form Values and Filled Forms

            #region Organisations and Users

            Mapper.CreateMap<Organisation, OrganisationDTO>().ReverseMap();
            Mapper.CreateMap<OrgUser, OrgUserDTO>();
            Mapper.CreateMap<OrgUserDTO, OrgUser>()
                .ForMember(dest => dest.Type, opts => opts.Ignore())
                .AfterMap((src, dest) => { dest.TypeId = src.Type.Id; });
            Mapper.CreateMap<OrgUserType, OrgUserTypeDTO>().ReverseMap();

            Mapper.CreateMap<PaymentRecord, PaymentRecordDTO>().ReverseMap();
            Mapper.CreateMap<PromotionCode, PromotionCodeDTO>().ReverseMap();
            Mapper.CreateMap<Subscription, SubscriptionDTO>().ReverseMap();

            #endregion Organisations and Users

            Mapper.CreateMap<Feedback, FeedbackDTO>().ReverseMap();
        }
    }
}