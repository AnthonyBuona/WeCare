using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace WeCare.Application.Contracts.Consultations
{
    // DTO que representa um Objetivo com suas consultas
    public class ObjectiveGroupDto
    {
        public string ObjectiveName { get; set; }
        public List<ConsultationInGroupDto> Consultations { get; set; } = new();
    }

    // DTO que representa uma consulta simplificada dentro de um grupo
    public class ConsultationInGroupDto : EntityDto<Guid>
    {
        public string TherapistName { get; set; }
        public string TherapistSpecialization { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }
}