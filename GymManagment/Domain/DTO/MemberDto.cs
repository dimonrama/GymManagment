
using System.ComponentModel.DataAnnotations;

namespace GymManagment.Domain.DTO
{
    public class MemberDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required]
        [Range(14, 80)]
        public int Age { get; set; }
        public int? TrainerId { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;   // ← добавили



    }
}
