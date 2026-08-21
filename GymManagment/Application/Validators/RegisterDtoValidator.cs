using FluentValidation;
using GymManagment.Domain.DTO;

namespace GymManagment.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator() {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Имя пользователя обязательно").MaximumLength(50);

            RuleFor(x=>x.Password).NotEmpty().WithMessage("Пароль не может быть пустым").MinimumLength(6).WithMessage("Пароль должен составлять не менее 6 символов");

            RuleFor(x => x.Role).NotEmpty().Must(x => x == "Trainer" || x == "Admin" || x == "Member").WithMessage("Недопустимое значение роли");
        }
    }
}
