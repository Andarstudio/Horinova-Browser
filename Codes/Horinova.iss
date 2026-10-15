; Horinova installer - build with Inno Setup 6 (iscc installer\Horinova.iss)
; Expects the published app in ..\publish (dotnet publish ... -o publish)

#define MyAppName "Horinova"
#define MyAppVersion "1.0.0"
#define MyAppExeName "Horinova.exe"
#define MyAppPublisher "Andar Studio LLC)

[Setup]
AppId={{90134EA4-9B3B-4FF0-8126-6A171C67D83A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}
SetupIconFile=..\Assets\horinova.ico
OutputDir=..\installer-output
OutputBaseFilename=Horinova-Setup
WizardStyle=modern
WizardImageFile=logo.ico
WizardSmallImageFile=logo.ico
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
CloseApplications=yes

; 1. Run as administrator
PrivilegesRequired=admin

; 2. Welcome page
DisableWelcomePage=no

; 3. Privacy policy page (uses the license page)
LicenseFile=pp.txt

; 4. Install location page
DisableDirPage=no

; 5. Ready to install page, then installation progress
DisableReadyPage=no

; 6. Finish page
DisableFinishedPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
WelcomeLabel1=Welcome to the [name] Setup Wizard
WelcomeLabel2=This will install [name/ver] on your computer.%n%nIt is recommended that you close all other applications before continuing.
WizardLicense=Privacy Policy
LicenseLabel=Please read the following Privacy Policy before continuing.
LicenseLabel3=Please read the Privacy Policy. You must accept it to continue installing [name].
LicenseAccepted=I &accept the Privacy Policy
LicenseNotAccepted=I &do not accept the Privacy Policy
WizardSelectDir=Install Location
SelectDirLabel3=Setup will install [name] into the following folder.
ReadyLabel1=Setup is now ready to begin installing [name] on your computer.
FinishedHeadingLabel=Completing the [name] Setup Wizard
FinishedLabel=[name] has been installed on your computer. You can start it by selecting the shortcut.

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
