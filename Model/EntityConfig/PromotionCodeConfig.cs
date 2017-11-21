﻿using LightMethods.Survey.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.EntityConfig
{
    public class PromotionCodeConfig : EntityTypeConfiguration<PromotionCode>
    {
        public PromotionCodeConfig()
        {
            this.Property(x => x.Title)
                .HasMaxLength(50)
                .IsRequired();

            this.Property(x => x.Code)
                .HasMaxLength(10)
                .IsFixedLength()
                .IsRequired();

            this.HasRequired(x => x.Organisation)
                .WithMany(x => x.PromotionCodes)
                .HasForeignKey(x => x.OrganisationId)
                .WillCascadeOnDelete();

            this.HasOptional(x => x.PaymentRecord)
                .WithOptionalPrincipal(x => x.PromotionCode)
                .WillCascadeOnDelete(false);
        }
    }
}
