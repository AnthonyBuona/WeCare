using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Attendances;

namespace WeCare.Web.Pages.Attendances
{
    public class EditModalModel : WeCarePageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateAttendanceDto Attendance { get; set; }

        public List<SelectListItem> PatientList { get; set; }

        private readonly IAttendanceAppService _attendanceAppService;

        public EditModalModel(IAttendanceAppService attendanceAppService)
        {
            _attendanceAppService = attendanceAppService;
        }

        public async Task OnGetAsync()
        {
            var attendanceDto = await _attendanceAppService.GetAsync(Id);
            Attendance = ObjectMapper.Map<AttendanceDto, CreateUpdateAttendanceDto>(attendanceDto);

            var patientLookup = await _attendanceAppService.GetPatientLookupAsync();
            PatientList = patientLookup
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _attendanceAppService.UpdateAsync(Id, Attendance);
            return NoContent();
        }
    }
}
