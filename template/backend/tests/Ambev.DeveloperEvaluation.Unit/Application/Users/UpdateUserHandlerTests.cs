using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users;

public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateUserHandler(_userRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid command When updating user Then returns updated result")]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = UpdateUserHandlerTestData.GenerateCommandWithId(userId);

        var existingUser = new User { Id = userId, Username = "old", Email = "old@email.com" };
        var updatedUser = new User { Id = userId, Username = command.Username, Email = command.Email };
        var result = new UpdateUserResult { Id = userId, Username = command.Username };

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);
        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(updatedUser);
        _mapper.Map<UpdateUserResult>(updatedUser).Returns(result);

        // When
        var updateResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateResult.Should().NotBeNull();
        updateResult.Id.Should().Be(userId);
        await _userRepository.Received(1).UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing user ID When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingUser_ThrowsKeyNotFoundException()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = UpdateUserHandlerTestData.GenerateCommandWithId(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{userId}*");
    }

    [Fact(DisplayName = "Given invalid command When updating Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateUserCommand(); // empty Id, empty Username

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When updating Then user fields are updated correctly")]
    public async Task Handle_ValidCommand_UpdatesUserFields()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = UpdateUserHandlerTestData.GenerateCommandWithId(userId);

        var existingUser = new User { Id = userId };
        var updatedUser = new User { Id = userId };
        var result = new UpdateUserResult { Id = userId };

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingUser);
        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(updatedUser);
        _mapper.Map<UpdateUserResult>(updatedUser).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _userRepository.Received(1).UpdateAsync(
            Arg.Is<User>(u =>
                u.Username == command.Username &&
                u.Email == command.Email &&
                u.Role == command.Role &&
                u.Status == command.Status &&
                u.Name.Firstname == command.Name.Firstname &&
                u.Name.Lastname == command.Name.Lastname),
            Arg.Any<CancellationToken>());
    }
}
