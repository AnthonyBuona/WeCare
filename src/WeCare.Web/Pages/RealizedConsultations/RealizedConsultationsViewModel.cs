using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using WeCare.Application.Contracts.Consultations;

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
    public class ObjectiveGroupViewModel
    {
        public List<ObjectiveGroupDto> ObjectiveGroups { get; set; } = new();
    }
}