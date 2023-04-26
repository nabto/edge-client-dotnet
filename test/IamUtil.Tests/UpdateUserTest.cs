namespace Nabto.Edge.ClientIam.Tests;

public class UpdateUserTest : LocalAllowAllIamFixture, IAsyncLifetime
{
    protected string _testUser1;
    protected string _testUser2;
    public UpdateUserTest() : base() {
        _testUser1 = TestUtil.UniqueUsername();
        _testUser2 = TestUtil.UniqueUsername();
    }

    public new async Task InitializeAsync() {
        await base.InitializeAsync();

        await IamUtil.CreateUserAsync(_connection, new IamUser{Username = _testUser1});
        await IamUtil.CreateUserAsync(_connection, new IamUser{Username = _testUser2});
    }

    [Fact]
    public async Task UpdateUserDisplayName()
    {
        var newDisplayName = TestUtil.RandomString(10);
        await IamUtil.UpdateUserDisplayNameAsync(_connection, _testUser1, newDisplayName);

        var updatedUser = await IamUtil.GetUserAsync(_connection, _testUser1);

        Assert.Equal(newDisplayName, updatedUser.DisplayName);
    }

    [Fact]
    public async Task UpdateUserFcm()
    {
        var projectId = TestUtil.RandomString(10);
        var token = TestUtil.RandomString(10);
        await IamUtil.UpdateUserFcmAsync(_connection, _testUser1, projectId, token);

        var updatedUser = await IamUtil.GetUserAsync(_connection, _testUser1);

        Assert.NotNull(updatedUser.Fcm);
        Assert.Equal(projectId, updatedUser.Fcm.ProjectId);
        Assert.Equal(token, updatedUser.Fcm.Token);
    }


    [Fact]
    public async Task UpdateUserFingerprint()
    {
        var newFingerprint = TestUtil.RandomFingerprint();
        await IamUtil.UpdateUserFingerprintAsync(_connection, _testUser1, newFingerprint);

        var updatedUser = await IamUtil.GetUserAsync(_connection, _testUser1);

        Assert.Equal(newFingerprint, updatedUser.Fingerprint);
    }

    [Fact]
    public async Task UpdateUserFingerprintConflict()
    {
        var newFingerprint = TestUtil.RandomFingerprint();
        await IamUtil.UpdateUserFingerprintAsync(_connection, _testUser1, newFingerprint);

        IamException e = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.UpdateUserFingerprintAsync(_connection, _testUser2, newFingerprint); });

        Assert.Equal(IamError.FINGERPRINT_IN_USE, e.Error);
    }


    [Fact]
    public async Task UpdateUserNotificationCategories()
    {
        await IamUtil.UpdateUserNotificationCategoriesAsync(_connection, _testUser1, new List<string>{});

        var user = await IamUtil.GetUserAsync(_connection, _testUser1);

        Assert.NotNull(user.NotificationCategories);
        Assert.Empty(user.NotificationCategories);
    }

    [Fact]
    public async Task UpdateUserPassword()
    {
        var password = TestUtil.RandomString(16);
        await IamUtil.UpdateUserPasswordAsync(_connection, _testUser1, password);
    }

    [Fact]
    public async Task UpdateUserRole_NonExisisting()
    {
        var role = TestUtil.RandomString(10);
        var e = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.UpdateUserRoleAsync(_connection, _testUser1, role); });

        Assert.Equal(IamError.USER_OR_ROLE_DOES_NOT_EXISTS, e.Error);
    }

    [Fact]
    public async Task UpdateUserRole()
    {
        var roles = await IamUtil.ListRolesAsync(_connection);
        var role = roles[0];
        await IamUtil.UpdateUserRoleAsync(_connection, _testUser1, role);
        var user = await IamUtil.GetUserAsync(_connection, _testUser1);
        Assert.Equal(role, user.Role);
    }

    [Fact]
    public async Task UpdateUserSct()
    {
        var sct = TestUtil.RandomString(16);
        await IamUtil.UpdateUserSctAsync(_connection, _testUser1, sct);
        var user = await IamUtil.GetUserAsync(_connection, _testUser1);
        Assert.Equal(sct, user.Sct);
    }

    [Fact]
    public async Task UpdateUserUsernameAsync()
    {
        var newUsername = TestUtil.UniqueUsername();
        await IamUtil.UpdateUserUsernameAsync(_connection, _testUser1, newUsername);
        var user = await IamUtil.GetUserAsync(_connection, newUsername);
        Assert.Equal(newUsername, user.Username);
    }


}
