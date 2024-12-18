using GP.Core.Entities;

namespace Gp.Api.Dtos
{
    public class ShipmentToDto
    {
        public int Id { get; set; }

     

        public decimal? Weight { get; set; }

        public DateTime? Dateofcreation { get; set; }=DateTime.Now;

        public int? FromCityID { get; set; }
        public string? FromCityName { get; set; }

        public int? CountryIdFrom { get; set; }

        public string? CountryNameFrom { get; set; }

        public int? ToCityId { get; set; }

        public string? ToCityName { get; set; }

        public int? CountryIdTo { get; set; }

        public string? CountryNameTo { get; set; }

        public DateTime? DateOfRecieving { get; set; }
        public string? Address { get; set; }

        // public List<ProductsDto> Products { get; set; }
        public ICollection<ProductsDto>? Products { get; set; } = new HashSet<ProductsDto>();
       
        //public int ProductId { get; set; }
        //public string ProductName { get; set; }
        //public decimal ProductPrice { get; set; }
        //public decimal ProductWeight { get; set; }

            //public string? PictureUrl { get; set; }

            //public string? ProductUrl { get; set; }

            //// public IFormFile? Image {  get; set; }

        //public int? CategoryId { get; set; }

        //public string CategoryName { get; set; }

        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserPicture{ get; set;}
    }
}
