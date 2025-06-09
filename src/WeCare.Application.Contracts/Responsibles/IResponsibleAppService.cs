using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Responsibles;

public interface IResponsibleAppService :

    ICrudAppService< //Defines CRUD methods
        ResponsibleDto, //Used to show books
        Guid, //Primary key 
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateResponsibleDto> //Used to create/update a bookICrudAppService< //Defines CRUD methods

{



}