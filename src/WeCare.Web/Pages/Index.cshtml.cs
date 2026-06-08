using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WeCare.Clinics;
using WeCare.Dashboards;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Consultations;
using WeCare.PeriodicReports;
using WeCare.Subscriptions;

namespace WeCare.Web.Pages;

public class IndexModel : WeCarePageModel
{
    public WeCareDashboardHeaderStatsDto? Stats { get; set; }
    public ClinicSettingsDto? ClinicInfo { get; set; }
    
    public bool IsResponsible { get; set; }
    public Responsible? ParentProfile { get; set; }
    public List<Patient> MyPatients { get; set; } = new();
    public List<Consultation> RecentConsultations { get; set; } = new();
    public List<PeriodicReport> MyReports { get; set; } = new();
    public string ClientIp { get; set; } = "127.0.0.1";

    public WeCare.Onboarding.OnboardingStatusDto? OnboardingStatus { get; set; }

    [BindProperty]
    public WeCare.Onboarding.CompleteOnboardingInputDto OnboardingInput { get; set; } = new();

    // SaaS Subscription Properties
    public SubscriptionStatusDto? Subscription { get; set; }

    [BindProperty]
    public RegisterTrialInputDto TrialInput { get; set; } = new();

    private readonly IDashboardAppService _dashboardAppService;
    private readonly IClinicAppService _clinicAppService;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Responsible, Guid> _responsibleRepository;
    private readonly IRepository<Consultation, Guid> _consultationRepository;
    private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;
    private readonly WeCare.Onboarding.IOnboardingAppService _onboardingAppService;
    private readonly ISubscriptionAppService _subscriptionAppService;

    public IndexModel(
        IDashboardAppService dashboardAppService, 
        IClinicAppService clinicAppService,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Responsible, Guid> responsibleRepository,
        IRepository<Consultation, Guid> consultationRepository,
        IRepository<PeriodicReport, Guid> periodicReportRepository,
        WeCare.Onboarding.IOnboardingAppService onboardingAppService,
        ISubscriptionAppService subscriptionAppService)
    {
        _dashboardAppService = dashboardAppService;
        _clinicAppService = clinicAppService;
        _patientRepository = patientRepository;
        _responsibleRepository = responsibleRepository;
        _consultationRepository = consultationRepository;
        _periodicReportRepository = periodicReportRepository;
        _onboardingAppService = onboardingAppService;
        _subscriptionAppService = subscriptionAppService;
    }

    public async Task OnGetAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            Stats = await _dashboardAppService.GetStatsAsync();

            try
            {
                ClinicInfo = await _clinicAppService.GetCurrentClinicSettingsAsync();
            }
            catch (Exception)
            {
                // Clinic info is non-critical
            }

            try
            {
                Subscription = await _subscriptionAppService.GetStatusAsync();
            }
            catch (Exception)
            {
                // Subscription is non-critical
            }

            // Check if user is in "Responsible" role
            if (CurrentUser.Roles.Contains("Responsible") || CurrentUser.Roles.Contains("responsible"))
            {
                IsResponsible = true;
                ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                if (ClientIp == "::1")
                {
                    ClientIp = "127.0.0.1";
                }
                
                ParentProfile = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
                if (ParentProfile != null)
                {
                    MyPatients = await _patientRepository.GetListAsync(p => p.PrincipalResponsibleId == ParentProfile.Id);
                    if (MyPatients.Any())
                    {
                        var patientIds = MyPatients.Select(p => p.Id).ToList();
                        
                        // Query consultations for my patients
                        var consultations = await _consultationRepository.GetListAsync(c => patientIds.Contains(c.PatientId));
                        RecentConsultations = consultations.OrderByDescending(c => c.DateTime).Take(5).ToList();

                        // Query published reports for my patients
                        var reports = await _periodicReportRepository.GetListAsync(r => patientIds.Contains(r.PatientId) && r.Status != PeriodicReportStatus.Draft);
                        MyReports = reports.OrderByDescending(r => r.CreationTime).ToList();
                    }
                }
            }
            else
            {
                OnboardingStatus = await _onboardingAppService.GetStatusAsync();
                
                // Prefill form
                if (OnboardingStatus.NeedsOnboarding)
                {
                    OnboardingInput.ClinicName = OnboardingStatus.ClinicName;
                    OnboardingInput.ContactEmail = OnboardingStatus.ContactEmail;
                    OnboardingInput.PrimaryColor = OnboardingStatus.PrimaryColor;
                    OnboardingInput.SecondaryColor = OnboardingStatus.SecondaryColor;
                    OnboardingInput.PatientBirthDate = DateTime.Now.AddYears(-6); // sensible default
                }
            }
        }
    }

    public async Task<IActionResult> OnPostOnboardingAsync()
    {
        try
        {
            await _onboardingAppService.CompleteOnboardingAsync(OnboardingInput);
            Alerts.Success("Configuração inicial e onboarding concluídos com sucesso! Bem-vindo ao WeCare!");
        }
        catch (Exception ex)
        {
            Alerts.Danger($"Falha ao registrar onboarding: {ex.Message}");
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRegisterTrialAsync()
    {
        try
        {
            await _subscriptionAppService.RegisterTrialAsync(TrialInput);
            Alerts.Success("Sua clínica foi registrada com sucesso! Seu período de testes de 14 dias foi ativado. Faça login com o e-mail administrativo!");
            return RedirectToPage("/Account/Login");
        }
        catch (Exception ex)
        {
            Alerts.Danger($"Erro ao cadastrar clínica: {ex.Message}");
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostUpgradePlanAsync(string planName)
    {
        try
        {
            await _subscriptionAppService.UpdateSubscriptionPlanAsync(planName);
            Alerts.Success($"Parabéns! Assinatura ativada e clínica atualizada para o plano {planName}!");
        }
        catch (Exception ex)
        {
            Alerts.Danger($"Erro ao processar assinatura: {ex.Message}");
        }
        return RedirectToPage();
    }

    public string GetHomeGuidance(Consultation consultation, string patientName)
    {
        var training = (consultation.MainTraining ?? "").ToLower().Trim();
        if (training.Contains("fina") || training.Contains("coordenação") || training.Contains("motor"))
        {
            return $"Para consolidar o treino de \"{consultation.MainTraining}\" realizado na clínica, incentive {patientName} a praticar atividades de pinça fina (como desenhar com giz de cera, abotoar camisas ou usar massinha de modelar) por 10 minutos hoje.";
        }
        if (training.Contains("fala") || training.Contains("comunicação") || training.Contains("linguagem") || training.Contains("fono"))
        {
            return $"Para consolidar a evolução em fonoaudiologia, pratique a pronúncia cantando as músicas favoritas de {patientName} ou apontando e nomeando objetos em figuras por 10 minutos hoje.";
        }
        if (training.Contains("visual") || training.Contains("socialização") || training.Contains("contato"))
        {
            return $"Estimule o engajamento de {patientName} hoje: faça jogos rápidos de imitação facial de frente para o espelho e incentive o contato visual durante as brincadeiras.";
        }
        
        // Default dynamic fallback using session notes
        if (!string.IsNullOrEmpty(consultation.Description) && consultation.Description.Length > 10)
        {
            return $"Com base no acompanhamento desta sessão (\"{consultation.Description}\"), apoie {patientName} repetindo atividades lúdicas de acolhimento em casa e dê reforços positivos por qualquer tentativa de iniciativa própria!";
        }
        
        return $"Para maximizar a evolução de {patientName}, pratique exercícios motores leves ou o encaixe de blocos por 10 minutos hoje. Incentive o contato visual e elogie cada pequena conquista!";
    }
}
