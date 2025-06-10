using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace WeCare.Tratamentos
{
    public class CreateUpdateTratamentoDto
    {
        [Required]
        [SelectItems("PatientLookup")] // Dropdown de Pacientes
        [Display(Name = "Paciente")]
        public Guid PatientId { get; set; }

        // --- CORREÇÃO AQUI ---
        [Required]
        [SelectItems("TherapistLookup")] // Dropdown de Terapeutas
        [Display(Name = "Terapeuta")]
        public Guid TherapistId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tipo de Tratamento")]
        public string Tipo { get; set; } // Ajuste o nome da propriedade se necessário
    }
}