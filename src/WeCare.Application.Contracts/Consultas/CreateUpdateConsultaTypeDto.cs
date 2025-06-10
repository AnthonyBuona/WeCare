using System;
using System.ComponentModel.DataAnnotations;

namespace WeCare.Consultas
{
    public class CreateUpdateTratamentoDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid TherapistId { get; set; }

        [Required]
        [StringLength(64)]
        [Display(Name = "Tipo de Consulta")]
        public string TipoConsulta { get; set; } = string.Empty;
    }
}
