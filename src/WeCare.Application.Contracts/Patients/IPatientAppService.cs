using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Patients;

public interface IPatientAppService :

    ICrudAppService< //Defines CRUD methods
        PatientDto, //Used to show books
        Guid, //Primary key 
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdatePatientDto> //Used to create/update a bookICrudAppService< //Defines CRUD methods

{



}