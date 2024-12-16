using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Data.Configurations
{
    internal class CategoryConfigratuon : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.TypeName).IsRequired().HasMaxLength(100);

            //builder.HasMany(C => C.products)
            // .WithOne(c => c.Category)
            // .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
