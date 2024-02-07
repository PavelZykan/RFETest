using System.ComponentModel.DataAnnotations;

namespace RFETest.WebContracts
{
    public class DiffInput
    {
        [Required(AllowEmptyStrings = false)]
        public string Input { get; set; }
    }
}
