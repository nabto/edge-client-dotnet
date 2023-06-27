namespace Nabto.Edge.ClientIam;
/**
 * <summary>IAM Util specific error codes.</summary>
 */
public enum IamError
{
    /**
     * <summary>The device configuration does not support the requested operation.</summary>
     */
    BLOCKED_BY_DEVICE_CONFIGURATION,

    /**
     * <summary>If the initial user was already paired when attempting to perform an initial pairing scenario.</summary>
     */
    INITIAL_USER_ALREADY_PAIRED,

    /**
     * <summary>The operation referenced a user on the device that does not exist.</summary>
     */
    USER_DOES_NOT_EXIST,

    /**
     * <summary>The operation referenced an user or role which does not exists.</summary>
     */
    USER_OR_ROLE_DOES_NOT_EXISTS,

    /**
     * <summary>The operation referenced a role on the device that does not exist.</summary>
     */
    ROLE_DOES_NOT_EXIST,

    /**
     * <summary>The operation requested creating a user or assigning a username that was already in use on the device.</summary>
     */
    USERNAME_EXISTS,

    /**
     * <summary>Specified input is invalid on the device, see the specific endpoint document for restrictions on input, e.g.
     * https://docs.nabto.com/developer/api-reference/coap/iam/post-users.html#request.</summary>
     */
    INVALID_INPUT,

    /**
     * <summary>The requested pairing mode is disabled on the device.</summary>
     */
    PAIRING_MODE_DISABLED,

    /**
     * <summary>The embedded device does not provide a Nabto Edge IAM implementation.</summary>
     */
    IAM_NOT_SUPPORTED,

    /**
     * <summary>The client could not be authenticated towards the device using the specified credentials.</summary>
     */
    AUTHENTICATION_ERROR,

    /**
     * <summary>The client is not allowed to do the requested operation. Improve the Authentication of the client or change the authorization requirements in the device.</summary>
     */
    FORBIDDEN,

    /**
     * <summary>A fingerprint is already in use for another user so it cannot be set on the current user.</summary>
     */
    FINGERPRINT_IN_USE,

    /**
     * <summary>The response cannot be parsed.</summary>
     */
    CANNOT_PARSE_RESPONSE,

    /**
     * <summary>Something unspecified failed.</summary>
     */
    FAILED
}
