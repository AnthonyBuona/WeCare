using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Consultations;
using WeCare.PeriodicReports;
using WeCare.CrossTenantAccess;
using WeCare.Responsibles;
using WeCare.Patients;

namespace WeCare.Notifications
{
    [Authorize]
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Consultation, Guid> _consultationRepository;
        private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;
        private readonly IRepository<CrossTenantAccessConsent, Guid> _consentRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<WeCare.Therapists.Therapist, Guid> _therapistRepository;

        public NotificationAppService(
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<PeriodicReport, Guid> periodicReportRepository,
            IRepository<CrossTenantAccessConsent, Guid> consentRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<WeCare.Therapists.Therapist, Guid> therapistRepository)
        {
            _consultationRepository = consultationRepository;
            _periodicReportRepository = periodicReportRepository;
            _consentRepository = consentRepository;
            _responsibleRepository = responsibleRepository;
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
        }

        public async Task<List<NotificationDto>> GetUnreadAsync()
        {
            var notifications = new List<NotificationDto>();
            var userId = CurrentUser.Id ?? Guid.Empty;
            if (userId == Guid.Empty) return notifications;

            var now = DateTime.Now;
            var tomorrow = now.Date.AddDays(2); // up to end of tomorrow

            // Check if user is Responsible
            if (CurrentUser.IsInRole("Responsible") || CurrentUser.IsInRole("responsible"))
            {
                var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == userId);
                if (responsible != null)
                {
                    var patients = await _patientRepository.GetListAsync(p => p.PrincipalResponsibleId == responsible.Id);
                    if (patients.Any())
                    {
                        var patientIds = patients.Select(p => p.Id).ToList();

                        // 1. Upcoming consultations today/tomorrow
                        var consultations = await _consultationRepository.GetListAsync(c => 
                            patientIds.Contains(c.PatientId) && 
                            c.DateTime >= now.Date && c.DateTime < tomorrow &&
                            c.Status == ConsultationStatus.Agendada);

                        foreach (var c in consultations)
                        {
                            var patientName = patients.FirstOrDefault(p => p.Id == c.PatientId)?.Name ?? "seu filho";
                            notifications.Add(new NotificationDto
                            {
                                Id = $"consultation-{c.Id}",
                                Type = "consultation",
                                Title = "Consulta Próxima",
                                Message = $"Consulta agendada para {patientName} em {c.DateTime.ToString("dd/MM 'às' HH:mm")}.",
                                ActionUrl = "/Calendar",
                                CreatedAt = c.CreationTime,
                                IsRead = false
                            });
                        }

                        // 2. Periodic Reports Published but not signed
                        var reports = await _periodicReportRepository.GetListAsync(r => 
                            patientIds.Contains(r.PatientId) && 
                            r.Status == PeriodicReportStatus.Published);

                        foreach (var r in reports)
                        {
                            var patientName = patients.FirstOrDefault(p => p.Id == r.PatientId)?.Name ?? "seu filho";
                            notifications.Add(new NotificationDto
                            {
                                Id = $"report-{r.Id}",
                                Type = "report",
                                Title = "Relatório Pendente",
                                Message = $"Novo Relatório Bimestral para {patientName} aguardando sua assinatura digital.",
                                ActionUrl = "/",
                                CreatedAt = r.CreationTime,
                                IsRead = false
                            });
                        }

                        // 3. Consents expiring in 3 days
                        var expiringDate = now.AddDays(3);
                        var consents = await _consentRepository.GetListAsync(c => 
                            patientIds.Contains(c.PatientId) && 
                            c.ExpirationDate > now && c.ExpirationDate <= expiringDate &&
                            !c.IsRevoked);

                        foreach (var c in consents)
                        {
                            var patientName = patients.FirstOrDefault(p => p.Id == c.PatientId)?.Name ?? "seu filho";
                            notifications.Add(new NotificationDto
                            {
                                Id = $"consent-{c.Id}",
                                Type = "consent",
                                Title = "Consentimento Expirando",
                                Message = $"O consentimento de prontuário compartilhado de {patientName} expira em breve ({c.ExpirationDate.ToString("dd/MM")}).",
                                ActionUrl = "/CrossTenantAccess",
                                CreatedAt = c.CreationTime,
                                IsRead = false
                            });
                        }
                    }
                }
            }
            else // Therapist or Admin
            {
                // 1. Upcoming consultations for this therapist today
                var consultations = await _consultationRepository.GetListAsync(c => 
                    c.DateTime >= now.Date && c.DateTime < tomorrow &&
                    c.Status == ConsultationStatus.Agendada);

                // Filter by therapist if user is therapist
                var therapist = await _therapistRepository.FirstOrDefaultAsync(t => t.UserId == userId);
                if (therapist != null)
                {
                    consultations = consultations.Where(c => c.TherapistId == therapist.Id).ToList();
                }

                foreach (var c in consultations)
                {
                    var patientName = "Paciente";
                    try
                    {
                        var patient = await _patientRepository.FirstOrDefaultAsync(p => p.Id == c.PatientId);
                        if (patient != null) patientName = patient.Name;
                    }
                    catch { }

                    notifications.Add(new NotificationDto
                    {
                        Id = $"consultation-{c.Id}",
                        Type = "consultation",
                        Title = "Próxima Sessão",
                        Message = $"Sessão agendada com {patientName} em {c.DateTime.ToString("dd/MM 'às' HH:mm")}.",
                        ActionUrl = "/Calendar",
                        CreatedAt = c.CreationTime,
                        IsRead = false
                    });
                }

                // 2. Published reports awaiting parent signatures (for therapists to follow up)
                var reports = await _periodicReportRepository.GetListAsync(r => r.Status == PeriodicReportStatus.Published);
                if (therapist != null)
                {
                    reports = reports.Where(r => r.TherapistId == therapist.Id).ToList();
                }

                foreach (var r in reports)
                {
                    var patientName = "Paciente";
                    try
                    {
                        var patient = await _patientRepository.FirstOrDefaultAsync(p => p.Id == r.PatientId);
                        if (patient != null) patientName = patient.Name;
                    }
                    catch { }

                    notifications.Add(new NotificationDto
                    {
                        Id = $"report-{r.Id}",
                        Type = "report",
                        Title = "Relatório Publicado",
                        Message = $"Relatório de {patientName} enviado à família aguardando ciente formal.",
                        ActionUrl = "/PeriodicReports",
                        CreatedAt = r.CreationTime,
                        IsRead = false
                    });
                }
            }

            return notifications.OrderByDescending(n => n.CreatedAt).ToList();
        }

        public Task MarkAsReadAsync(string notificationId)
        {
            // Transient read states are elegantly handled in browser localStorage to optimize multi-tenant DB performance
            return Task.CompletedTask;
        }
    }
}
