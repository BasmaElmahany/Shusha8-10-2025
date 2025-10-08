using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shusha_project_BackUp.Data
{
    public class Request_Proceeds
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("center")]
        public int? centerID { get; set; }

        public Center? center { get; set; }  

        public decimal requested_proceeds { get; set; } 





    }
}
