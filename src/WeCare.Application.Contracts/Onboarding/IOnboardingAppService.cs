using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace WeCare.Onboarding
{
    public class OnboardingStatusDto
    {
        public bool NeedsOnboarding { get; set; }
        public string ClinicName { get; set; }
        public string ContactEmail { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
    }

    public class CompleteOnboardingInputDto
    {
        // Clinic Info
        public string ClinicName { get; set; }
        public string ContactEmail { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }

        // Therapist Info
        public string TherapistName { get; set; }
        public string TherapistEmail { get; set; }
        public string TherapistSpecialization { get; set; }

        // Patient & Responsible Info
        public string ResponsibleName { get; set; }
        public string ResponsibleCpf { get; set; }
        public string ResponsibleEmail { get; set; }
        public string PatientName { get; set; }
        public DateTime PatientBirthDate { get; set; }
    }

    public interface IOnboardingAppService : IApplicationService
    {
        Task<OnboardingStatusDto> GetStatusAsync();
        Task CompleteOnboardingAsync(CompleteOnboardingInputDto input);
    }
}
