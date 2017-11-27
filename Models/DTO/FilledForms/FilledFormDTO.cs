﻿using System;
using System.Collections.Generic;

namespace LightMethods.Survey.Models.DTO
{
    public class FilledFormDTO
    {
        public Guid Id { set; get; }

        public int Serial { set; get; }

        public DateTime DateCreated { set; get; }

        public DateTime DateUpdated { set; get; }

        public Guid FormTemplateId { get; set; }

        public DateTime SurveyDate { set; get; }

        public Guid FilledById { set; get; }

        public string FilledBy { set; get; }

        public Guid ProjectId { set; get; }

        public IEnumerable<FormValueDTO> FormValues { set; get; }

        public IEnumerable<FilledFormLocationDTO> Locations { set; get; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
