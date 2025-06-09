using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Volo.Abp;

namespace WeCare.Consultas
{
    [Authorize] // Você pode definir uma permission específica como WeCarePermissions.ConsultaType.Default
    public class ConsultaTypeAppService : ApplicationService, IConsultaTypeAppService
    {
        private readonly IRepository<ConsultaType, Guid> _repository;

        public ConsultaTypeAppService(IRepository<ConsultaType, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<ConsultaTypeDto> GetAsync(Guid id)
        {
            var consulta = await _repository.GetAsync(id);
            return ObjectMapper.Map<ConsultaType, ConsultaTypeDto>(consulta);
        }

        public async Task<PagedResultDto<ConsultaTypeDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await _repository.GetQueryableAsync();

            var query = queryable
                .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "TipoConsulta" : input.Sorting)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var consultas = await AsyncExecuter.ToListAsync(query);
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            return new PagedResultDto<ConsultaTypeDto>(
                totalCount,
                ObjectMapper.Map<List<ConsultaType>, List<ConsultaTypeDto>>(consultas)
            );
        }

        public async Task<ConsultaTypeDto> CreateAsync(CreateUpdateConsultaTypeDto input)
        {
            var consulta = ObjectMapper.Map<CreateUpdateConsultaTypeDto, ConsultaType>(input);

            await _repository.InsertAsync(consulta);

            return ObjectMapper.Map<ConsultaType, ConsultaTypeDto>(consulta);
        }

        public async Task<ConsultaTypeDto> UpdateAsync(Guid id, CreateUpdateConsultaTypeDto input)
        {
            var consulta = await _repository.GetAsync(id);

            ObjectMapper.Map(input, consulta);

            await _repository.UpdateAsync(consulta);

            return ObjectMapper.Map<ConsultaType, ConsultaTypeDto>(consulta);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
