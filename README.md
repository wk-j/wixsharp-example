
## Issue - The Wix element has an incorrect

- จะมีปัญหากับ Wix Toolset เวอร์ชั่น 4
- https://github.com/sbt/sbt-native-packager/issues/780

```
Build FAILED.

"Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpExample\WixSharpExample.fsproj" (Rebuild target) (1) ->
(MSIAuthoring target) ->
  Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpExample\WixSharpExample.wxs(2): error CNDL0199: The Wix element has an incorrect namespace of 'http://schem
as.microsoft.com/wix/2006/wi'.  Please make the Wix element look like the following: <Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">. [Z:\Source\csharp\wixsharp-setup
\WixSharpExample\src\WixSharpExample\WixSharpExample.fsproj]
  Z:\Source\csharp\wixsharp-setup\WixSharpExample\packages\WixSharp\build\WixSharp.targets(6,5): error MSB3073: The command ""Z:\Source\csharp\wixsharp-setup\WixSharpExa
mple\src\WixSharpExample\bin\Release\WixSharpExample.exe" "/MSBUILD:WixSharpExample"" exited with code -1. [Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpE
xample\WixSharpExample.fsproj]
```

- แก้ไขโดยเปลี่ยนมาใช้ Toolset เวอร์ชั่น 3

```fsharp
Environment.SetEnvironmentVariable
    ("WIXSHARP_WIXDIR", sprintf "%s" @"C:\Program Files (x86)\WiX Toolset v3.10\bin",
     EnvironmentVariableTarget.Process)
```

## Issue - [CustomAction] attribute but they don't meet the MakeSfxCA criteria.

- Error หลังจาก MSBuild พยายาม Excute `WixSharpExample.exe`

```
 "Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpExample\bin\Release\WixSharpExample.exe" "/MSBUILD:WixSharpExample"
EXEC : warning : some of the type members are marked with [CustomAction] attribute but they don't meet the MakeSfxCA criteria of being public static method of a public type: [Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpExample\WixSharpExample.fsproj]
    WixSharpExample+CustomActions.MyAction

  Searching for custom action entry points in WixSharpExample.exe
      Loaded dependent assembly: C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\FSharp.Core\v4.0_4.4.0.0__b03f5f7f11d50a3a\FSharp.Core.dll
      Loaded dependent assembly: C:\Program Files (x86)\WiX Toolset v3.10\sdk\Microsoft.Deployment.WindowsInstaller.dll
      MyAction=WixSharpExample!WixSharpExample+CustomActions.MyAction
  Searching for an embedded UI class in WixSharpExample.exe
  Modifying SfxCA.dll stub
  Copying file version info from Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpExample\WixSharpExample.exe to Z:\Source\csharp\wixsharp-setup\WixSharpExample
  \src\WixSharpExample\%this%.CA.dll
EXEC : error : System.IO.IOException: EnumResourceNames error. Error code: 1813 [Z:\Source\csharp\wixsharp-setup\WixSharpExample\src\WixSharpExample\WixSharpExample.fsproj
]
     at Microsoft.Deployment.Resources.ResourceCollection.Find(String resFile, ResourceType type)
     at Microsoft.Deployment.Tools.MakeSfxCA.MakeSfxCA.CopyVersionResource(String sourceFile, String destFile)
     at Microsoft.Deployment.Tools.MakeSfxCA.MakeSfxCA.Build(String output, String sfxdll, IList`1 inputs, TextWriter log)
     at Microsoft.Deployment.Tools.MakeSfxCA.MakeSfxCA.Main(String[] args)
  Wix# support for EmptyDirectories is automatically disabled
```

- CustomActions.MyAction จะมีปัญหาใน `F#` ปิด CustomAction ไปก่อนให้เหลือเฉพาะที่จำเป็น

```fsharp
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
```