using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Consultas;

public interface ITratamentoAppService :

    ICrudAppService< //Defines CRUD methods
        TratamentoDto, //Used to show books
        Guid, //Primary key 
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateTratamentoDto> //Used to create/update a bookICrudAppService< //Defines CRUD methods

{



}