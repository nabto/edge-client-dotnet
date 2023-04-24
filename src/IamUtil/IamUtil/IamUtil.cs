
namespace Nabto.Edge.Client;
using Nabto.Edge.Client.Impl;

public class IamUtil
{

    // Different ways to pair
    public static Task PairLocalInitialAsync(Nabto.Edge.Client.Connection connection)
    {
        return Pairing.PairLocalInitialAsync(connection);
    }

    public static Task PairLocalOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername) {
        return Pairing.PairLocalOpenAsync(connection, desiredUsername);
    }

    public static Task PairPasswordOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername, string password)
    {
        return Pairing.PairPasswordOpenAsync(connection, desiredUsername, password);
    }

    public static Task PairInvitePasswordAsync(Nabto.Edge.Client.Connection connection, string username, string password)
    {
        return Pairing.PairInvitePasswordAsync(connection, username, password);
    }

    // Get and set device details

    public static Task<DeviceDetails> GetDeviceDetailsAsync(Nabto.Edge.Client.Connection connection)
    {
        return DeviceInfo.GetDeviceDetailsAsync(connection);
    }

    public static Task UpdateDeviceFriendlyNameAsync(Nabto.Edge.Client.Connection connection, string friendlyName) 
    {
        return DeviceInfo.UpdateDeviceFriendlyNameAsync(connection, friendlyName);
    }

    // get set iam settings

    public static Task<IamSettings> GetIamSettingsAsync(Nabto.Edge.Client.Connection connection)
    {
        return Nabto.Edge.Client.Impl.IamSettings.GetIamSettingsAsync(connection);
    }

    public static Task UpdateIamSettingsPasswordOpenPairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.Client.Impl.IamSettings.UpdateIamSettingsPasswordOpenPairingAsync(connection, enabled);
    }

    public static Task UpdateIamSettingsPasswordInvitePairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.Client.Impl.IamSettings.UpdateIamSettingsPasswordInvitePairingAsync(connection, enabled);
    }

    public static Task UpdateIamSettingsLocalOpenPairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.Client.Impl.IamSettings.UpdateIamSettingsLocalOpenPairingAsync(connection, enabled);
    }

    // list roles and notification categories

    public static Task<List<string>> ListRolesAsync(Nabto.Edge.Client.Connection connection)
    {
        return IamUtilImpl.ListRolesAsync(connection);
    }
    public static Task<List<string>> ListNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection)
    {
        return IamUtilImpl.ListNotificationCategoriesAsync(connection);
    }


    // user management
    public static Task<IamUser> GetCurrentUserAsync(Nabto.Edge.Client.Connection connection)
    {
        return IamUtilImpl.GetCurrentUserAsync(connection);
    }

    public static Task<IamUser> GetUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        return IamUtilImpl.GetUserAsync(connection, username);
    }
    
    // Update user settings
    public static Task UpdateUserDisplayNameAsync(Nabto.Edge.Client.Connection connection, string username, string displayName)
    {
        return UserSettings.UpdateUserDisplayNameAsync(connection, username, displayName);
    }

    public static Task UpdateUserFcmAsync(Nabto.Edge.Client.Connection connection, string username, string fcmProjectId, string fcmToken)
    {
        // TODO
        return null;
    }

    public static Task UpdateUserFingerprintAsync(Nabto.Edge.Client.Connection connection, string username, string fingerprint)
    {
        return UserSettings.UpdateUserFingerprintAsync(connection, username, fingerprint);
    }

    public static Task UpdateUserNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection, string username, List<string> categories)
    {
        return UserSettings.UpdateUserNotificationCategoriesAsync(connection, username, categories);
    }

    public static Task UpdateUserPasswordAsync(Nabto.Edge.Client.Connection connection, string username, string password)
    {
        return UserSettings.UpdateUserPasswordAsync(connection, username, password);
    }

    public static Task UpdateUserRoleAsync(Nabto.Edge.Client.Connection connection, string username, string role)
    {
        return UserSettings.UpdateUserRoleAsync(connection, username, role);
    }

    public static Task UpdateUserSctAsync(Nabto.Edge.Client.Connection connection, string username, string sct)
    {
        return UserSettings.UpdateUserSctAsync(connection, username, sct);
    }

    public static Task UpdateUserUsernameAsync(Nabto.Edge.Client.Connection connection, string username, string newUsername)
    {
        return UserSettings.UpdateUserUsernameAsync(connection, username, newUsername);
    }

    public static Task DeleteUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        return User.DeleteUserAsync(connection, username);
    }

    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        await User.CreateUserAsync(connection, username);
    }
};