 using GP.Core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Voice.V1.DialingPermissions;

namespace GP.Repository.Data.Configurations
{
    public class OrderConfigration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(O => O.ShippingAddress, SA => SA.WithOwner()); //1-1 total

            builder.Property(O => O.Status)
                .HasConversion(
                   os => os.ToString(),
                   os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

       
         builder.HasOne(O=>O.Request).WithOne().HasForeignKey<Order>(s => s.RequestId);
                

        }
    }
}
