using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class Proceed
    {
        [Key]
        public int id {  get; set; }

        [ForeignKey("center")]
        public int centerId { get; set; }   
        public Center? center { get; set; }

        public decimal amount { get; set; }

        public decimal rest_amount { get; set; } 

        public DateOnly Date { get; set; }
         
    }
}
