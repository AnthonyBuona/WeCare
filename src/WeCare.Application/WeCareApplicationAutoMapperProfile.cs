using AutoMapper;
using WeCare.Books;
using WeCare.Tratamentos;
using WeCare.Patients;
using WeCare.Responsibles;
using System;
using WeCare.Shared;
using WeCare.Therapists;
using WeCare.Consultations;
using WeCare.Application.Contracts.Consultations; // Importação necessária

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

            // --- MAPEAMENTO CORRIGIDO E ADICIONADO ---
            // Este é o mapeamento que estava faltando e causava o erro.
            CreateMap<Consultation, ConsultationInGroupDto>()
                .ForMember(dest => dest.TherapistName, opt => opt.MapFrom(src => src.Therapist.Name))
                .ForMember(dest => dest.TherapistSpecialization, opt => opt.MapFrom(src =>
                    // Se o terapeuta ou a especialização for nula, retorna uma string vazia.
                    src.Therapist != null ? src.Therapist.Specialization : string.Empty
                ));
        }
    }
}