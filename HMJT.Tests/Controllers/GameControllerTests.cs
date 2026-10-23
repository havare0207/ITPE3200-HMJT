using HMJT.Controllers;
using Microsoft.AspNetCore.Authorization;

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
}