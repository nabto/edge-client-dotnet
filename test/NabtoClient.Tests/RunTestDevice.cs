using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Nabto.Edge.Client.Tests;



public class DeviceConfig
{
    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
}

public class TestDeviceRunner : IDisposable
{

    private string _iamConfig = @"    {
   ""Config"" : {
      ""UnpairedRole"" : ""Unpaired""
   },
   ""Policies"" : [
      {
         ""Id"" : ""Pairing"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""IAM:GetPairing"",
                  ""IAM:PairingPasswordInvite"",
                  ""IAM:PairingLocalInitial"",
                  ""IAM:PairingLocalOpen"",
                  ""IAM:PairingPasswordOpen""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      },
      {
         ""Id"" : ""Tunnelling"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""TcpTunnel:GetService"",
                  ""TcpTunnel:Connect"",
                  ""TcpTunnel:ListServices""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      },
      {
         ""Id"" : ""ManageIAM"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""IAM:ListUsers"",
                  ""IAM:GetUser"",
                  ""IAM:DeleteUser"",
                  ""IAM:SetUserRole"",
                  ""IAM:ListRoles"",
                  ""IAM:CreateUser"",
                  ""IAM:SetUserPassword"",
                  ""IAM:SetUserDisplayName"",
                  ""IAM:SetUserFingerprint"",
                  ""IAM:SetUserSct"",
                  ""IAM:SetUserUsername"",
                  ""IAM:SetUserFcm"",
                  ""IAM:SetSettings"",
                  ""IAM:GetSettings"",
                  ""IAM:SetDeviceInfo"",
                  ""IAM:SetUserNotificationCategories"",
                  ""IAM:ListNotificationCategories""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      },
      {
         ""Id"" : ""ManageOwnUser"",
         ""Statements"" : [
            {
               ""Actions"" : [
                  ""IAM:GetUser"",
                  ""IAM:DeleteUser"",
                  ""IAM:SetUserDisplayName""
               ],
               ""Conditions"" : [
                  {
                     ""StringEquals"" : {
                        ""IAM:Username"" : [
                           ""${Connection:Username}""
                        ]
                     }
                  }
               ],
               ""Effect"" : ""Allow""
            },
            {
               ""Actions"" : [
                  ""IAM:ListRoles"",
                  ""IAM:ListUsers""
               ],
               ""Effect"" : ""Allow""
            }
         ]
      }
   ],
   ""Roles"" : [
      {
         ""Id"" : ""Unpaired"",
         ""Policies"" : [
            ""Pairing"",
            ""ManageIAM""
         ]
      },
      {
         ""Id"" : ""Administrator"",
         ""Policies"" : [
            ""ManageIAM"",
            ""Tunnelling"",
            ""Pairing""
         ]
      },
      {
         ""Id"" : ""Administrator2"",
         ""Policies"" : [
            ""ManageIAM"",
            ""Tunnelling"",
            ""Pairing""
         ]
      },
      {
         ""Id"" : ""Standard"",
         ""Policies"" : [
            ""Tunnelling"",
            ""Pairing"",
            ""ManageOwnUser""
         ]
      },
      {
         ""Id"" : ""Guest"",
         ""Policies"" : [
            ""Pairing"",
            ""ManageOwnUser""
         ]
      }
   ],
   ""Version"" : 1
}";

    private string _tempPath;


    private StreamWriter _standardError = default!;
    private StreamWriter _standardOutput = default!;
    private Guid _uuid;

    private Process _deviceProcess = default!;

    public string ProductId { get; set; } = TestUtil.RandomProductId();
    public string DeviceId { get; set; } = TestUtil.RandomDeviceId();

    public string FriendlyName { get; set; } = TestUtil.RandomString(10);

    public TestDeviceRunner()
    {
        _uuid = Guid.NewGuid();
        _tempPath = Path.GetTempPath() + "device-" + _uuid;
        Directory.CreateDirectory(_tempPath);
        init();
        StartDevice();
    }

    public void Dispose()
    {
        StopDevice();
    }

    ~TestDeviceRunner()
    {
    }

    public void init()
    {
        string configDir = Path.Combine(_tempPath, "config");
        string keysDir = Path.Combine(_tempPath, "keys");
        string stateDir = Path.Combine(_tempPath, "state");
        string logsDir = Path.Combine(_tempPath, "logs");
        Directory.CreateDirectory(configDir);
        Directory.CreateDirectory(keysDir);
        Directory.CreateDirectory(stateDir);
        Directory.CreateDirectory(logsDir);

        WriteConfig(configDir);
        WriteIamConfig(configDir);
        WriteServicesConfig(configDir);
        WriteKey(keysDir);
        WriteState(stateDir);

        _standardError = new StreamWriter(Path.Combine(logsDir, "error.log"));
        _standardOutput = new StreamWriter(Path.Combine(logsDir, "output.log"));
    }

    public void WriteConfig(string configDir)
    {
        var deviceConfig = new DeviceConfig { ProductId = ProductId, DeviceId = DeviceId };
        string jsonString = JsonSerializer.Serialize(deviceConfig);
        var sw = new StreamWriter(Path.Combine(configDir, "device.json"));
        sw.Write(jsonString);
        sw.Close();
    }

    public void WriteIamConfig(string configDir)
    {
        var sw = new StreamWriter(Path.Combine(configDir, "tcp_tunnel_device_iam_config.json"));
        sw.Write(_iamConfig);
        sw.Close();
    }

    public void WriteServicesConfig(string configDir)
    {
        var sw = new StreamWriter(Path.Combine(configDir, "tcp_tunnel_device_services.json"));
        sw.Write("[]");
        sw.Close();
    }

    public void WriteKey(string keysDir)
    {
        var client = INabtoClient.Create();
        string privateKey = client.CreatePrivateKey();
        var sw = new StreamWriter(Path.Combine(keysDir, "device.key"));
        sw.Write(privateKey);
        sw.Close();
    }

    public void WriteState(string stateDir)
    {
        string defaultState = @"{""Version"":1,""FriendlyName"":""" + FriendlyName + @""", ""OpenPairingPassword"":""rHfMdaw4zpne"",""OpenPairingSct"":""zAsxwgETYzrX"",""LocalOpenPairing"":true,""PasswordOpenPairing"":true,""PasswordInvitePairing"":true,""LocalInitialPairing"":true,""OpenPairingRole"":""Administrator"",""InitialPairingUsername"":""admin"",""Users"": {""Username"": ""admin"", ""role"": ""Administrator""}}";
        var sw = new StreamWriter(Path.Combine(stateDir, "tcp_tunnel_device_iam_state.json"));

        sw.Write(defaultState);
        sw.Close();
    }

    public void StartDevice()
    {
        ProcessStartInfo info = new ProcessStartInfo();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            info.FileName = "../../../../../test-devices/tcp_tunnel_device_macos";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                info.FileName = "../../../../../test-devices/tcp_tunnel_device_linux_arm64";
            }
            else
            {
                info.FileName = "../../../../../test-devices/tcp_tunnel_device_linux_x86-64";
            }

        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            info.FileName = "../../../../../test-devices/tcp_tunnel_device_windows.exe";
        }

        info.Arguments = $"-H {_tempPath} --random-ports";
        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;

        _deviceProcess = new Process
        {
            StartInfo = info
        };
        bool v = _deviceProcess.Start();

        _ = ReadOutputAsync(_deviceProcess.StandardError, _standardError);
        _ = ReadOutputAsync(_deviceProcess.StandardOutput, _standardOutput);
        _deviceProcess.Exited += this.ProcessExited;
        _deviceProcess.EnableRaisingEvents = true;
    }

    public void ProcessExited(object? sender, EventArgs e)
    {
        bool ok = false;
        if (_deviceProcess.ExitCode == 0)
        {
            ok = true;
        }
        else
        {
            if (System.OperatingSystem.IsWindows())
            {
                ok = _deviceProcess.ExitCode == -1 /* killed */;
            }
            else
            {
                ok = _deviceProcess.ExitCode == 137 /* killed with sigterm */ ||
                   _deviceProcess.ExitCode == 145 /* killed with sigkill */;
            }
        }
        if (!ok)
        {
            Assert.Fail($"Unexpected exit from tcp tunnel, status was {_deviceProcess.ExitCode}");
        }
    }

    public async Task ReadOutputAsync(StreamReader sr, StreamWriter sw)
    {

        try
        {
            string? line;
            do
            {
                line = await sr.ReadLineAsync();
                if (line != null)
                {
                    await sw.WriteLineAsync(line);
                }
            } while (line != null);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void StopDevice()
    {
        _deviceProcess.Kill();
        _standardError.Close();
        _standardOutput.Close();
    }


}
