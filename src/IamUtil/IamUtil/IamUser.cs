namespace Nabto.Edge.ClientIam;

public class Fcm
{
    public string? Token { get; set; }
    public string? ProjectId { get; set; }
}

public class IamUser
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }

    public string? Fingerprint { get; set; }

    public string? Password { get; set; }

    public string? Sct { get; set; }

    public string? Role { get; set; }

    public Fcm? Fcm { get; set; }

    public List<string>? NotificationCategories { get; set; }
}
