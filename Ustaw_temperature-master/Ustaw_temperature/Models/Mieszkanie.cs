using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ustaw_temperature.Models
{
    public class Mieszkanie
    {
        public int Id { get; set; }
        [Range(0, 5)] public int LiczbaOkien { get; set; }
        [Range(1, 5)] public int LiczbaPokoi { get; set; }
        
        [Range(10,17)] public int BazowaTemperatura { get; set; }
        public string? Uzytkownik { get; set; }
        public IdentityUser? User { get; set; }
        public string? Uzytkownik2 { get; set; }
    }
}
