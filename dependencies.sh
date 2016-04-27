#!/bin/sh
paket add nuget WixSharp project src/WixSharpExample/WixSharpExample.fsproj
paket add nuget WixSharp project src/WixSharpExample.CS/WixSharpExample.CS.csproj
# paket add reference System.Windows.Forms WixSharpExample.fsproj
# paket add reference System.Xml.Linq WixSharpExample.fsproj