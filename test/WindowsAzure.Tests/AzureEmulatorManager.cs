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
        private const string StorageEmulatorProcessNamev1 = "DSServiceLDB";
        private const string StorageEmulatorProcessNamev2 = "WAStorageEmulator";
        private const string EmulatorPathv1 = @"c:\program files\microsoft sdks\windows azure\emulator\csrun.exe";
        private const string EmulatorPathv2 = @"c:\program files (x86)\microsoft sdks\azure\storage emulator\wastorageemulator.exe";

        public readonly string ConnectionString = "UseDevelopmentStorage=true";

        // Fiddler  
        //public readonly string ConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://ipv4.fiddler";

        public static AzureStorageEmulatorManager Instance = new AzureStorageEmulatorManager();

        public Process GetProcess()
        {
            return Process.GetProcessesByName(StorageEmulatorProcessNamev1).FirstOrDefault()
                  ?? Process.GetProcessesByName(StorageEmulatorProcessNamev2).FirstOrDefault();
        }

        public bool IsRunning()
        {
            return GetProcess() != null;
        }

        public void Start()
        {
            if (!IsRunning())
            {
                try
                {
                    using (var process = Process.Start(EmulatorPathv1, "/devstore:start"))
                    {
                        process.WaitForExit();
                    }
                }
                catch
                {
                    using (var process = Process.Start(EmulatorPathv2, "start"))
                    {
                        process.WaitForExit();
                    }
                }
            }
        }

        public void Stop()
        {
            if (IsRunning())
            {
                try
                {
                    using (var process = Process.Start(EmulatorPathv1, "/devstore:shutdown"))
                    {
                        process.WaitForExit();
                    }
                }
                catch
                {
                    using (var process = Process.Start(EmulatorPathv2, "stop"))
                    {
                        process.WaitForExit();
                    }
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
