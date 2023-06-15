namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

public class PairLocalInitialTest : LocalAllowAllIamFixture
{

    [Fact]
    public async Task InitialPairing()
    {
        var exception = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.PairLocalInitialAsync(_connection); });
        Assert.Equal(IamError.PAIRING_MODE_DISABLED, exception.Error);
    }
}
