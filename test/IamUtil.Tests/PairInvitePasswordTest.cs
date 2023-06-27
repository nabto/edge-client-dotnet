namespace Nabto.Edge.ClientIam.Tests;

using Nabto.Edge.Client.Tests;
using Nabto.Edge.Client;


public class PairInvitePasswordTest : LocalAllowAllIamFixture
{
    [Fact]
    public async Task PairSuccess()
    {
        // create a new user on the device

        var username = TestUtil.UniqueUsername();
        string password = "supersecret";
        await IamUtil.CreateUserAsync(_connection, new IamUser { Username = username, Password = password, Role = "Administrator" });

        var c = _client.CreateConnection();
        c.SetOptions(GetConnectionOptions());
        c.SetOptions(new ConnectionOptions { PrivateKey = _client.CreatePrivateKey() });

        await c.ConnectAsync();

        await IamUtil.PairPasswordInviteAsync(c, username, password);

        var user = await IamUtil.GetCurrentUserAsync(c);
        Assert.Equal(username, user.Username);
    }
}
