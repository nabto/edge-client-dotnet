using Xunit;

namespace Nabto.Edge.ClientIam.Tests;

public class GetUserTest : LocalAllowAllIamFixture
{

    [Fact]
    public async Task GetCurrentUser()
    {
        var username = await CreateDefaultUser();

        var user = await IamUtil.GetCurrentUserAsync(_connection);
        Assert.Equal(user.Username, username);
        Assert.NotNull(user.Role);
        Assert.NotNull(user.Sct);
        Assert.Equal(user.Fingerprint, _connection.GetClientFingerprint());
        Assert.NotNull(user.NotificationCategories);
        Assert.Empty(user.NotificationCategories);
    }
}
