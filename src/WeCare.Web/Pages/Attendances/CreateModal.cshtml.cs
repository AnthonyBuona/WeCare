using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Attendances;

namespace WeCare.Web.Pages.Attendances
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateAttendanceDto Attendance { get; set; }

        public List<SelectListItem> PatientList { get; set; }

        private readonly IAttendanceAppService _attendanceAppService;

        public CreateModalModel(IAttendanceAppService attendanceAppService)
        {
            _attendanceAppService = attendanceAppService;
        }

        public async Task OnGetAsync()
        {
            Attendance = new CreateUpdateAttendanceDto
            {
                SessionDate = DateTime.Now,
                Status = AttendanceStatus.Present
            };

            var patientLookup = await _attendanceAppService.GetPatientLookupAsync();
            PatientList = patientLookup
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _attendanceAppService.CreateAsync(Attendance);
            return NoContent();
        }
    }
}
