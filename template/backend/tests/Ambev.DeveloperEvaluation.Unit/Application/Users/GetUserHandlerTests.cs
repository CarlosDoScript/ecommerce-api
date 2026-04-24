using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUserHandler(_userRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing user ID When getting user Then returns user result")]
    public async Task Handle_ExistingUser_ReturnsUserResult()
    {
        // Given
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "johndoe", Email = "john@email.com" };
        var result = new GetUserResult { Id = userId, Name = user.Username, Email = user.Email };

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(result);

        // When
        var response = await _handler.Handle(new GetUserCommand(userId), CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(userId);
        response.Email.Should().Be("john@email.com");
    }

    [Fact(DisplayName = "Given non-existing user ID When getting user Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingUser_ThrowsKeyNotFoundException()
    {
        // Given
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // When
        var act = () => _handler.Handle(new GetUserCommand(userId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{userId}*");
    }

    [Fact(DisplayName = "Given existing user When getting Then repository is called with correct ID")]
    public async Task Handle_ExistingUser_CallsRepositoryWithCorrectId()
    {
        // Given
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var result = new GetUserResult { Id = userId };

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(result);

        // When
        await _handler.Handle(new GetUserCommand(userId), CancellationToken.None);

        // Then
        await _userRepository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
    }
}
