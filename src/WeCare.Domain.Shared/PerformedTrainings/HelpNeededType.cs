using System.ComponentModel.DataAnnotations;

namespace WeCare.Domain.Shared.PerformedTrainings
{
    public enum HelpNeededType
    {
        [Display(Name = "Independente")]
        I,

        [Display(Name = "Suporte Verbal")]
        SV,

        [Display(Name = "Suporte Gestual")]
        SG,

        [Display(Name = "Suporte Posicional")]
        SP,

        [Display(Name = "Suporte Total")]
        ST
    }
}