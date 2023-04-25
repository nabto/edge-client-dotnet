namespace Nabto.Edge.ClientIam.Tests;

using Xunit;
using System.Text.RegularExpressions;

public class IamExceptionTest {

    [Fact]
    public async Task exceptionHasMeaningFullMessage() {
        var iamConnection = await IamConnection.Create();

        IamException exception = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.UpdateUserNotificationCategoriesAsync(iamConnection.Connection, iamConnection.Username, new List<string> { "nonexisting_category" }); });
        Assert.Equal(IamError.INVALID_INPUT, exception.Error);
        Assert.True(Regex.IsMatch(exception.Message, @"One or more categories are invalid", RegexOptions.IgnoreCase));
    }
}
