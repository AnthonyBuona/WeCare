using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Guests;
using WeCare.Patients;
using Volo.Abp.Domain.Repositories;

namespace WeCare.Web.Pages.Guests
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateGuestDto Guest { get; set; } = new();

        public List<SelectListItem> PatientLookup { get; set; } = new();

        private readonly IGuestAppService _guestAppService;
        private readonly IRepository<Patient, Guid> _patientRepository;

        public CreateModalModel(
            IGuestAppService guestAppService,
            IRepository<Patient, Guid> patientRepository)
        {
            _guestAppService = guestAppService;
            _patientRepository = patientRepository;
        }

        public async Task OnGetAsync()
        {
            Guest = new CreateGuestDto();
            
            // Carregar pacientes do responsável logado
            // Como no AppService, precisamos filtrar os pacientes.
            // Para UI simples, vou buscar todos e filtrar no client ou assumir que o repositório faria isso (mas não faz auto magicamente aqui)
            // TODO: Criar um método no PatientAppService GetMyPatientsAsync
            
            // Fazer uma query direta por enquanto, simulando o que o AppService faria
            // (Idealmente deveria chamar um serviço de aplicação para popular o dropdown)
            
            var patientLookup = await _guestAppService.GetMyPatientsLookupAsync();
            PatientLookup = patientLookup.Items
                .Select(x => new SelectListItem(x.DisplayName, x.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _guestAppService.CreateAsync(Guest);
            return NoContent();
        }
    }
}
