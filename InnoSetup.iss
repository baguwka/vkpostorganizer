#define AppName "Vk Postpone Helper"
#define ExeName "VkPostOrganizer.exe"
#define Version "1.2.1.1213"

[Setup]
AppName={#AppName}
AppVersion={#Version}
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
UninstallDisplayIcon={app}\{#ExeName}
Compression=lzma2
SolidCompression=yes
OutputDir=R:\Projects\VK Post Organizer\VK Post Organizer\bin\Setup
OutputBaseFilename=Setup {#AppName} {#Version}

[Files]
Source: "VK Post Organizer\bin\Release\Data Persistence Provider.dll"; DestDir: "{app}"      
Source: "VK Post Organizer\bin\Release\GongSolutions.Wpf.DragDrop.dll"; DestDir: "{app}"       
Source: "VK Post Organizer\bin\Release\GongSolutions.Wpf.DragDrop.pdb"; DestDir: "{app}"         
Source: "VK Post Organizer\bin\Release\GongSolutions.Wpf.DragDrop.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\JetBrains.Annotations.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\JetBrains.Annotations.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"            
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.ServiceLocation.pdb"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.ServiceLocation.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.Unity.Configuration.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.Unity.Configuration.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.Unity.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.Unity.RegistrationByConvention.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.Unity.RegistrationByConvention.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Microsoft.Practices.Unity.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Newtonsoft.Json.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\NLog.config"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\NLog.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\NLog.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Prism.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Prism.Unity.Wpf.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Prism.Unity.Wpf.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Prism.Wpf.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Prism.Wpf.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Prism.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\RateLimiter.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\SimpleInjector.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\SimpleInjector.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\System.Net.Http.Formatting.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\System.Net.Http.Formatting.xml"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"         
Source: "VK Post Organizer\bin\Release\VkPostOrganizer.exe"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\VkPostOrganizer.exe.config"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\VkPostOrganizer.pdb"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Xceed.Wpf.AvalonDock.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Xceed.Wpf.AvalonDock.Themes.Aero.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Xceed.Wpf.AvalonDock.Themes.Metro.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Xceed.Wpf.AvalonDock.Themes.VS2010.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Xceed.Wpf.DataGrid.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\Xceed.Wpf.Toolkit.dll"; DestDir: "{app}"
Source: "VK Post Organizer\bin\Release\ru\Xceed.Wpf.AvalonDock.resources.dll"; DestDir: "{app}\ru"


[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#ExeName}"
