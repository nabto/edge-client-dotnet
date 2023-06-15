namespace Nabto.Edge.ClientIam.Tests;

using Xunit;
using System.Text.RegularExpressions;

public class IamExceptionTest : LocalAllowAllIamFixture
{

    [Fact]
    public async Task exceptionHasMeaningFullMessage()
    {
        var username = await CreateDefaultUser();
        IamException exception = await Assert.ThrowsAsync<IamException>(async () => { await IamUtil.UpdateUserNotificationCategoriesAsync(_connection, username, new List<string> { "nonexisting_category" }); });
        Assert.Equal(IamError.INVALID_INPUT, exception.Error);
        Assert.True(Regex.IsMatch(exception.Message, @"One or more categories are invalid", RegexOptions.IgnoreCase));
    }
}
