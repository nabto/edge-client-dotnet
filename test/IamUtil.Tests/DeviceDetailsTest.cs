namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

public class DeviceDetailsTest : LocalAllowAllIamFixture
{
    [Fact]
    public async Task GetDeviceDetails() {
        var details = await IamUtil.GetDeviceDetailsAsync(_connection);
        Assert.NotNull(details.AppName);
        //Assert.NotNull(details.AppVersion); // AppVersion is not set in default tunnel
        Assert.NotNull(details.DeviceId);
        Assert.NotNull(details.ProductId);
        Assert.NotNull(details.FriendlyName);
        Assert.NotNull(details.NabtoVersion);
        Assert.NotEmpty(details.Modes);
    }

    [Fact]
    public async Task SetDeviceFriendlyName() {
        var displayName = TestUtil.RandomString(16);
        await IamUtil.UpdateDeviceFriendlyNameAsync(_connection, displayName);

        var details = await IamUtil.GetDeviceDetailsAsync(_connection);

        Assert.Equal(displayName, details.FriendlyName);
    }

};
