namespace Shusha_project_BackUp.Models
{
    public class Daily_Production_View
    {
        public int Id { get; set; }
        public string Ward_name { get; set; }
        public int No_of_Wheggs { get; set; }
        public int No_of_Breggs { get; set; }
        public int Whdead_herd { get; set; }
        public int Brdead_herd { get; set; }
        public int double_eggs { get; set; }

        public int no_of_carton_broken { get; set; }
       
        public double Feed_Inventory { get; set; }
        public DateOnly Date { get; set; }
    }
}
