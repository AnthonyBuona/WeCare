using System;
using AutoMapper;
using WeCare.Application.Contracts.Consultations;
using WeCare.Books;
using WeCare.Web.Pages.RealizedConsultations;

namespace WeCare.Web;

public class WeCareWebAutoMapperProfile : Profile
{
    public WeCareWebAutoMapperProfile()
    {
        CreateMap<BookDto, CreateUpdateBookDto>();
        CreateMap<ConsultationInGroupDto, ConsultationItemViewModel>();

        // Mapeia um grupo de objetivo (DTO) para um card de objetivo (ViewModel).
        CreateMap<ObjectiveGroupDto, ObjectiveDisplayViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ObjectiveName))
            // Define uma lógica de progresso estável, usando a quantidade de consultas.
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => Math.Min(100, src.Consultations.Count * 10)));
        //Define your object mappings here, for the Web project
    }
}
