using System.Diagnostics;
using System.IO;

namespace Nabto.Edge.Client.Tests;

public class RunTestDevice {

    public string CreateTestDirectory(string deviceId)
    {
        string newDir = Path.GetTempPath() + Guid.NewGuid();
        string configDir = Path.Combine(newDir, "config");
        string keysDir = Path.Combine(newDir, "keys");
        string stateDir = Path.Combine(newDir, "state");


        Directory.CreateDirectory(newDir);


        return newDir;
    }


    [Fact]
    public async Task StartStopDevice() {

        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = "/workspace/edge-client-dotnet/test-devices/tcp_tunnel_device_linux";
        info.Arguments = "-H ${SCRIPT_DIR}/config/$config --random-ports $opts 2>&1 > /tmp/tunnel-$config";
        var process = new Process()
        {
            //StartInfo =
        };

        //process.Close();
        //process.StandardInput.Close();
        //await process.WaitForExitAsync();
    }

}
