namespace Nabto.Edge.Client;

public class MdnsResult {
    public enum MdnsAction {
        ADD = 0,
        UPDATE = 1,
        REMOVE = 2
    }
    public string? ServiceInstanceName { get; set; }
    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
    public MdnsAction Action { get; set; }
};

public interface MdnsScanner {
    public delegate void ResultHandler(MdnsResult mdnsResult);
    public void Start();
    public ResultHandler? Handlers { get; set; }
}
