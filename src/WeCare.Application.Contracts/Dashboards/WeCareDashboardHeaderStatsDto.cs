using System;
using System.Collections.Generic;

namespace WeCare.Dashboards
{
    public class WeCareDashboardHeaderStatsDto
    {
        public int TotalPatients { get; set; }
        public int TotalTherapists { get; set; }
        public int TotalClinics { get; set; }

        // Real KPIs
        public int ConsultationsToday { get; set; }
        public int ConsultationsThisMonth { get; set; }
        public int ActivePatientsThisMonth { get; set; }
        public int PendingReports { get; set; }

        // Upcoming sessions list (today, next 3)
        public List<UpcomingConsultationSummaryDto> UpcomingConsultationsToday { get; set; } = new();

        // For Therapist/Responsible specific views
        public int MyPatients { get; set; }
        public int MyAppointments { get; set; }

        // Monthly Attendance comparison stats
        public List<MonthlyAttendanceStatDto> MonthlyAttendanceStats { get; set; } = new();
    }

    public class UpcomingConsultationSummaryDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string TherapistName { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }

    public class MonthlyAttendanceStatDto
    {
        public string MonthName { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int CancelledCount { get; set; }
        public double PresenceRate { get; set; }
    }
}
