using AutoMapper;
using WeCare.Books;
using WeCare.Tratamentos;
using WeCare.Patients;
using WeCare.Responsibles;
using System;
using WeCare.Shared;
using WeCare.Therapists;
using WeCare.Consultations;
using WeCare.Application.Contracts.Consultations;
using WeCare.Application.Contracts.PerformedTrainings;
using WeCare.PerformedTrainings;
using WeCare.Activities;
using WeCare.Trainings;
using WeCare.Objectives;
using WeCare.Clinics;
using WeCare.Guests;
using WeCare.Attendances;
using WeCare.PeriodicReports;
using WeCare.CrossTenantAccess;
using WeCare.Billing;
using WeCare.Gamification;

namespace WeCare
{
    public class WeCareApplicationAutoMapperProfile : Profile
    {
        public WeCareApplicationAutoMapperProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<CreateUpdateBookDto, Book>();

            CreateMap<Patient, PatientDto>();
            CreateMap<CreateUpdatePatientDto, Patient>();
            CreateMap<PatientDto, CreateUpdatePatientDto>();
            CreateMap<Patient, LookupDto<Guid>>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Responsible, ResponsibleDto>();
            CreateMap<CreateUpdateResponsibleDto, Responsible>();
            CreateMap<ResponsibleDto, CreateUpdateResponsibleDto>();

            CreateMap<Tratamento, TratamentoDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.TherapistName, opt => opt.MapFrom(src => src.Therapist.Name));
            CreateMap<CreateUpdateTratamentoDto, Tratamento>();
            CreateMap<Tratamento, CreateUpdateTratamentoDto>();
            CreateMap<TratamentoDto, CreateUpdateTratamentoDto>();

            CreateMap<Therapist, TherapistDto>();
            CreateMap<CreateUpdateTherapistDto, Therapist>();
            CreateMap<Therapist, LookupDto<Guid>>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Consultation, ConsultationDto>()
               .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.Name))
               .ForMember(d => d.TherapistName, o => o.MapFrom(s => s.Therapist.Name));
            CreateMap<CreateUpdateConsultationDto, Consultation>();

            CreateMap<PerformedTraining, PerformedTrainingDto>();
            CreateMap<CreateUpdatePerformedTrainingDto, PerformedTraining>();
            CreateMap<Consultation, ConsultationInGroupDto>()
                .ForMember(dest => dest.TherapistName, opt => opt.MapFrom(src => src.Therapist.Name))
                .ForMember(dest => dest.TherapistSpecialization, opt => opt.MapFrom(src =>
                    src.Therapist != null ? src.Therapist.Specialization : string.Empty
                ));
            CreateMap<Training, TrainingDto>();
            CreateMap<CreateUpdateTrainingDto, Training>();

            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.TrainingName, opt => opt.MapFrom(src => src.Training.Name));
            CreateMap<ActivityDto, CreateUpdateActivityDto>();
            CreateMap<CreateUpdateActivityDto, Activity>();

            CreateMap<Objective, ObjectiveDto>();
            CreateMap<CreateUpdateObjectiveDto, Objective>();

            CreateMap<CreateUpdatePerformedTrainingDto, PerformedTraining>();
            CreateMap<PerformedTraining, PerformedTrainingDto>()
                .ForMember(dest => dest.TrainingName, opt => opt.MapFrom(src => src.Training.Name));

            // Duplicates removed
            
            // Adicionados
            CreateMap<Clinic, ClinicDto>();
            CreateMap<Clinic, ClinicSettingsDto>();
            CreateMap<CreateUpdateClinicDto, Clinic>();
            CreateMap<ClinicOperatingHour, ClinicOperatingHourDto>();
            CreateMap<ClinicOperatingHourDto, ClinicOperatingHour>();
            
            CreateMap<Guest, GuestDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name));
            CreateMap<CreateUpdateGuestDto, Guest>();

            CreateMap<Attendance, AttendanceDto>();
            CreateMap<CreateUpdateAttendanceDto, Attendance>();
            CreateMap<AttendanceDto, CreateUpdateAttendanceDto>();

            CreateMap<PeriodicReport, PeriodicReportDto>();
            CreateMap<CreateUpdatePeriodicReportDto, PeriodicReport>();
            CreateMap<PeriodicReportDto, CreateUpdatePeriodicReportDto>();

            // Cross Tenant Access Maps
            CreateMap<CrossTenantAccessConsent, CrossTenantAccessConsentDto>()
                .ForMember(dest => dest.RawToken, opt => opt.Ignore());
            CreateMap<CreateCrossTenantAccessConsentDto, CrossTenantAccessConsent>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.SourceTenantId, opt => opt.Ignore())
                .ForMember(dest => dest.ExpirationDate, opt => opt.Ignore())
                .ForMember(dest => dest.AuthTokenHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsRevoked, opt => opt.Ignore());
            CreateMap<SharedAccessAuditLog, SharedAccessAuditLogDto>();

            // Billing Maps
            CreateMap<TussProcedureMapping, TussProcedureMappingDto>();
            CreateMap<BillingGuide, BillingGuideDto>();
            CreateMap<CreateBillingGuideDto, BillingGuide>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<BillingBatch, BillingBatchDto>();

            // Gamification Maps
            CreateMap<CaregiverQuest, CaregiverQuestDto>();
            CreateMap<QuestExecutionLog, QuestExecutionLogDto>();
            CreateMap<CreateQuestExecutionLogDto, QuestExecutionLog>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.ExecutionDate, opt => opt.Ignore());
            CreateMap<UserGamifiedProfile, UserGamifiedProfileDto>();
        }
    }
}