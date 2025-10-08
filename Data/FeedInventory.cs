using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class FeedInventory
    {
        [Key]
        public int Id { get; set; }

        public int Tons_qty { get; set; }

        public decimal ton_cost { get; set; }

        public DateTime Date { get; set; }

        public decimal total {  get; set; }
        public FeedInventory () {

            total = ton_cost * Tons_qty;
            }
    }
}
