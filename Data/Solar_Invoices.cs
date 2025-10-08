using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Solar_Invoices
    {
        [Key]
        public int invoice_id { get; set; }

        public decimal amount {  get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("branch")]
        public int BranchID { get; set; }
        public Branch? branch { get; set; }


    }
}
