using System.ComponentModel.DataAnnotations;

namespace Shusha_project_BackUp.Data
{
    public class FeedPurchases
    {
        [Key]
        public int Id { get; set; }

        public int Tons_qty { get; set; }

        public decimal price { get; set; }

        public DateTime Date { get; set; }

        public decimal total { get; set; }
        public FeedPurchases()
        {

            total = price * Tons_qty;
        }
    }
}
