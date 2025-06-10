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
using WeCare.Patients; // Importe os namespaces necessários
using WeCare.Therapists;
using WeCare.Consultas;

// Mudar o namespace para Tratamentos é uma boa prática
namespace WeCare.Tratamentos
{
    // Defina a permissão correta
    [Authorize(WeCarePermissions.Tratamentos.Default)]
    public class TratamentoAppService : CrudAppService<
            Tratamento,               // A entidade
            TratamentoDto,            // O DTO para exibir dados
            Guid,                     // Chave primária
            PagedAndSortedResultRequestDto, // DTO para paginação
            CreateUpdateTratamentoDto>,     // DTO para criar/atualizar
        ITratamentoAppService
    {
        // Injetar os repositórios de Patient e Therapist
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;

        public TratamentoAppService(
            IRepository<Tratamento, Guid> repository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository)
            : base(repository)
        {
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
        }

        // Sobrescrevemos o método GetAsync para carregar os detalhes (nomes)
        public override async Task<TratamentoDto> GetAsync(Guid id)
        {
            // Pede ao repositório para incluir os detalhes de Patient e Therapist na consulta
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);
            var query = queryable.Where(x => x.Id == id);
            var tratamento = await AsyncExecuter.FirstOrDefaultAsync(query);

            return ObjectMapper.Map<Tratamento, TratamentoDto>(tratamento);
        }

        // Sobrescrevemos o método GetListAsync para carregar os detalhes e corrigir a ordenação
        public override async Task<PagedResultDto<TratamentoDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // Pede ao repositório para incluir os detalhes de Patient e Therapist na consulta
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);

            var query = queryable
                // Corrigido de "TipoConsulta" para "Tipo"
                .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Tipo" : input.Sorting)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var tratamentos = await AsyncExecuter.ToListAsync(query);
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            return new PagedResultDto<TratamentoDto>(
                totalCount,
                ObjectMapper.Map<List<Tratamento>, List<TratamentoDto>>(tratamentos)
            );
        }

        // Os métodos Create, Update e Delete herdados de CrudAppService já funcionam como esperado
        // e não precisam ser sobrescritos para a lógica básica.
    }
}