namespace Nabto.Edge.ClientIam;

public class Fcm
{
    public string? Token { get; set; }
    public string? ProjectId { get; set; }
}

public class IamUser
{
    public String? Username { get; set; }
    public String? DisplayName { get; set; }

    public String? Fingerprint { get; set; }

    public String? Sct { get; set; }

    public String? Role { get; set; }

    public Fcm? Fcm { get; set; }

    public List<string>? NotificationCategories { get; set; }
}
