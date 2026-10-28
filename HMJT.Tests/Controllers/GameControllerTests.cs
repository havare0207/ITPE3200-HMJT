using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using HMJT.Controllers;
using HMJT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace HMJT.Tests.Controllers;

public class GameControllerTests
{
    // Tests that GameController requires
    // the user to be authenticated.
    [Fact]
    public void GameController_ShouldRequireAuthorization()
    {
        // Act
        var authorizeAttribute =
            typeof(GameController)
                .GetCustomAttributes(
                    typeof(AuthorizeAttribute),
                    true
                )
                .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    // Tests that QuestionController also requires
    // the user to be authenticated.
    [Fact]
    public void QuestionController_ShouldRequireAuthorization()
    {
        // Act
        var authorizeAttribute =
            typeof(QuestionController)
                .GetCustomAttributes(
                    typeof(AuthorizeAttribute),
                    true
                )
                .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    // Tests that the GameViewModel rejects an empty game name.
    [Fact]
    public void GameViewModel_ShouldRejectEmptyName()
    {
        // Arrange
        var vm = new GameViewModel
        {
            Name = string.Empty
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(vm);

        // Act
        bool isValid = Validator.TryValidateObject(
            vm,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        // Assert
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            result => result.MemberNames.Contains(nameof(GameViewModel.Name))
        );
    }

    // Tests that the GameViewModel rejects a name
    // shorter than the required two characters.
    [Fact]
    public void GameViewModel_ShouldRejectNameShorterThanTwoCharacters()
    {
        // Arrange
        var vm = new GameViewModel
        {
            Name = "A"
        };

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(vm);

        // Act
        bool isValid = Validator.TryValidateObject(
            vm,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        // Assert
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            result => result.MemberNames.Contains(nameof(GameViewModel.Name))
        );
    }
}