using System.ComponentModel.DataAnnotations;

namespace WeCare.PeriodicReports
{
    public class ParentSignReportDto
    {
        [Required]
        [StringLength(15)]
        public string ResponsibleSignatureCPF { get; set; }

        [Required]
        [StringLength(50)]
        public string ResponsibleSignatureIP { get; set; }
    }
}
