using AutoMapper;
using WeCare.Books;
using WeCare.Consultas;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Tratamentos;

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
        CreateMap<Tratamento, TratamentoDto>();
        CreateMap<CreateUpdateTratamentoDto, Tratamento>();
        CreateMap<Tratamento, CreateUpdateTratamentoDto>();
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}
