namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

public class PairLocalInitialTest {

    [Fact]
    public async Task InitialPairing() {
        var iamConnection = await IamConnection.Create();
        var exception = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.PairLocalInitialAsync(iamConnection.Connection); });
        Assert.Equal(IamError.PAIRING_MODE_DISABLED, exception.Error);
    }
}
