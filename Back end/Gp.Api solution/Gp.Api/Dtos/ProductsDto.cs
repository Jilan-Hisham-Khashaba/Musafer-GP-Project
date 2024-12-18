namespace Gp.Api.Dtos
{
    public class ProductsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string? ProductUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductWeight { get; set; }

        public string? PictureUrl { get; set; }
        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}
