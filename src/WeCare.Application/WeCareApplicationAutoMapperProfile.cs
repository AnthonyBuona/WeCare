using AutoMapper;
using WeCare.Books;
using WeCare.Tratamentos;
using WeCare.Patients;
using WeCare.Responsibles;
using System;
using WeCare.Shared;
using WeCare.Therapists;


namespace WeCare;

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
		CreateMap<Therapist, TherapistDto>();
		CreateMap<CreateUpdateTherapistDto, Therapist>();
		CreateMap<Therapist, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
	}

	/* You can configure your AutoMapper mapping configuration here.
	 * Alternatively, you can split your mapping configurations
	 * into multiple profile classes for a better organization. */
}

