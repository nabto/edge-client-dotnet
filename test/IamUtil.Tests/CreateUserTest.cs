namespace Nabto.Edge.ClientIam.Tests;

public class CreateUserTest : LocalAllowAllIamFixture {

    [Fact]
    public async void CreateUser() {
        var username = TestUtil.UniqueUsername();
        await IamUtil.CreateUserAsync(_connection, new IamUser{Username = username});

        // ensure we are not paired
        IamException ex = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.GetCurrentUserAsync(_connection); });
        Assert.Equal(IamError.USER_DOES_NOT_EXIST, ex.Error);

        await IamUtil.UpdateUserFingerprintAsync(_connection, username, _connection.GetClientFingerprint());

        var user = await IamUtil.GetCurrentUserAsync(_connection);
        Assert.Equal(username, user.Username);
    }
}
