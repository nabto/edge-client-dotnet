using Xunit;

namespace Nabto.Edge.Client.Tests;

public class GetUserTest
{

    [Fact]
    public async Task GetCurrentUser()
    {
        var iamConnection = await IamConnection.Create();

        var user = await IamUtil.GetCurrentUserAsync(iamConnection.Connection);
        Assert.Equal(user.Username, iamConnection.Username);
        Assert.NotNull(user.Role);
        Assert.NotNull(user.Sct);
        Assert.Equal(user.Fingerprint, iamConnection.Connection.GetClientFingerprint());
        Assert.Empty(user.NotificationCategories);
    }

    [Fact]
    public async Task ListRoles() { 
        var iamConnection = await IamConnection.Create();

        var roles = await IamUtil.ListRolesAsync(iamConnection.Connection);
        Assert.True(roles.Count > 0);
    }

    [Fact]
    public async Task ListNotificationCategories() { 
        var iamConnection = await IamConnection.Create();

        var categories = await IamUtil.ListNotificationCategoriesAsync(iamConnection.Connection);
        Assert.Empty(categories);
    }



}