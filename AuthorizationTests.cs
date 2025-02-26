using System.Security.Claims;
using Bunit;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
//using TestTest.Components.Pages; // Tilpas namespace
using TestTest.Components.Pages;
using Xunit;

public class AuthorizationTests : TestContext
{

    public AuthorizationTests()
    {
        // Tilføj AuthenticationStateProvider til test-miljøet
        Services.AddAuthorizationCore();
    }

    [Fact]
    public void NotAuthenticated_User_Should_See_GuestMessage()
    {
        // Arrange - Simulerer en ikke-logget ind bruger
        var authContext = this.AddTestAuthorization();
        authContext.SetNotAuthorized();

        var cut = RenderComponent<TestAuthorization>();

        // Act & Assert - Matcher den tekst, en ikke-logget ind bruger ser
        cut.MarkupMatches(@"
            <h1>Authorization Test Page</h1>
            <div>
                <h1>Hello, Guest</h1>
                <h3>Please log in to see more content.</h3>
            </div>
        ");
    }

    [Fact]
    public void Authenticated_User_Should_See_NotAdminMessage()
    {
        // Arrange - Simulerer en logget ind bruger uden admin-rolle
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("TestUser"); // Almindelig bruger uden admin-rolle

        var cut = RenderComponent<TestAuthorization>();

        // Act & Assert - Matcher den tekst, en logget ind bruger ser
        cut.MarkupMatches(@"
            <h1>Authorization Test Page</h1>
            <div>
                <h1>Hello, TestUser</h1>
                <h3>You are not admin sadly.</h3>
            </div>
        ");
    }

    [Fact]
    public void Admin_User_Should_See_AdminMessage()
    {
        // Arrange - Simulerer en admin-bruger
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("AdminUser");
        authContext.SetRoles("Admin"); // Gør brugeren til admin

        var cut = RenderComponent<TestAuthorization>();

        // Act & Assert - Matcher den tekst, en admin-bruger ser
        cut.MarkupMatches(@"
            <h1>Authorization Test Page</h1>
            <div>
                <h1>Hello, AdminUser</h1>
                <h3>You are admin!</h3>
            </div>
        ");
    }
}
