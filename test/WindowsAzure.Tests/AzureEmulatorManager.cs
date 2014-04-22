using System.Diagnostics;
using System.Linq;

namespace WindowsAzure.Tests
{
    public static class AzureEmulatorManager
    {
        public static AzureStorageEmulatorManager Storage = AzureStorageEmulatorManager.Instance;
        public static AzureComputeEmulatorManager Compute = AzureComputeEmulatorManager.Instance;
    }

    public class AzureStorageEmulatorManager
    {
        private const string StorageEmulatorProcessName = "DSServiceLDB";
        private const string EmulatorPath = @"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun.exe";

        public readonly string ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://ipv4.fiddler";

        public static AzureStorageEmulatorManager Instance = new AzureStorageEmulatorManager();

        public Process GetProcess()
        {
            return Process.GetProcessesByName(StorageEmulatorProcessName).FirstOrDefault();
        }

        public bool IsRunning()
        {
            return GetProcess() != null;
        }

        public void Start()
        {
            if (!IsRunning())
            {
                using (var process = Process.Start(EmulatorPath, "/devstore:start"))
                {
                    process.WaitForExit();
                }
            }
        }

        public void Stop()
        {
            if (IsRunning())
            {
                using (var process = Process.Start(EmulatorPath, "/devstore:shutdown"))
                {
                    process.WaitForExit();
                }
            }
        }

        public void Kill()
        {
            var process = GetProcess();
            if (process != null)
            {
                process.Kill();
            }
        }
    }

    public class AzureComputeEmulatorManager
    {
        private const string ComputeEmulatorProcessName = "DFService";
        private const string ComputeEmulatorPath = @"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun.exe";

        public static AzureComputeEmulatorManager Instance = new AzureComputeEmulatorManager();

        public Process GetProcess()
        {
            return Process.GetProcessesByName(ComputeEmulatorProcessName).FirstOrDefault();
        }

        public bool IsRunning()
        {
            return GetProcess() != null;
        }

        public void Start()
        {
            if (!IsRunning())
            {
                using (var process = Process.Start(ComputeEmulatorPath, "/devfabric:start"))
                {
                    process.WaitForExit();
                }
            }
        }

        public void Stop()
        {
            if (IsRunning())
            {
                using (var process = Process.Start(ComputeEmulatorPath, "/devfabric:shutdown"))
                {
                    process.WaitForExit();
                }
            }
        }

        public void Kill()
        {
            var process = GetProcess();
            if (process != null)
            {
                process.Kill();
            }
        }
    }
}
