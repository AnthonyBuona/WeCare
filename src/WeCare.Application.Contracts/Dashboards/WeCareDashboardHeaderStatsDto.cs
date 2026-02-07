using System;
using Volo.Abp.Application.Dtos;

namespace WeCare.Dashboards
{
    public class WeCareDashboardHeaderStatsDto
    {
        public int TotalPatients { get; set; }
        public int TotalTherapists { get; set; }
        public int TotalClinics { get; set; }
        // For Therapist/Responsible specific views
        public int MyPatients { get; set; }
        public int MyAppointments { get; set; }
    }
}
