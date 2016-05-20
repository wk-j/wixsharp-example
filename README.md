## สร้าง Windows Installer ด้วย WixSharp

## Wix Toolset

[Wix](http://wixtoolset.org/) เป็นเครื่องมือสำหรับสร้าง Windows installer สามารถใช้ติดตั้ง Windows app, Windows service หรือแม้แต่ IIS website
การสร้าง Installer ต้องเขียน Wix task นามสกุล .wxs (xml) แล้ว Build ด้วย MSBuild

สำหรับคนที่ไม่ชอบ Xml มีอีกทางเลือกคือใช้ WixSharp

[WixSharp](https://wixsharp.codeplex.com) เป็นไลบรารี่ที่ช่วยให้เขียน Wix task ด้วยภาษา .Net ที่เราถนัด เช่น VB.Net C# หรือ F# โดยตัว Library จะ Transpile โค้ดที่เราเขียนกลับไปเป็น .wsx อีกทีหนึ่ง

## ติดตั้ง Library

สามารถติดตั้งผ่าน Pacakge manager console หรือ Paket

```
Install-Package WixSharp
paket add nuget WixSharp MyApplication.Installer/MyApplication.Installer.fsproj
```

## สร้าง Solution

ใน Solution ประกอบด้วย 2 โปรเจค

```
src
├── MyApplication
│   ├── App.config
│   ├── AssemblyInfo.fs
│   ├── LICENSE.rtf
│   ├── MyApplication.fsproj
│   ├── Program.fs
│   └── packages.config
├── MyApplication.Installer
│   ├── App.config
│   ├── Main.fs
│   └── MyApplication.Installer.fsproj
└── MyApplication.sln
```

- MyApplication.fsproj เป็น โปรแกรม Command line
- MyApplication.Installer เป็นโปรเจคที่ติดตั้ง WixSharp เพื่อสร้าง Installer ให้โปรแกรม MyApplication

## รูปแบบ Installer ที่ต้องการสร้าง

- Bundle ไฟล์ทั้งหมดที่มีนามสกุล .exe .dll และ .config ของโปรแกรม MyApplication
- อนุญาติให้ผู้ใช้เปลี่ยน Installation path ได้ (default คือ %ProgramFiles%)
- ก่อนติดตั้งต้องแสดงหน้าจอ License ที่โหลดเนื้อหาจากไฟล์ LICENSE.rtf
- สร้าง Shortcuts MyApplication.exe ไว้ที่ Desktop

## Prerequirement

- ต้องติดตั้ง Wix toolset เวอร์ชั่น 3.10 (http://wixtoolset.org/releases)
- MSBuild 14.0 (มาพร้อมกับ Visual Studio)

## ไฟล์ MyApplication.Installer/Main.fs

Script installer ทั้งหมดจะเก็บไว้ในไฟล์ Main.fs

```fsharp
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
Status API Training Shop Blog About
```

## อธิบายโค้ดแต่ละส่วน

#### Bundle ไฟล์ทั้งหมดที่มีนามสกุล .exe .dll และ .config ของโปรแกรม MyApplication

```fsharp
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
```

#### อนุญาติให้ผู้ใช้เปลี่ยน Installation path ได้ (default คือ %ProgramFiles%)
#### ก่อนติดตั้งต้องแสดงหน้าจอ License ที่โหลดเนื้อหาจากไฟล์ LICENSE.rtf

```fsharp
let createProject config files =
    let dir = new Dir(config.InstallDir, files)
    let proj = Project(config.InstallerName, dir)
    proj.UI <- WUI.WixUI_InstallDir
    proj.LicenceFile <- System.IO.Path.Combine(config.AppDir, "LICENSE.rtf")
    (proj)
```

#### สร้าง Shortcuts MyApplication.exe ไว้ที่ Desktop

```fsharp
let createShortcut (exe : File) =
    exe.Shortcuts <- [| FileShortcut(exe.Id, "INSTALLDIR")
                        FileShortcut(exe.Id, "%Desktop%") |]
```

## Build

สามารถ Build ผ่าน Visual Studio (เลือก Configration เป็น Release) หรือผ่าน Command line โดยใช้คำสั่ง

```
msbuild MyApplication.sln /t:Build /p:Configuration=Release
```

## ผลลัพท์

หลักจากสั่ง Build จะได้ไฟล์

- `MyApplication.Installer\wix\MyApplication.Installer.g.wxs` เป็น Wix task ที่ถูก Generate จาก MSBuild
- `MyApplication.Installer\MyApplication.Installer.msi` เป็นไฟล์ Installer ที่ Bundle MyApplication สามารถนำไปตัดตั้งที่เครื่องใดก็ได้


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