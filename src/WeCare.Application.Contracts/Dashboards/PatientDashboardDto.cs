using System;
using System.Collections.Generic;

namespace WeCare.Dashboards
{
    public class PatientDashboardDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        public int TotalConsultations { get; set; }
        public int TotalObjectives { get; set; }
        public int ActiveObjectives { get; set; }
        public int CompletedObjectives { get; set; }

        public List<PerformanceDataPointDto> PerformanceHistory { get; set; } = new();
        public List<ObjectiveStatusDto> ObjectivesList { get; set; } = new();
        
        // New detailed sections
        public List<ConsultationDetailDto> RecentConsultations { get; set; } = new();
        public List<TrainingPerformanceDto> TrainingStats { get; set; } = new();
    }

    public class PerformanceDataPointDto
    {
        public DateTime Date { get; set; }
        public string Label { get; set; } = string.Empty;
        public double SuccessRate { get; set; } // 0 to 100
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
    }

    public class ObjectiveStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int ConsultationCount { get; set; }
    }

    public class ConsultationDetailDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public string ObjectiveName { get; set; } = string.Empty; // New field
        public string Description { get; set; } = string.Empty;
        public List<PerformedTrainingDto> PerformedTrainings { get; set; } = new();
    }

    public class PerformedTrainingDto
    {
        public string TrainingName { get; set; } = string.Empty;
        public string HelpNeeded { get; set; } = string.Empty;
        public int Attempts { get; set; }
        public int Successes { get; set; }
        public double SuccessRate => Attempts > 0 ? Math.Round(((double)Successes / Attempts) * 100, 1) : 0;
    }

    public class TrainingPerformanceDto
    {
        public string TrainingName { get; set; } = string.Empty;
        public int TotalExecutions { get; set; }
        public double AverageSuccessRate { get; set; }
        public int TotalAttempts { get; set; }
         public int TotalSuccesses { get; set; }
    }
}
