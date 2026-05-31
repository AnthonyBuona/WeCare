using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TenantManagement;
using WeCare.CrossTenantAccess;
using WeCare.Patients;
using WeCare.Responsibles;

namespace WeCare.Web.Pages.CrossTenantAccess
{
    public class IndexModel : WeCarePageModel
    {
        private readonly IRepository<CrossTenantAccessConsent, Guid> _consentRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly ITenantRepository _tenantRepository;

        public IndexModel(
            IRepository<CrossTenantAccessConsent, Guid> consentRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            ITenantRepository tenantRepository)
        {
            _consentRepository = consentRepository;
            _patientRepository = patientRepository;
            _responsibleRepository = responsibleRepository;
            _tenantRepository = tenantRepository;
        }

        public List<ConsentViewModel> Consents { get; set; } = new List<ConsentViewModel>();
        public List<SelectListItem> PatientOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ClinicOptions { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public Guid SelectedPatientId { get; set; }

        [BindProperty]
        public string TargetClinicInput { get; set; } // Either Tenant ID or name

        [BindProperty]
        public int ExpirationDays { get; set; } = 30;

        [TempData]
        public string GeneratedRawToken { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostGenerateTokenAsync()
        {
            if (SelectedPatientId == Guid.Empty)
            {
                Alerts.Danger("Por favor, selecione um paciente.");
                await LoadDataAsync();
                return Page();
            }

            Guid targetTenantId;
            if (!Guid.TryParse(TargetClinicInput, out targetTenantId))
            {
                // Try to find a tenant with this ID or name, otherwise create a random GUID as mock clinic
                var tenants = await _tenantRepository.GetListAsync();
                var matchingTenant = tenants.FirstOrDefault(t => t.Name.Equals(TargetClinicInput, StringComparison.OrdinalIgnoreCase));
                if (matchingTenant != null)
                {
                    targetTenantId = matchingTenant.Id;
                }
                else
                {
                    targetTenantId = Guid.NewGuid(); // Fallback random clinic
                }
            }

            // Generate raw token
            string rawToken = "wctk_" + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            string tokenHash = ComputeSha256Hash(rawToken);
            var expirationDate = Clock.Now.AddDays(ExpirationDays);

            var consent = new CrossTenantAccessConsent(
                GuidGenerator.Create(),
                SelectedPatientId,
                CurrentTenant.Id ?? Guid.Empty,
                targetTenantId,
                expirationDate,
                "Read",
                tokenHash,
                isRevoked: false,
                tenantId: CurrentTenant.Id
            );

            await _consentRepository.InsertAsync(consent, autoSave: true);

            GeneratedRawToken = rawToken;
            Alerts.Success("Token de compartilhamento gerado com sucesso! Copie-o antes de sair.");

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRevokeAsync(Guid id)
        {
            var consent = await _consentRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (consent != null)
            {
                consent.IsRevoked = true;
                await _consentRepository.UpdateAsync(consent, autoSave: true);
                Alerts.Success("Acesso revogado com sucesso!");
            }
            else
            {
                Alerts.Danger("Consentimento não encontrado.");
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            // Get current logged-in caregiver's responsible profile
            var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
            List<Guid> patientIds;
            List<Patient> myPatients;

            if (responsible != null)
            {
                myPatients = await _patientRepository.GetListAsync(x => x.PrincipalResponsibleId == responsible.Id);
                patientIds = myPatients.Select(x => x.Id).ToList();
            }
            else
            {
                // Fallback: list all patients in current tenant
                myPatients = await _patientRepository.GetListAsync();
                patientIds = myPatients.Select(x => x.Id).ToList();
            }

            PatientOptions = myPatients.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            // Load tenants for clinic selection dropdown
            var allTenants = await _tenantRepository.GetListAsync();
            var currentTenantId = CurrentTenant.Id;
            var otherTenants = allTenants.Where(t => t.Id != currentTenantId).ToList();

            ClinicOptions = otherTenants.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            // Load consents
            var consents = await _consentRepository.GetListAsync(x => patientIds.Contains(x.PatientId));
            
            Consents = new List<ConsentViewModel>();
            foreach (var c in consents)
            {
                var patient = myPatients.FirstOrDefault(p => p.Id == c.PatientId);
                var targetTenant = allTenants.FirstOrDefault(t => t.Id == c.TargetTenantId);
                
                string targetClinicName = targetTenant?.Name ?? "Clínica Crescer (Externa)";
                string statusStr = "Ativo";
                string badgeClass = "badge bg-success";

                if (c.IsRevoked)
                {
                    statusStr = "Revogado";
                    badgeClass = "badge bg-danger";
                }
                else if (c.ExpirationDate < Clock.Now)
                {
                    statusStr = "Expirado";
                    badgeClass = "badge bg-secondary";
                }

                Consents.Add(new ConsentViewModel
                {
                    Id = c.Id,
                    PatientName = patient?.Name ?? "Paciente",
                    TargetClinicName = targetClinicName,
                    ExpirationDate = c.ExpirationDate,
                    Status = statusStr,
                    BadgeClass = badgeClass,
                    IsRevoked = c.IsRevoked
                });
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

    public class ConsentViewModel
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; }
        public string TargetClinicName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
        public string BadgeClass { get; set; }
        public bool IsRevoked { get; set; }
    }
}
