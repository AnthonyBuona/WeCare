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
            CreateMap<CreateUpdateClinicDto, Clinic>();
            
            CreateMap<Guest, GuestDto>();
        }
    }
}