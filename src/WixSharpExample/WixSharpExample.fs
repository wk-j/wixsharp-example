open System
open WixSharp
open System.Linq
open Microsoft.Deployment.WindowsInstaller
open System.Windows.Forms
open System.IO

let build() = 
    Environment.SetEnvironmentVariable
        ("WIXSHARP_WIXDIR", sprintf "%s" @"C:\Program Files (x86)\WiX Toolset v3.10\bin", 
         EnvironmentVariableTarget.Process)
    Environment.SetEnvironmentVariable("WIXSHARP_DIR", Environment.CurrentDirectory, EnvironmentVariableTarget.Process)
    let installDir = "%ProgramFiles%\MyApplication"
    let appDir = sprintf @"Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\MyApplication\bin\Release"
    
    let getFile name = 
        let path = sprintf "%s\%s" appDir name
        File(path)
    
    let dir = 
        new Dir(installDir, 
                getFile ("MyApplication.exe.config"), 
                getFile ("MyApplication.exe"), 
                getFile ("FSharp.Core.dll"))
    let proj = 
        Project
            ("MyApplication", dir, 
                SetPropertyAction("IDIR", "[INSTALLDIR]"), 
                Property("IDIR", "empty"))

    Compiler.BuildMsi(proj)

[<EntryPoint>]
let main argv = 
    build() |> ignore
    (0)
