using FluentValidation;
using GymManagment.Domain.DTO;

namespace GymManagment.Application.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator() { 
       RuleFor(x => x.Username).NotEmpty().WithMessage("Имя пользователя не может быть пустым");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Пароль не может быть пустым");
        }
    }
}
