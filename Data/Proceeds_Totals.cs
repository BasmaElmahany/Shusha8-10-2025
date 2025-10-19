using Shusha_project_BackUp.Data.Migrations;

namespace Shusha_project_BackUp.Data
{
    public class Proceeds_Totals
    {
        public int id { get; set; }


        public DateOnly Date {  get; set; }

        public decimal Egg { get; set; }
        public decimal broken_Egg { get; set; }
        public decimal double_Egg { get; set; }

        public decimal herd { get; set; }

        public decimal Waste { get; set; }

        public decimal waste_fees { get; set; }

        public decimal Miscellaneous { get; set; }

        public decimal total { get; set; }
        public decimal calculateTotal()
        {
            return total = Egg + broken_Egg + double_Egg+ herd+ Waste+ waste_fees+ Miscellaneous;
        }

        public void UpdateTotal()
        {
            total = calculateTotal();
        }

    }
}
