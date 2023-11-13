using NautralShop.Models;

namespace NautralShop.Areas.Admin.Models
{
    public class ProductsVM
    {
        public string ProductName { get; set; } = null!;

        public double? ProductPrice { get; set; }

        public IFormFile? IFormFileImage { get; set; }

        public double? ProductValuePromotion { get; set; }

        public string? ProductIngredient { get; set; }

        public string? ProductUseful { get; set; }

        public string? ProductUserManual { get; set; }

        public string? ProductDescription { get; set; }

        public string? ProductDetailDescription { get; set; }

        public bool ProductStatus { get; set; }

        public int? CategoryId { get; set; }


    }
}
