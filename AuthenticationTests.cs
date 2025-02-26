using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TestTest.Components.Account.Pages.Manage; // Tilpas namespace til dit projekt
using TestTest.Components.Pages;
using Xunit;

public class AuthenticationTests : TestContext
{

    [Fact]
    public void NotAuthenticated_User_Should_See_LoginMessage()
    {
        // Arrange
        var authContext = this.AddTestAuthorization();
        authContext.SetNotAuthorized(); // Simulerer en ikke-logget ind bruger

        var cut = RenderComponent<Home>();

        // Act & Assert - Matcher den tekst, en ikke-logget ind bruger ser
        cut.MarkupMatches(@"
            <h1>Hello, world!</h1>
            <p>Please log in to access more features.</p>
        ");
    }


    [Fact]
    public void Authenticated_User_Can_See_AuthenticatorSetup()
    {
        // Arrange - Simulerer en logget ind bruger
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser");

        var cut = RenderComponent<Home>();

        // Act & Assert - Matcher den tekst, en logget ind bruger ser
        cut.MarkupMatches(@"
            <h1>Hello, world!</h1>
            <p>Welcome back, authenticated user!</p>
        ");
    }
}
