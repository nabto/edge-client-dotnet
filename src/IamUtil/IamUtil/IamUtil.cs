
namespace Nabto.Edge.Client;
using Nabto.Edge.Client.Impl;

public class IamUtil
{
    public static Task PairLocalOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername) {
        return IamUtilImpl.PairLocalOpenAsync(connection, desiredUsername);
    }

    public static Task PairPasswordOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername, string password)
    {
        return IamUtilImpl.PairPasswordOpenAsync(connection, desiredUsername, password);
    }

    public static Task<IamUser> GetUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        return IamUtilImpl.GetUserAsync(connection, username);
    }

    public static Task<IamUser> GetCurrentUserAsync(Nabto.Edge.Client.Connection connection)
    {
        return IamUtilImpl.GetCurrentUserAsync(connection);
    }

    public static Task UpdateUserNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection, string username, List<string> categories)
    {
        return IamUtilImpl.UpdateUserNotificationCategoriesAsync(connection, username, categories);
    }

    public static Task UpdateUserDisplayNameAsync(Nabto.Edge.Client.Connection connection, string username, string displayName)
    {
        return IamUtilImpl.UpdateUserDisplayNameAsync(connection, username, displayName);
    }

    public static Task<List<string>> ListRolesAsync(Nabto.Edge.Client.Connection connection)
    {
        return IamUtilImpl.ListRolesAsync(connection);
    }
    public static Task<List<string>> ListNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection)
    {
        return IamUtilImpl.ListNotificationCategoriesAsync(connection);
    }

};