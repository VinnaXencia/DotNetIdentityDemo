using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetBrushUp.DataModels
{
    public class EmployeeDetail
    {
        [Key]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLocation { get; set; }
        public string EmployeePhoneNo { get; set; }
        public string? EmployeeProofFilePath { get; set; }
        public string? EmployeeProofFileName { get; set; }

        [NotMapped]
        public IFormFile? EmployeeProofFile { get; set; }
    }
}
