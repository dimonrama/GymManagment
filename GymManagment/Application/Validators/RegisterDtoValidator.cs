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
           
            When(x => x.Role == "Member", () =>
            {
                RuleFor(x => x.FullName).NotEmpty().WithMessage("ФИО обязательно").MaximumLength(100);
                RuleFor(x => x.Age).NotNull().WithMessage("Возраст обязателен")
                    .InclusiveBetween(14, 80).WithMessage("Возраст клиента должен быть от 14 до 80");
                RuleFor(x => x.Email).NotEmpty().WithMessage("Email обязателен")
                    .EmailAddress().WithMessage("Некорректный email").MaximumLength(100);
            });

            When(x => x.Role == "Trainer", () =>
            {
                RuleFor(x => x.FullName).NotEmpty().WithMessage("ФИО обязательно").MaximumLength(100);
                RuleFor(x => x.Age).NotNull().WithMessage("Возраст обязателен")
                    .InclusiveBetween(20, 70).WithMessage("Возраст тренера должен быть от 20 до 70");
                RuleFor(x => x.ExperienceYears).NotNull().WithMessage("Стаж обязателен")
                    .InclusiveBetween(0, 50).WithMessage("Стаж должен быть от 0 до 50");
            });
        }
    }
}
