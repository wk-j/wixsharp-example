open System
open WixSharp

let build() = 
    Environment.SetEnvironmentVariable
        ("WIXSHARP_WIXDIR", sprintf "%s" @"C:\Program Files (x86)\WiX Toolset v3.10\bin", 
         EnvironmentVariableTarget.Process)
    Environment.SetEnvironmentVariable("WIXSHARP_DIR", Environment.CurrentDirectory, EnvironmentVariableTarget.Process)
    let proj = Project()
    proj.Name <- "MyApplication"
    proj.Dirs.Add(new Dir(@"src\MyApplication\bin\Release", File("MyApplication.exe"))) |> ignore
        //Project("WixSharpExample", Dir(@"src\WixSharpExample\MyApplication\bin\Release"))
    Compiler.BuildMsi(proj)

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    build() |> ignore
    0 // return an integer exit code
