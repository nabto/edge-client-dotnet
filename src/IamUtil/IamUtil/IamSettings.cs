namespace Nabto.Edge.ClientIam;

/**
 * <summary>
 * <para>This class represents the Nabto Edge IAM configuration on a Nabto Edge device.</para>
 * <para>Note that this may be different than what is observed through a <see cref="DeviceDetails"/> instance: IamSettings represents the global configuration on the device; <see cref="DeviceDetails"/> 
 * represents the IAM state for a specific connection where e.g. some pairing modes may have been disabled.</para>
 * </summary>
 */
public class IamSettings
{
    /**
     * <summary>Is password open pairing enabled?</summary>
     */
    public bool? PasswordOpenPairing;

    /**
     * <summary>Is password invite pairing enabled?</summary>
     */
    public bool? PasswordInvitePairing;

    /**
     * <summary>Is local open pairing enabled?</summary>
     */
    public bool? LocalOpenPairing;

    /**
     * <summary>Is password invite pairing enabled?</summary>
     */
    public string? PasswordOpenSct;

    /**
     * <summary>Is password invite pairing enabled?</summary>
     */
    public string? PasswordOpenPassword;

}
