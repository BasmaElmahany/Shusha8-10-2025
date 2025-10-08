
using System.Collections.Generic;

namespace Shusha_project_BackUp.Models
{
    public class WardSalesViewModel
    {
       public double emp_expenses {  get; set; }
        public double feed_expenses { get; set; }
        public double feedpuchases_expenses { get; set; }
        public double medicine_expenses { get; set; }
        public double Electricity_Invoices_expenses { get; set; }
        public double solar_Invoices_expenses { get; set; }

        public double Total {  get; set; }

        public List<dynamic> Sales { get; set; }

        public double netProfit { get; set; }

    }
}
