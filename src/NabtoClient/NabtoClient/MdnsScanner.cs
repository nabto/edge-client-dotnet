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

public delegate void MdnsResultHandler(MdnsResult mdnsResult);

public interface MdnsScanner {
    public void Start();
    public MdnsResultHandler Handlers { get; set; }
}
