
namespace Nabto.Edge.ClientIam;
using Nabto.Edge.ClientIam.Impl;


/**
 * <summary>
 * <para>This class simplifies interaction with the Nabto Edge Embedded SDK device's CoAP IAM endpoints.</para>
 *
 * <para>For instance, it is made simple to invoke the different pairing endpoints - just invoke a simple high level
 * pairing function to pair the client with the connected device and don't worry about CBOR encoding and decoding.</para>
 *
 * <para>Read more about the important concept of pairing in the <see href="https://docs.nabto.com/developer/guides/concepts/iam/pairing.html">Nabto Edge IAM Pairing</see> guide.</para>
 *
 * <para>All the most popular IAM device endpoints are wrapped to also allow management of the user profile on the device
 * (own or other users' if client is in admin role).</para>
 *
 * <para>Note that the device's IAM configuration must allow invocation of the different functions and the pairing modes must
 * be enabled at runtime. Read more about that in the <see href="https://docs.nabto.com/developer/guides/concepts/iam/intro.html">general Nabto Edge IAM intro</see>.</para>
 * </summary>
 */
public class IamUtil
{

    // Different ways to pair
    public static Task PairLocalInitialAsync(Nabto.Edge.Client.Connection connection)
    {
        return Pairing.PairLocalInitialAsync(connection);
    }

    /**
     * <summary>
     * <para>Perform <see href="https://docs.nabto.com/developer/guides/concepts/iam/pairing.html#open-local">Local Open pairing</see>, requesting the specified username.</para>
     *
     * <para>Local open pairing uses the trusted local network (LAN) pairing mechanism. No password is required for pairing and no
     * invitation is needed, anybody on the LAN can initiate pairing.</para>
     *
     * <param name="connection">An established connection to the device this client should be paired with</param>
     * <param name="desiredUsername">Assign this username on the device if available (pairing fails with .USERNAME_EXISTS if not)</param>
     *
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`USERNAME_EXISTS` if the username and/or password is invalid.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`INVALID_INPUT` if desiredUsername is <see href="https://docs.nabto.com/developer/api-reference/coap/iam/post-users.html#request">not valid</see>.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`BLOCKED_BY_DEVICE_CONFIGURATION` if the device configuration does not support local open pairing (the `IAM:PairingLocalOpen` action
     * is not set for the Unpaired role or the device does not support the pairing mode at all).</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`PAIRING_MODE_DISABLED` if the pairing mode is configured on the device but is disabled at runtime.</exception>
     * <exception cref="IamException">Thrown with <see cref="IamError"/>`IAM_NOT_SUPPORTED` if Nabto Edge IAM is not supported by the device.</exception>
     * </summary>
     */
    public static Task PairLocalOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername)
    {
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
        return Nabto.Edge.ClientIam.Impl.IamSettings.GetIamSettingsAsync(connection);
    }

    public static Task UpdateIamSettingsPasswordOpenPairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.UpdateIamSettingsPasswordOpenPairingAsync(connection, enabled);
    }

    public static Task UpdateIamSettingsPasswordInvitePairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.UpdateIamSettingsPasswordInvitePairingAsync(connection, enabled);
    }

    public static Task UpdateIamSettingsLocalOpenPairingAsync(Nabto.Edge.Client.Connection connection, bool enabled)
    {
        return Nabto.Edge.ClientIam.Impl.IamSettings.UpdateIamSettingsLocalOpenPairingAsync(connection, enabled);
    }

    // list roles and notification categories

    public static Task<List<string>> ListRolesAsync(Nabto.Edge.Client.Connection connection)
    {
        return DeviceInfo.ListRolesAsync(connection);
    }
    public static Task<List<string>> ListNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection)
    {
        return DeviceInfo.ListNotificationCategoriesAsync(connection);
    }


    // user management
    public static Task<IamUser> GetCurrentUserAsync(Nabto.Edge.Client.Connection connection)
    {
        return User.GetCurrentUserAsync(connection);
    }

    public static Task<IamUser> GetUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        return User.GetUserAsync(connection, username);
    }

    // Update user settings
    public static Task UpdateUserDisplayNameAsync(Nabto.Edge.Client.Connection connection, string username, string displayName)
    {
        return UserSettings.UpdateUserDisplayNameAsync(connection, username, displayName);
    }

    public static Task UpdateUserFcmAsync(Nabto.Edge.Client.Connection connection, string username, string fcmProjectId, string fcmToken)
    {
        return UserSettings.UpdateUserFcmAsync(connection, username, fcmProjectId, fcmToken);
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

    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, IamUser user)
    {
        await User.CreateUserAsync(connection, user);
    }
};
