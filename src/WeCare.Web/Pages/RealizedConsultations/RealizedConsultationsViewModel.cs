using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace WeCare.Web.Pages.RealizedConsultations
{
    // ViewModel principal para a página de visualização
    public class ConsultationHistoryViewModel
    {
        public List<ObjectiveDisplayViewModel> Objectives { get; set; } = new();
    }

    // ViewModel para exibir um objetivo e suas consultas
    public class ObjectiveDisplayViewModel
    {
        public string Name { get; set; }
        public int Progress { get; set; }
        public List<ConsultationItemViewModel> Consultations { get; set; } = new();
    }

    // ViewModel para exibir um item de consulta na lista
    public class ConsultationItemViewModel
    {
        public string TherapistName { get; set; }
        public string TherapistSpecialization { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }

    // ViewModel para o formulário do modal "Adicionar Novo Objetivo"
    public class ObjectiveViewModel
    {
        [HiddenInput]
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }

        [Required]
        [Display(Name = "Nome do Objetivo")]
        public string ObjectiveName { get; set; }

        [Required]
        [SelectItems("TherapistLookup")]
        [Display(Name = "Primeiro Terapeuta")]
        public Guid TherapistId { get; set; }

        [Required]
        [Display(Name = "Data da Primeira Consulta")]
        [DataType(DataType.Date)]
        public DateTime FirstConsultationDate { get; set; }

        [Required]
        [Display(Name = "Hora da Primeira Consulta")]
        [DataType(DataType.Time)]
        public string FirstConsultationTime { get; set; }
    }
}