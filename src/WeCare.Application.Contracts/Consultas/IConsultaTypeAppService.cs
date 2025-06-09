using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Consultas;

public interface IConsultaTypeAppService :

    ICrudAppService< //Defines CRUD methods
        ConsultaTypeDto, //Used to show books
        Guid, //Primary key 
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateConsultaTypeDto> //Used to create/update a bookICrudAppService< //Defines CRUD methods

{



}