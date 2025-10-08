using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp
{
    public class Contract
    {
        [Key]
        public int contract_id { get; set; }

        public string contract_description {  get; set; }

        public virtual Employees? employees { get; set; }

       
        [ForeignKey("employees")]
        public int  emp_id {  get; set; }
        [Range(0, double.MaxValue)]
        public decimal Gross_salary { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Medical_Assurance { get; set; }

        [Range(0, double.MaxValue)]
        public decimal taxes { get; set; }

        public decimal Incentives { get; set; }

       public  DateTime Date { get; set; }

        public decimal NetSalary { get; set; }

        // Method to calculate NetSalary
        public void CalculateNetSalary()
        {
            NetSalary = Gross_salary - Medical_Assurance - taxes + Incentives;
        }



    }
}
