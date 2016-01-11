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
        private const string StorageEmulatorProcessName = "AzureStorageEmulator";
        private const string EmulatorPath = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe";

        public readonly string ConnectionString = "UseDevelopmentStorage=true";

        // Fiddler  
        //public readonly string ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://ipv4.fiddler";

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
            if (IsRunning())
            {
                return;
            }

            using (var process = Process.Start(EmulatorPath, "start"))
            {
                if (process != null)
                {
                    process.WaitForExit();
                }
            }
        }

        public void Stop()
        {
            if (!IsRunning())
            {
                return;
            }

            using (var process = Process.Start(EmulatorPath, "stop"))
            {
                if (process != null)
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
            if (IsRunning())
            {
                return;
            }

            using (var process = Process.Start(ComputeEmulatorPath, "/devfabric:start"))
            {
                if (process != null)
                {
                    process.WaitForExit();
                }
            }
        }

        public void Stop()
        {
            if (!IsRunning())
            {
                return;
            }

            using (var process = Process.Start(ComputeEmulatorPath, "/devfabric:shutdown"))
            {
                if (process != null)
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
