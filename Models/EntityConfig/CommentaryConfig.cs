﻿using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class CommentaryConfig : EntityTypeConfiguration<Commentary>
    {
        public CommentaryConfig()
        {
            this.HasRequired<Project>(c => c.Project)
                .WithMany()
                .WillCascadeOnDelete();
        }
    }
}