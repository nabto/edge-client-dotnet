namespace Nabto.Edge.ClientIam.Tests;

using Nabto.Edge.Client.Tests;
using Nabto.Edge.Client;


public class PairInvitePasswordTest : LocalAllowAllIamFixture {
    [Fact]
    public async Task PairSuccess()
    {
        // create a new user on the device

        var username = TestUtil.UniqueUsername();
        string password = "supersecret";
        await IamUtil.CreateUserAsync(_connection, username);
        await IamUtil.UpdateUserPasswordAsync(_connection, username, password);
        await IamUtil.UpdateUserRoleAsync(_connection, username, "Administrator");

        var c = _client.CreateConnection();
        c.SetOptions(GetConnectionOptions());
        c.SetOptions(new ConnectionOptions { PrivateKey = _client.CreatePrivateKey() });

        await c.ConnectAsync();

        await IamUtil.PairInvitePasswordAsync(c, username, password);

        var user = await IamUtil.GetCurrentUserAsync(c);
        Assert.Equal(username, user.Username);
    }
}
