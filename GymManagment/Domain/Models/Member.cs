using System.ComponentModel.DataAnnotations;

namespace GymManagment.Domain.Models
{
    public class Member
    {
        public int Id { get; set; }
        [Required] [MaxLength (100)] public string FullName { get; set; } = string.Empty;
        [Required][Range(14, 80)] public int Age { get; set; }
         public int? TrainerId { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;   // ← добавили
        public Trainer? Trainer {get; set;}

    }
}
