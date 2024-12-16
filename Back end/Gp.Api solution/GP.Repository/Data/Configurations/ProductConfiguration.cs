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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.ProductName).IsRequired().HasMaxLength(100);
            builder.Property(P => P.ProductUrl).HasMaxLength(1000).IsRequired(false);

            builder.Property(P => P.PictureUrl).IsRequired();
            builder.Property(P => P.ProductPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(P => P.ProductWeight).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(P => P.Category).WithMany().HasForeignKey(p=>p.CategoryId);


 






        }
    }
}
