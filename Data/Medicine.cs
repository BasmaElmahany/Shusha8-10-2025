using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class Medicine
    {
        [Key]
        public int ID { get; set; }


        [Required]
        public string Name { get; set; }



        [Required]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; } = 0;
        [Required]
        public string Type { get; set; }
        [Required]
        public DateTime? ProductionDate { get; set; }

        [Required]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public decimal Price { get; set; }

       

        
    }
}
