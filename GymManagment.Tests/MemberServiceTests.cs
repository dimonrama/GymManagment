using AutoMapper;
using GymManagment.Application.Repositories;
using GymManagment.Application.Services;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace GymManagment.Tests;

public class MemberServiceTests
{
  
    [Fact]
    public async Task CreateMember_InvalidAge_ReturnsValidationError()
    {
        // Arrange
        var mockRepo = new Mock<IMemberRepository>(); // фейковый репозиторий
        var mockMapper = new Mock<IMapper>(); // фейковый маппер
        var service = new MemberService(mockMapper.Object, mockRepo.Object);

        var dto = new MemberDto
        {
            FullName = "Тест Тестов",
            Age = 5, // невалидный возраст
            Email = "test@test.com"
        };

        // Act
        var result = await service.CreateMemberAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Result.ErrorTypes.ValidationError, result.ErrorType);
    }

    [Fact]
    public async Task CreateMember_EmailAlreadyExists_ReturnsConflict()
    {
        var mockRepo = new Mock<IMemberRepository>();
        var mockMapper = new Mock<IMapper>();
        var service = new MemberService(mockMapper.Object,mockRepo.Object);

        var dto = new MemberDto
        {
            FullName = "Андрей Тест(остерон)",
            Age = 25, 
            Email = "test@test.com"
        };
        mockRepo.Setup(r => r.IsEmailExistsAsync("test@test.com")).ReturnsAsync(true);
        var result = await service.CreateMemberAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Result.ErrorTypes.Conflict, result.ErrorType);
    }
    [Fact]
    public async Task Should_Return_Only_Members_Of_Given_Trainer_When_TrainerId_Filter_Applied()
    {
        var mockRepo = new Mock<IMemberRepository>(); 
        var mockMapper = new Mock<IMapper>(); 
       var fakeMembers = new PagedResult<Member>
    {
        Items = new List<Member> { new Member { Id = 1, FullName = "Test", TrainerId = 5 } },
        TotalCount = 1,
        TotalPages = 1,
        CurrentPage = 1,
        PageSize = 10
    };
        var fakeMappedResult = new PagedResult<MemberDto>
        {
            Items = new List<MemberDto> { new MemberDto { Id = 1, FullName = "Test", TrainerId = 5 } },
            TotalCount = 1,
            TotalPages = 1,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepo.Setup(r => r.GetAllAsync(1, 5)).ReturnsAsync(fakeMembers);
        mockMapper.Setup(m => m.Map<PagedResult<MemberDto>>(fakeMembers)).Returns(fakeMappedResult);

        var service = new MemberService(mockMapper.Object, mockRepo.Object);

        // Act
        var result = await service.GetAllMembersAsync(1, 5);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Test", result.Items[0].FullName);
        mockRepo.Verify(r => r.GetAllAsync(1, 5), Times.Once);
    }
} 