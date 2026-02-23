using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;

namespace WeCare.Therapists
{
    // O Handler implementa as interfaces para ouvir eventos de criação e atualização de usuários.
    public class TherapistCreationHandler
        : ILocalEventHandler<EntityCreatedEventData<IdentityUser>>,
          ILocalEventHandler<EntityUpdatedEventData<IdentityUser>>,
          ITransientDependency
    {
        private readonly IRepository<Therapist, Guid> _therapistRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IIdentityRoleRepository _roleRepository;

        public TherapistCreationHandler(
            IRepository<Therapist, Guid> therapistRepository,
            IIdentityUserRepository userRepository,
            IIdentityRoleRepository roleRepository)
        {
            _therapistRepository = therapistRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // Método chamado quando um usuário é CRIADO
        public async Task HandleEventAsync(EntityCreatedEventData<IdentityUser> eventData)
        {
            // Desativado para evitar duplicação com o TherapistAppService que agora cria explicitamente
            // await CreateTherapistIfNeededAsync(eventData.Entity);
        }

        // Método chamado quando um usuário é ATUALIZADO
        public async Task HandleEventAsync(EntityUpdatedEventData<IdentityUser> eventData)
        {
            await CreateTherapistIfNeededAsync(eventData.Entity);
        }

        /// <summary>
        /// Lógica central que verifica a role e cria o terapeuta se necessário.
        /// </summary>
        private async Task CreateTherapistIfNeededAsync(IdentityUser user)
        {
            const string therapistRoleName = "Therapist";

            // 1. Verifica se o usuário tem a role "Terapeuta"
            var roles = await _userRepository.GetRolesAsync(user.Id);
            if (!roles.Any(role => role.Name.Equals(therapistRoleName, StringComparison.OrdinalIgnoreCase)))
            {
                // Se o usuário não tem a role, não há nada a fazer.
                return;
            }

            // 2. Verifica se um terapeuta já não existe para este usuário, evitando duplicatas.
            if (await _therapistRepository.AnyAsync(t => t.UserId == user.Id))
            {
                // Terapeuta já existe, trabalho concluído.
                return;
            }

            // 3. Cria a nova entidade Therapist com os dados do usuário.
            var newTherapist = new Therapist
            {
                UserId = user.Id,
                Name = user.Name, // Usamos o Nome do usuário como padrão para o Nome do terapeuta.
                Email = user.Email
            };

            await _therapistRepository.InsertAsync(newTherapist, autoSave: true);
        }
    }
}