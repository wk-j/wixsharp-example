open System
open WixSharp
open System.Linq

type Config = 
    { InstallDir : string
      AppDir : string
      InstallerName : string
      ToolPath : string }

let filterFile root pattern = 
    System.IO.DirectoryInfo(root).GetFiles(pattern).Select(fun x -> x.FullName |> File)
          .Select(fun x -> upcast x : WixEntity)

let createShortcut (exe : File) = 
    exe.Shortcuts <- [| FileShortcut(exe.Id, "INSTALLDIR")
                        FileShortcut(exe.Id, "%Desktop%") |]

let createProject config files = 
    let dir = new Dir(config.InstallDir, files)
    let proj = Project(config.InstallerName, dir)
    proj.UI <- WUI.WixUI_InstallDir
    proj.LicenceFile <- System.IO.Path.Combine(config.AppDir, "LICENSE.rtf")
    (proj)

let build() = 
    let config = 
        { InstallDir = "%ProgramFiles%\MyApplication"
          AppDir = @"Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\MyApplication\bin\Release"
          InstallerName = "MyApplication.Installer"
          ToolPath = @"C:\Program Files (x86)\WiX Toolset v3.10\bin" }

    Environment.SetEnvironmentVariable("WIXSHARP_WIXDIR", config.ToolPath, EnvironmentVariableTarget.Process)

    let flatten input = 
        seq { 
            for file in input do
                yield! file
        }
        |> Seq.toArray
    
    let filter = filterFile config.AppDir
    
    let interestedFiles = 
        [| filter "*.exe"
           filter "*.dll"
           filter "*.config" |]
        |> flatten
    
    let proj = createProject config interestedFiles
    proj.AllFiles.Where(fun x -> x.Id = "MyApplication.exe").First() |> createShortcut
    Compiler.BuildMsi(proj)

[<EntryPoint>]
let main argv = 
    build() |> ignore
    (0)
