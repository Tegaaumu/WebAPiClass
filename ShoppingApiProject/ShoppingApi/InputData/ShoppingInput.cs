using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.InputData
{
    public class ShoppingInput
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductImage { get; set; }

        [Required]
        public double ProductPriceM { get; set; }
        public double ProductPriceF { get; set; }
        public string BrandName { get; set; }

        public string CompanyImage { get; set; }

        [Required]
        public string Categpories { get; set; }
        
        [Required]
        [Range(0, 5, ErrorMessage = "Please the rating can only be from 1 to 5")]
        public double StarRatings { get; set; }


        [Required]
        public int ItemsRemaining { get; set; }
    }
}
