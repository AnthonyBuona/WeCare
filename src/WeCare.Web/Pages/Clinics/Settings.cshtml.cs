using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using WeCare.Clinics;

namespace WeCare.Web.Pages.Clinics
{
    public class SettingsModel : AbpPageModel
    {
        [BindProperty]
        public ClinicSettingsDto ClinicSettings { get; set; }

        private readonly IClinicAppService _clinicAppService;

        public SettingsModel(IClinicAppService clinicAppService)
        {
            _clinicAppService = clinicAppService;
        }

        public async Task OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                var settings = await _clinicAppService.GetCurrentClinicSettingsAsync();
                ClinicSettings = settings ?? new ClinicSettingsDto();
            }
            else
            {
                ClinicSettings = await _clinicAppService.GetSettingsAsync(id.Value);
            }

            // Ensure defaults for color pickers if not set
            if (string.IsNullOrEmpty(ClinicSettings.PrimaryColor))
            {
                ClinicSettings.PrimaryColor = "#5e72e4";
            }
            if (string.IsNullOrEmpty(ClinicSettings.SecondaryColor))
            {
                ClinicSettings.SecondaryColor = "#2dce89";
            }

            EnsureOperatingHours();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                await _clinicAppService.UpdateCurrentClinicSettingsAsync(ClinicSettings);
            }
            else
            {
                await _clinicAppService.UpdateSettingsAsync(id.Value, ClinicSettings);
            }

            return NoContent();
        }

        private void EnsureOperatingHours()
        {
            if (ClinicSettings.OperatingHours == null)
                ClinicSettings.OperatingHours = new List<ClinicOperatingHourDto>();

            var existingDays = new HashSet<DayOfWeek>(
                ClinicSettings.OperatingHours.Select(x => x.DayOfWeek));

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (!existingDays.Contains(day))
                {
                    ClinicSettings.OperatingHours.Add(new ClinicOperatingHourDto
                    {
                        DayOfWeek = day,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        IsClosed = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday
                    });
                }
            }

            ClinicSettings.OperatingHours = ClinicSettings.OperatingHours
                .OrderBy(x => x.DayOfWeek)
                .ToList();
        }
    }
}
