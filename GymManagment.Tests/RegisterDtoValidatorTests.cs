using FluentValidation.TestHelper;
using GymManagment.Application.Validators;
using GymManagment.Domain.DTO;
using Xunit;

namespace GymManagment.Tests
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Username_Is_Empty()
        {
            var dto = new RegisterDto { Username = "", Password = "123456", Role = "Member" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Small()
        {
            var dto = new RegisterDto { Username = "retatre", Password = "123", Role = "Member" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var dto = new RegisterDto { Username = "retatre", Password = "123456", Role = "Member" };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}