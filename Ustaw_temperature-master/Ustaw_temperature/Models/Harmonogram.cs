using System.ComponentModel.DataAnnotations;

namespace Ustaw_temperature.Models
{
    public class Harmonogram : IValidatableObject
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        [Range(0, 23)] public int Start { get; set; }
        [Range(1, 24)] public int End { get; set; }
        [Range(18, 35)] public int DocelowaTemperatura  { get; set; }
        public int MieszkanieId { get; set; }
        public virtual Mieszkanie? Mieszkanie { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Start >= End)
            {
                yield return new ValidationResult(
                    "Wartość 'Start' musi być mniejsza niż 'End'.",
                    new[] { nameof(Start), nameof(End) });
            }
        }
    }
}
//chce stworzyć kolejny model o nazwie licznik w którym kluczem obcym będzie id mieszkania i id harmonogramu. Chce żeby w jego kontrolerze była wyliczana cena dziennego zużycia gazu na podstawie danych z medalu harmonogram  