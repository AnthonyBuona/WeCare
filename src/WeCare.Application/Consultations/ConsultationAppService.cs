using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Permissions;
using WeCare.Patients;
using WeCare.Therapists;

namespace WeCare.Consultations
{
    [Authorize(WeCarePermissions.Consultations.Default)]
    public class ConsultationAppService : CrudAppService<
            Consultation,
            ConsultationDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateConsultationDto>,
        IConsultationAppService
    {
        public ConsultationAppService(IRepository<Consultation, Guid> repository) : base(repository)
        {
            GetPolicyName = WeCarePermissions.Consultations.Default;
            GetListPolicyName = WeCarePermissions.Consultations.Default;
            CreatePolicyName = WeCarePermissions.Consultations.Create;
            UpdatePolicyName = WeCarePermissions.Consultations.Edit;
            DeletePolicyName = WeCarePermissions.Consultations.Delete;
        }

        public override async Task<ConsultationDto> GetAsync(Guid id)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);
            var query = queryable.Where(x => x.Id == id);
            var consultation = await AsyncExecuter.FirstOrDefaultAsync(query);

            return ObjectMapper.Map<Consultation, ConsultationDto>(consultation);
        }

        public override async Task<PagedResultDto<ConsultationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);

            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);

            var consultations = await AsyncExecuter.ToListAsync(queryable);
            var totalCount = await Repository.GetCountAsync();

            return new PagedResultDto<ConsultationDto>(
                totalCount,
                ObjectMapper.Map<List<Consultation>, List<ConsultationDto>>(consultations)
            );
        }

        protected override IQueryable<Consultation> ApplySorting(IQueryable<Consultation> query, PagedAndSortedResultRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                return query.OrderBy(t => t.DateTime);
            }

            var sorting = input.Sorting
                .Replace("patientName", "Patient.Name", StringComparison.OrdinalIgnoreCase)
                .Replace("therapistName", "Therapist.Name", StringComparison.OrdinalIgnoreCase);

            return query.OrderBy(sorting);
        }
    }
}