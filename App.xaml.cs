using System.Diagnostics;
using Dynamsoft.License;

namespace BarcodeReader
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string licence = "DLS2eyJoYW5kc2hha2VDb2RlIjoiMTA0MDI1MTg1LVRYbFFjbTlxIiwibWFpblNlcnZlclVSTCI6Imh0dHBzOi8vbWRscy5keW5hbXNvZnRvbmxpbmUuY29tIiwib3JnYW5pemF0aW9uSUQiOiIxMDQwMjUxODUiLCJzdGFuZGJ5U2VydmVyVVJMIjoiaHR0cHM6Ly9zZGxzLmR5bmFtc29mdG9ubGluZS5jb20iLCJjaGVja0NvZGUiOjQxOTMxNTk2OX0=";

            string errorMsg;
            int errorCode = LicenseManager.InitLicense(licence, out errorMsg);
            if (errorCode != (int) Dynamsoft.Core.EnumErrorCode.EC_OK) Debug.WriteLine("Licence initialization error: " + errorMsg);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}