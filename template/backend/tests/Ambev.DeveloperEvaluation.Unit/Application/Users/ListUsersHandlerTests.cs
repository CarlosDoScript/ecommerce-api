using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class ListUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ListUsersHandler _handler;

    public ListUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListUsersHandler(_userRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid query When listing users Then returns paginated result")]
    public async Task Handle_ValidQuery_ReturnsPaginatedResult()
    {
        // Given
        var command = new ListUsersCommand { Page = 1, Size = 10 };
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Username = "user1" },
            new User { Id = Guid.NewGuid(), Username = "user2" }
        };
        var userItems = users.Select(u => new UserListItem { Id = u.Id, Username = u.Username }).ToList();

        _userRepository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((users.AsEnumerable(), 2));
        _mapper.Map<IEnumerable<UserListItem>>(Arg.Any<IEnumerable<User>>()).Returns(userItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.Data.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given total not fitting one page When listing Then calculates total pages correctly")]
    public async Task Handle_MultiplePages_CalculatesTotalPagesCorrectly()
    {
        // Given
        var command = new ListUsersCommand { Page = 1, Size = 5 };
        var users = Enumerable.Range(1, 5).Select(_ => new User()).ToList();
        var userItems = users.Select(_ => new UserListItem()).ToList();

        _userRepository.GetPagedAsync(1, 5, null, Arg.Any<CancellationToken>())
            .Returns((users.AsEnumerable(), 13));
        _mapper.Map<IEnumerable<UserListItem>>(Arg.Any<IEnumerable<User>>()).Returns(userItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3); // ceil(13/5) = 3
    }

    [Fact(DisplayName = "Given empty user list When listing Then returns empty result")]
    public async Task Handle_NoUsers_ReturnsEmptyResult()
    {
        // Given
        var command = new ListUsersCommand { Page = 1, Size = 10 };

        _userRepository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<User>(), 0));
        _mapper.Map<IEnumerable<UserListItem>>(Arg.Any<IEnumerable<User>>()).Returns([]);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalItems.Should().Be(0);
        result.Data.Should().BeEmpty();
    }
}
