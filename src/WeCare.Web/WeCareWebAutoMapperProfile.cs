using AutoMapper;
using WeCare.Books;

namespace WeCare.Web;

public class WeCareWebAutoMapperProfile : Profile
{
    public WeCareWebAutoMapperProfile()
    {
        CreateMap<BookDto, CreateUpdateBookDto>();
        
        //Define your object mappings here, for the Web project
    }
}
