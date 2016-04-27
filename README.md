
## Issue

- The Wix element has an incorrect
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

- เปลี่ยนมาใช้ Toolset เวอร์ชั่น 3

```fsharp
Environment.SetEnvironmentVariable
    ("WIXSHARP_WIXDIR", sprintf "%s" @"C:\Program Files (x86)\WiX Toolset v3.10\bin",
     EnvironmentVariableTarget.Process)
```