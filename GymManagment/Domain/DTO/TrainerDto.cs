using System.ComponentModel.DataAnnotations;

namespace GymManagment.Domain.DTO
{
    public class TrainerDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Range(20, 70)]
        public int Age { get; set; }

        [Required]
        [Range(0, 50)]
        public int ExperienceYears { get; set; }
    }
}
