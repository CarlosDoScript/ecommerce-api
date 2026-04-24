using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new DeleteUserHandler(_userRepository);
    }

    [Fact(DisplayName = "Given existing user ID When deleting Then returns success")]
    public async Task Handle_ExistingUser_ReturnsSuccess()
    {
        // Given
        var userId = Guid.NewGuid();
        _userRepository.DeleteAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "Given non-existing user ID When deleting Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingUser_ThrowsKeyNotFoundException()
    {
        // Given
        var userId = Guid.NewGuid();
        _userRepository.DeleteAsync(userId, Arg.Any<CancellationToken>()).Returns(false);

        // When
        var act = () => _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{userId}*");
    }

    [Fact(DisplayName = "Given empty user ID When deleting Then throws ValidationException")]
    public async Task Handle_EmptyId_ThrowsValidationException()
    {
        // Given
        var command = new DeleteUserCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given existing user When deleting Then repository DeleteAsync is called")]
    public async Task Handle_ExistingUser_CallsRepositoryDelete()
    {
        // Given
        var userId = Guid.NewGuid();
        _userRepository.DeleteAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        // Then
        await _userRepository.Received(1).DeleteAsync(userId, Arg.Any<CancellationToken>());
    }
}
