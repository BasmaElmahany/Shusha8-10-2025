namespace Shusha_project_BackUp.DTOs
{
    public class ProceedsWithCenterDto
    {
        public DateOnly Date { get; set; }
        public string CenterName { get; set; }
        public decimal Amount { get; set; }
        public decimal RestAmount { get; set; }
    }
}
