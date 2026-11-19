; LumaPlay 1.1.2 — Inno Setup 6
; El usuario recibe una app ya compilada; no compila código C# durante la instalación.

#define AppName "LumaPlay"
#define AppVersion "1.1.2"
#define AppPublisher "LumaPlay"

[Setup]
AppId={{B8E37F5B-7F9B-4B6D-A92A-1F7A0A5EA110}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\Programs\LumaPlay
DefaultGroupName=LumaPlay
DisableProgramGroupPage=yes
UsePreviousAppDir=yes
UsePreviousGroup=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
OutputDir=..\..\..\artifacts
OutputBaseFilename=LumaPlay_Setup
SetupIconFile=..\src\Assets\lumaplay.ico
UninstallDisplayIcon={app}\LumaPlay.exe
LicenseFile=..\LICENSE.txt
InfoAfterFile=..\LEEME.txt
WizardStyle=modern
WizardSizePercent=110
Compression=lzma2/ultra64
SolidCompression=yes
SetupLogging=yes
CloseApplications=yes
RestartApplications=no
Uninstallable=yes
ShowLanguageDialog=no

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el Escritorio"; GroupDescription: "Accesos directos:"; Flags: unchecked

[Files]
Source: "..\publish\LumaPlay.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LEEME.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\LumaPlay"; Filename: "{app}\LumaPlay.exe"; WorkingDir: "{app}"; IconFilename: "{app}\LumaPlay.exe"
Name: "{autodesktop}\LumaPlay"; Filename: "{app}\LumaPlay.exe"; WorkingDir: "{app}"; IconFilename: "{app}\LumaPlay.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\LumaPlay.exe"; Description: "Iniciar LumaPlay ahora"; Flags: postinstall nowait skipifsilent
