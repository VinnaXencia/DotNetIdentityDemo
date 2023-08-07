using System.ComponentModel.DataAnnotations;

namespace DotNetBrushUp.DataModels
{
    public class EmployeeDetail
    {
        [Key]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLocation { get; set; }
        public string EmployeePhoneNo { get; set; }
    }
}
