using GP.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Data.Configurations
{
    public class CommentsConfiguration : IEntityTypeConfiguration<Comments>
    {
        public void Configure(EntityTypeBuilder<Comments> builder)
        {
            builder.Property(c => c.Content).IsRequired().HasMaxLength(400);

            builder.Property(c => c.Rate).IsRequired().HasColumnType("decimal(18,2)");

            //builder.HasOne(P => P.User).WithMany().HasForeignKey(p => p.UserId);
        }
    
    }
}
