namespace Nabto.Edge.ClientIam;

/**
 * <summary>This class represents <see href="https://docs.nabto.com/developer/guides/push/intro.html">Nabto Push FCM</see> configuration.</summary>
 */
public class Fcm
{
    /**
     * <summary>FCM registration token for push notifications, identifying a specific client to receive notifications. See the <see href="https://docs.nabto.com/developer/guides/push/intro.html">Nabto Edge Push Intro</see>.</summary>
     */
    public string? Token { get; set; }
    /**
     * <summary>FCM project id push notifications. See the <see href="https://docs.nabto.com/developer/guides/push/intro.html">Nabto Edge Push Intro</see>.</summary>
     */
    public string? ProjectId { get; set; }
}

/**
 * <summary>This class represents a Nabto Edge IAM user.</summary>
 */
public class IamUser
{
    /**
     * <summary>Username of the user.</summary>
     */
    public string? Username { get; set; }
    /**
     * <summary>Display name of the user.</summary>
     */
    public string? DisplayName { get; set; }

    /**
     * <summary>Public key fingerprint of the user.</summary>
     */
    public string? Fingerprint { get; set; }

    /**
     * <summary>Password for the user (only for PUT operations).</summary>
     */
    public string? Password { get; set; }

    /**
     * <summary>Server Connect Token for user.</summary>
     */
    public string? Sct { get; set; }

    /**
     * <summary>The single role this user is assigned.</summary>
     */
    public string? Role { get; set; }

    /**
     * <summary>The <see cref="Fcm"/> configuration for the user.</summary>
     */
    public Fcm? Fcm { get; set; }

    /**
     * <summary>The push notification categories the user is subscribed to.</summary>
     */
    public List<string>? NotificationCategories { get; set; }
}
