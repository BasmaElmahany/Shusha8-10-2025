using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class Budget
    {
        [Key]
        public int id { get; set; } 

        public int year { get; set; }

        public decimal egg { get; set; }

        public decimal waste { get; set; }

        public decimal herd { get; set; }

        public decimal total { get; set; }
        public decimal Miscellaneous { get; set; }
        public decimal calculateTotal()
        {
            return total= egg + waste + herd+ Miscellaneous;
        }

        public void UpdateTotal()
        {
            total = calculateTotal();
        }
    }
}
