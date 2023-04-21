namespace Nabto.Edge.Client.Tests;

public class PairInvitePasswordTest : LocalAllowAllIamFixture {
    [Fact]
    public async Task PairSuccess() {
        var iamConnection = await IamConnection.Create();

        // create a new user on the device

        var username = TestUtil.UniqueUsername();
        string password = "supersecret";
        // TODO
        //await IamUtil.CreateUserAsync(iamConnection.Connection, username);

    }   
}