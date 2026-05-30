using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Patients;
using WeCare.Permissions;
using WeCare.Shared;

namespace WeCare.Attendances
{
    [Authorize(WeCarePermissions.Attendances.Default)]
    public class AttendanceAppService : CrudAppService<
            Attendance,
            AttendanceDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateAttendanceDto>,
        IAttendanceAppService
    {
        private readonly IRepository<Patient, Guid> _patientRepository;

        public AttendanceAppService(
            IRepository<Attendance, Guid> repository,
            IRepository<Patient, Guid> patientRepository)
            : base(repository)
        {
            _patientRepository = patientRepository;

            GetPolicyName = WeCarePermissions.Attendances.Default;
            GetListPolicyName = WeCarePermissions.Attendances.Default;
            CreatePolicyName = WeCarePermissions.Attendances.Create;
            UpdatePolicyName = WeCarePermissions.Attendances.Edit;
            DeletePolicyName = WeCarePermissions.Attendances.Delete;
        }

        public async Task<List<LookupDto<Guid>>> GetPatientLookupAsync()
        {
            var patients = await _patientRepository.GetListAsync();
            return patients.Select(p => new LookupDto<Guid>(p.Id, p.Name)).ToList();
        }

        public override async Task<PagedResultDto<AttendanceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = "SessionDate DESC";
            }

            var queryable = await Repository.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);

            var attendances = await AsyncExecuter.ToListAsync(queryable);
            var attendanceDtos = ObjectMapper.Map<List<Attendance>, List<AttendanceDto>>(attendances);

            if (attendanceDtos.Any())
            {
                var patientIds = attendanceDtos.Select(a => a.PatientId).Distinct().ToList();
                var patients = await _patientRepository.GetListAsync(p => patientIds.Contains(p.Id));
                var patientDict = patients.ToDictionary(p => p.Id, p => p.Name);

                foreach (var dto in attendanceDtos)
                {
                    if (patientDict.TryGetValue(dto.PatientId, out var patientName))
                    {
                        dto.PatientName = patientName;
                    }
                }
            }

            return new PagedResultDto<AttendanceDto>(
                totalCount,
                attendanceDtos
            );
        }

        protected override IQueryable<Attendance> ApplySorting(IQueryable<Attendance> query, PagedAndSortedResultRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                return query.OrderByDescending(x => x.SessionDate);
            }

            var sorting = input.Sorting.Replace("patientName", "PatientId", StringComparison.OrdinalIgnoreCase);
            return query.OrderBy(sorting);
        }
    }
}
