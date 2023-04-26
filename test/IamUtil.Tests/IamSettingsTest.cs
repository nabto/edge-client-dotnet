namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

public class IamSettingsTest : LocalAllowAllIamFixture {

    [Fact]
    public async Task GetIamSettings() {
        var iamSettings = await IamUtil.GetIamSettingsAsync(_connection);

        Assert.NotNull(iamSettings.LocalOpenPairing);
        Assert.NotNull(iamSettings.PasswordInvitePairing);
        Assert.NotNull(iamSettings.PasswordOpenPairing);
        Assert.NotNull(iamSettings.PasswordOpenSct);
        Assert.NotNull(iamSettings.PasswordOpenPassword);
    }

    [Fact]
    public async Task UpdateIamSettings() {

        // TODO This disables pairing modes which other tests are using.
        // await IamUtil.UpdateIamSettingsLocalOpenPairingAsync(_connection, false);
        // await IamUtil.UpdateIamSettingsPasswordInvitePairingAsync(_connection, false);
        // await IamUtil.UpdateIamSettingsPasswordOpenPairingAsync(_connection, false);

        // var iamSettings = await IamUtil.GetIamSettingsAsync(_connection);
        // Assert.False(iamSettings.LocalOpenPairing);
        // Assert.False(iamSettings.PasswordOpenPairing);
        // Assert.False(iamSettings.PasswordInvitePairing);


        await IamUtil.UpdateIamSettingsLocalOpenPairingAsync(_connection, true);
        await IamUtil.UpdateIamSettingsPasswordInvitePairingAsync(_connection, true);
        await IamUtil.UpdateIamSettingsPasswordOpenPairingAsync(_connection, true);


        var iamSettings = await IamUtil.GetIamSettingsAsync(_connection);
        Assert.True(iamSettings.LocalOpenPairing);
        Assert.True(iamSettings.PasswordOpenPairing);
        Assert.True(iamSettings.PasswordInvitePairing);
    }
}
