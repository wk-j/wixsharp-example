using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WixSharp;

namespace WixSharpExample.CS
{
    class Program
    {
        static public void Main(string[] args)
        {
            Environment.SetEnvironmentVariable
                ("WIXSHARP_WIXDIR", @"C:\Program Files (x86)\WiX Toolset v3.10\bin",
                 EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("WIXSHARP_DIR", Environment.CurrentDirectory, EnvironmentVariableTarget.Process);

            var project = new Project("MyApplication",
                              new Dir(@"%ProgramFiles%\MyApplication",
                                  new File("README.md")),

                              new SetPropertyAction("IDIR", "[INSTALLDIR]"),
                              new Property("IDIR", "empty"));

            project.UI = WUI.WixUI_InstallDir;

            Compiler.PreserveTempFiles = true;
            Compiler.BuildMsi(project);
        }
    }
}
