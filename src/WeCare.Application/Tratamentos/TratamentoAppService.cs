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
using WeCare.Tratamentos;

namespace WeCare.Tratamentos
{
    [Authorize(WeCarePermissions.Tratamentos.Default)]
    public class TratamentoAppService : CrudAppService<
            Tratamento,
            TratamentoDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateTratamentoDto>,
        ITratamentoAppService
    {
        public TratamentoAppService(IRepository<Tratamento, Guid> repository) : base(repository)
        {
            // Permissões padrão
            GetPolicyName = WeCarePermissions.Tratamentos.Default;
            GetListPolicyName = WeCarePermissions.Tratamentos.Default;
            CreatePolicyName = WeCarePermissions.Tratamentos.Create;
            UpdatePolicyName = WeCarePermissions.Tratamentos.Edit;
            DeletePolicyName = WeCarePermissions.Tratamentos.Delete;
        }

        public override async Task<TratamentoDto> GetAsync(Guid id)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);
            var query = queryable.Where(x => x.Id == id);
            var tratamento = await AsyncExecuter.FirstOrDefaultAsync(query);

            return ObjectMapper.Map<Tratamento, TratamentoDto>(tratamento);
        }

        public override async Task<PagedResultDto<TratamentoDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // 1. Obtém a consulta com os relacionamentos incluídos
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);

            // 2. Aplica a ordenação (agora corrigida pelo método ApplySorting)
            queryable = ApplySorting(queryable, input);

            // 3. Aplica a paginação
            queryable = ApplyPaging(queryable, input);

            // 4. Executa a consulta
            var tratamentos = await AsyncExecuter.ToListAsync(queryable);
            var totalCount = await Repository.GetCountAsync();

            return new PagedResultDto<TratamentoDto>(
                totalCount,
                ObjectMapper.Map<List<Tratamento>, List<TratamentoDto>>(tratamentos)
            );
        }

        // CORREÇÃO PRINCIPAL: Traduz os campos de ordenação do DTO para a Entidade
        protected override IQueryable<Tratamento> ApplySorting(IQueryable<Tratamento> query, PagedAndSortedResultRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                // Ordenação padrão
                return query.OrderBy(t => t.CreationTime);
            }

            // "Traduz" os nomes dos campos
            var sorting = input.Sorting
                .Replace("patientName", "Patient.Name", StringComparison.OrdinalIgnoreCase)
                .Replace("therapistName", "Therapist.Name", StringComparison.OrdinalIgnoreCase);

            return query.OrderBy(sorting);
        }
        public async Task<PagedResultDto<TratamentoDto>> GetListByPatient(Guid patientId, PagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);

            queryable = queryable.Where(x => x.PatientId == patientId);

            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);

            var tratamentos = await AsyncExecuter.ToListAsync(queryable);
            var totalCount = await Repository.CountAsync(x => x.PatientId == patientId);

            return new PagedResultDto<TratamentoDto>(
                totalCount,
                ObjectMapper.Map<List<Tratamento>, List<TratamentoDto>>(tratamentos)
            );
        }
    }
}