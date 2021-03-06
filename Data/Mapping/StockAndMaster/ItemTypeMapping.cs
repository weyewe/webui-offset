﻿using System.Data.Entity.ModelConfiguration;
using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Mapping
{
    public class ItemTypeMapping : EntityTypeConfiguration<ItemType>
    {
        public ItemTypeMapping()
        {
            HasKey(it => it.Id);
            HasMany(it => it.Items)
                .WithRequired(i => i.ItemType)
                .HasForeignKey(i => i.ItemTypeId);
            Ignore(it => it.Errors);
        }
    }
}