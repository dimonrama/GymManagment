

namespace GymManagment.Domain.DTO
{
    public class RegisterDto
    {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;

            // Поля профиля: только для ролей Member и Trainer (для Admin не нужны)
            public string? FullName { get; set; }
            public int? Age { get; set; }
            public string? Email { get; set; }          // только Member
            public int? TrainerId { get; set; }         // только Member, необязательно
            public int? ExperienceYears { get; set; }   // только Trainer
        
    }

    public class LoginDto
    {
       
        public string Username { get; set; } = string.Empty;

       
        public string Password { get; set; } = string.Empty;
    }
}
