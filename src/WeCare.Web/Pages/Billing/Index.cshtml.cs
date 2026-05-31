using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using WeCare.Billing;

namespace WeCare.Web.Pages.Billing
{
    public class IndexModel : WeCarePageModel
    {
        private readonly IBillingAppService _billingAppService;
        private readonly IRepository<BillingGuide, Guid> _billingGuideRepository;

        public IndexModel(
            IBillingAppService billingAppService,
            IRepository<BillingGuide, Guid> billingGuideRepository)
        {
            _billingAppService = billingAppService;
            _billingGuideRepository = billingGuideRepository;
        }

        public List<BillingGuideDto> Guides { get; set; } = new List<BillingGuideDto>();
        public List<string> Operators { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string SelectedOperator { get; set; }

        // Dashboard Metrics
        public decimal TotalToReceive { get; set; }
        public int PendingGuidesCount { get; set; }
        public decimal DisputedClaimsValue { get; set; } = 450.00m; // Beautiful simulated Glosas metric

        [BindProperty]
        public List<Guid> SelectedGuideIds { get; set; } = new List<Guid>();

        [BindProperty]
        public IFormFile PfxCertificate { get; set; }

        [BindProperty]
        public string PfxPassword { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            if (SelectedGuideIds == null || !SelectedGuideIds.Any())
            {
                Alerts.Danger("Por favor, selecione pelo menos uma guia para exportar.");
                await LoadDataAsync();
                return Page();
            }

            string base64Pfx = null;
            if (PfxCertificate != null && PfxCertificate.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    await PfxCertificate.CopyToAsync(ms);
                    base64Pfx = Convert.ToBase64String(ms.ToArray());
                }
            }

            try
            {
                var batch = await _billingAppService.GenerateBillingBatchAsync(
                    SelectedGuideIds,
                    base64Pfx,
                    PfxPassword
                );

                Alerts.Success($"Lote {batch.BatchNumber} exportado e assinado digitalmente com sucesso!");

                // Return the compliant XML file as download
                var xmlBytes = Encoding.UTF8.GetBytes(batch.XmlPayload);
                return File(xmlBytes, "application/xml", $"{batch.BatchNumber}.xml");
            }
            catch (Exception ex)
            {
                Alerts.Danger($"Erro na exportação de guias: {ex.Message}");
                await LoadDataAsync();
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            var allGuides = await _billingGuideRepository.GetListAsync();

            // Unique Operators for filtering
            Operators = allGuides.Select(g => g.HealthInsuranceName).Distinct().ToList();

            // Calculate Metrics
            TotalToReceive = allGuides.Where(g => g.Status == "Pending" || g.Status == "Exported").Sum(g => g.ConsultationValue);
            PendingGuidesCount = allGuides.Count(g => g.Status == "Pending");

            // Filter logic
            var filtered = allGuides;
            if (!string.IsNullOrEmpty(SelectedOperator))
            {
                filtered = allGuides.Where(g => g.HealthInsuranceName.Equals(SelectedOperator, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            Guides = filtered.Select(g => new BillingGuideDto
            {
                Id = g.Id,
                ConsultationId = g.ConsultationId,
                HealthInsuranceName = g.HealthInsuranceName,
                CardNumber = g.CardNumber,
                AuthorizationPassword = g.AuthorizationPassword,
                ConsultationValue = g.ConsultationValue,
                Status = g.Status
            }).ToList();
        }
    }
}
