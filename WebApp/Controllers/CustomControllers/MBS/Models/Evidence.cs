using AutoMapper;
using LightMethods.Survey.Models.DTO;
using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Models.MBS
{
    public class EvidenceDTO
    {
        public Guid SurveyId { set; get; }

        public DateTime? Date { set; get; }

        public string Target { set; get; }

        public string Comments { set; get; }

        public IList<AttachmentDTO> attachments { set; get; }

        public static EvidenceDTO From(FilledForm form)
        {
            return new EvidenceDTO
            {
                SurveyId = form.Id,
                Date = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("d7ec4638-e111-4e84-a25a-1ff3bf7f5e5d")).FirstOrDefault()?.DateValue,
                Comments = form.FormValues.Where(fv => fv.MetricId == Guid.Parse("587dbe10-a531-4f24-87cd-08d1c61c5e55")).FirstOrDefault().TextValue,
                attachments =  form.FormValues.Where(fv => fv.MetricId == Guid.Parse("4dec02b8-cbf3-4603-9e5e-1b6ed9603b69")).FirstOrDefault().Attachments.Select(a=> Mapper.Map<AttachmentDTO>(a)).ToList()
            };
        }

        
    }
}