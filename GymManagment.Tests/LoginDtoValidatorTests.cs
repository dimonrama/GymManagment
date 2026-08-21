using FluentValidation.TestHelper;
using GymManagment.Application.Validators;
using GymManagment.Domain.DTO;
using Xunit;

namespace GymManagment.Tests
{
    public class LoginDtoValidatorTests
    {
        private readonly LoginDtoValidator _validator = new();
        [Fact]
        public void Should_Have_Error_When_Login_Is_Empty()
        {
            var dto = new LoginDto() { Username = "", Password = "123456"}; ;
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Username);
        }
        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var dto = new LoginDto() { Username = "edewdcc", Password = "" }; ;
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = new LoginDto { Username = "retatre", Password = "123456" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

    }
}
