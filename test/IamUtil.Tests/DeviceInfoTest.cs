namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

public class DeviceInfoTest : LocalAllowAllIamFixture
{

    [Fact]
    public async Task ListRoles()
    {

        var roles = await IamUtil.GetRolesAsync(_connection);
        Assert.True(roles.Count > 0);
    }

    [Fact]
    public async Task ListNotificationCategories()
    {
        var categories = await IamUtil.GetNotificationCategoriesAsync(_connection);
        Assert.Empty(categories);
    }
}
