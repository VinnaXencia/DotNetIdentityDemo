using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DotNetBrushUp.DataModels
{
    public class ContactDataModel
    {
        [Key]
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhoneNo { get; set; }
        public string? ContactProofFilePath { get; set; }
        public string? ContactProofFileName { get; set; }

        [NotMapped]
        public IFormFile ContactProofFile { get; set; }
    }
}
