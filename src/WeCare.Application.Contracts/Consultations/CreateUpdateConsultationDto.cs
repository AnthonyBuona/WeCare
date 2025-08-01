﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using System.Collections.Generic;
using WeCare.Application.Contracts.PerformedTrainings;

namespace WeCare.Consultations
{
    public class CreateUpdateConsultationDto
    {
        [Required]
        [SelectItems("PatientLookup")]
        [Display(Name = "Paciente")]
        public Guid PatientId { get; set; }

        [Required]
        [SelectItems("TherapistLookup")]
        [Display(Name = "Terapeuta")]
        public Guid TherapistId { get; set; }

        [Required]
        [Display(Name = "Data e Hora")]
        [DynamicFormIgnore]
        public DateTime DateTime { get; set; }


        [Required]
        [Display(Name = "Especialidade")]
        public string Specialty { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public string MainTraining { get; set; }
        public string Duration { get; set; }
        public List<CreateUpdatePerformedTrainingDto> PerformedTrainings { get; set; } = new();

    }
}