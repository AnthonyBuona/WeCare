using System.ComponentModel.DataAnnotations;

namespace WeCare.Consultations
{
    public enum ConsultationStatus
    {
        [Display(Name = "Agendada")]
        Agendada = 0,

        [Display(Name = "Realizada")]
        Realizada = 1
    }
}
