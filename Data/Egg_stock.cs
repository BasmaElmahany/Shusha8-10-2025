namespace Shusha_project_BackUp.Data
{
    public class Egg_stock
    {
        public int Id { get; set; }

        public decimal plates_whiteEggs { get; set; }

        public decimal plates_BrownEggs { get; set; }

        public decimal brokenPlates { get; set; }

        public decimal doubleEggs { get; set; }

        public decimal total { get; set; }

        public void RecalculateTotal()
        {
            total = plates_whiteEggs + plates_BrownEggs + brokenPlates + doubleEggs;
        }


    }
}
