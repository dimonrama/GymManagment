using FluentValidation.TestHelper;
using GymManagment.Application.Validators;
using GymManagment.Domain.DTO;
using Xunit;

namespace GymManagment.Tests
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator = new();

        // ---------- общие поля ----------

        [Fact]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            var dto = new RegisterDto { Username = "", Password = "123456", Role = "Admin" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Small()
        {
            var dto = new RegisterDto { Username = "retatre", Password = "123", Role = "Admin" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        // ---------- Admin: профиль не нужен ----------

        [Fact]
        public void Should_Not_Have_Errors_When_Admin_Without_Profile()
        {
            var dto = new RegisterDto { Username = "retatre", Password = "123456", Role = "Admin" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // ---------- Member ----------

        [Fact]
        public void Should_Not_Have_Errors_When_Member_Is_Valid()
        {
            var dto = new RegisterDto
            {
                Username = "retatre",
                Password = "123456",
                Role = "Member",
                FullName = "Иван Петров",
                Age = 25,
                Email = "ivan@mail.ru"
            };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Errors_When_Member_Has_No_Profile()
        {
            var dto = new RegisterDto { Username = "retatre", Password = "123456", Role = "Member" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
            result.ShouldHaveValidationErrorFor(x => x.Age);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Member_Age_Is_Out_Of_Range()
        {
            var dto = new RegisterDto
            {
                Username = "retatre",
                Password = "123456",
                Role = "Member",
                FullName = "Иван Петров",
                Age = 13,
                Email = "ivan@mail.ru"
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Age);
        }

        [Fact]
        public void Should_Have_Error_When_Member_Email_Is_Invalid()
        {
            var dto = new RegisterDto
            {
                Username = "retatre",
                Password = "123456",
                Role = "Member",
                FullName = "Иван Петров",
                Age = 25,
                Email = "not-an-email"
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        // ---------- Trainer ----------

        [Fact]
        public void Should_Not_Have_Errors_When_Trainer_Is_Valid()
        {
            var dto = new RegisterDto
            {
                Username = "retatre",
                Password = "123456",
                Role = "Trainer",
                FullName = "Андрей Семёнов",
                Age = 35,
                ExperienceYears = 10
            };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Trainer_Experience_Is_Missing()
        {
            var dto = new RegisterDto
            {
                Username = "retatre",
                Password = "123456",
                Role = "Trainer",
                FullName = "Андрей Семёнов",
                Age = 35
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.ExperienceYears);
        }

        [Fact]
        public void Should_Have_Error_When_Trainer_Age_Is_Out_Of_Range()
        {
            var dto = new RegisterDto
            {
                Username = "retatre",
                Password = "123456",
                Role = "Trainer",
                FullName = "Андрей Семёнов",
                Age = 19,
                ExperienceYears = 1
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Age);
        }
    }
}