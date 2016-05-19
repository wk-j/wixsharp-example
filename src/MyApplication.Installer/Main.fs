open System
open WixSharp
open System.Linq

let installDir = "%ProgramFiles%\MyApplication"
let appDir = @"Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\MyApplication\bin\Release"
//let appDir = @"..\MyApplication\bin\Release"
let installerName = "MyApplication.Installer"
let toolPath = @"C:\Program Files (x86)\WiX Toolset v3.10\bin"
let filterFile root pattern = 
    System.IO.DirectoryInfo(root).GetFiles(pattern).Select(fun x -> x.FullName |> File)
          .Select(fun x -> upcast x : WixEntity)

let build() = 
    Environment.SetEnvironmentVariable("WIXSHARP_WIXDIR", toolPath, EnvironmentVariableTarget.Process)
    let flatten input = 
        seq { 
            for file in input do
                yield! file
        }
        |> Seq.toArray
    
    let filter = filterFile appDir
    
    let interestedFiles = 
        [| filter "*.exe"
           filter "*.dll"
           filter "*.config" |]
        |> flatten
    
    let createShortcut (exe : File) = 
        exe.Shortcuts <- [| FileShortcut(exe.Id, "INSTALLDIR")
                            FileShortcut(exe.Id, "%Desktop%") |]
    
    let createProject() = 
        let dir = new Dir(installDir, interestedFiles)
        let proj = Project(installerName, dir)
        proj.UI <- WUI.WixUI_InstallDir
        proj.LicenceFile <- System.IO.Path.Combine(appDir, "LICENSE.rtf")
        (proj)
    
    let proj = createProject()
    proj.AllFiles.Where(fun x -> x.Id = "MyApplication.exe").First() |> createShortcut
    Compiler.BuildMsi(proj)

[<EntryPoint>]
let main argv = 
    build() |> ignore
    (0)
