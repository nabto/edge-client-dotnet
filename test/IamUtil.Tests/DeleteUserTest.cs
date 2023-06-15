namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

public class DeleteUserTest : LocalAllowAllIamFixture, IAsyncLifetime
{
    protected string _testUser1;
    public DeleteUserTest() : base()
    {
        _testUser1 = TestUtil.UniqueUsername();
    }

    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await IamUtil.CreateUserAsync(_connection, new IamUser { Username = _testUser1 });
    }

    [Fact]
    public async Task DeleteUser()
    {
        await IamUtil.DeleteUserAsync(_connection, _testUser1);

        var e = await Assert.ThrowsAsync<IamException>(async () => await IamUtil.GetUserAsync(_connection, _testUser1));
        Assert.Equal(IamError.USER_DOES_NOT_EXIST, e.Error);
    }

    [Fact]
    public async Task DeleteUser_NonExisting()
    {
        var e = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.DeleteUserAsync(_connection, "nonexisting"); });
        Assert.Equal(IamError.USER_DOES_NOT_EXIST, e.Error);
    }
}
