solution "Radegast"
  configurations { "Debug", "Release" }
  platforms { "x86" }
  framework "4.0"
  language "C#"
  warnings "Extra"
  disablewarnings {"1591", "1574", "0419", "0618", "0414", "0169"}
  buildoptions { "/checked-", "/nostdlib-"}
  targetdir "bin"
  links {
    "System",
    "System.Core",
    "System.Xml"
  }

  configuration "Debug"
    defines { "TRACE", "DEBUG", "SMARTHREADPOOL_REF" }
    flags { "Symbols" }
    optimize "Off"
    clr "Unsafe"

  configuration "Release"
    defines { "TRACE", "SMARTHREADPOOL_REF" }
    optimize "Full"
    clr "Unsafe"

  configuration "**.png"
    buildaction "Copy"
  configuration "**.config"
    buildaction "Copy"
  configuration "**.so"
    buildaction "Copy"
  configuration "**.xml"
    buildaction "Copy"
  configuration "**.txt"
    buildaction "Copy"
  configuration "**.png"
    buildaction "Copy"
  configuration "**.wav"
    buildaction "Copy"

  project "OpenMetaverseTypes"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverseTypes"))
    namespace("OpenMetaverse")
    implibdir(path.join("..", "bin"))
    targetdir(path.join("..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs")
    }

  project "OpenMetaverse.StructuredData"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverse.StructuredData"))
    namespace("OpenMetaverse.StructuredData")
    implibdir(path.join("..", "bin"))
    targetdir(path.join("..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs")
    }
    links {
      "OpenMetaverseTypes"
    }

  project "OpenMetaverse"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverse"))
    namespace("OpenMetaverse")
    implibdir(path.join("..", "bin"))
    targetdir(path.join("..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs")
    }
    links {
      "System.Data",
      "System.Drawing",
      "log4net",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "SmartThreadPool",
      "XMLRPC",
      "zlib.net"
    }

  project "OpenMetaverse.Rendering.Meshmerizer"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverse.Rendering.Meshmerizer"))
    namespace("OpenMetaverse.Rendering")
    implibdir(path.join("..", "bin"))
    targetdir(path.join("..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs")
    }
    links {
      "System.Data",
      "System.Drawing",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "PrimMesher"
    }

  project "Radegast"
    kind("WindowedApp")
    location("Radegast")
    targetdir(path.join("..", "bin"))
    files {
      path.join("%{prj.location}", "Properties", "**.cs"),
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "**.resx"),
      path.join("%{prj.location}", "openjpeg-dotnet*"),
      path.join("%{prj.location}", "*.config"),
      path.join("%{prj.location}", "*.so"),
      path.join("%{prj.location}", "*.dylib"),
      path.join("%{prj.location}", "fmodex.*"),
      path.join("%{prj.location}", "*.xml"),
      path.join("%{prj.location}", "*.txt"),
      path.join("%{prj.location}", "*.png"),
      path.join("%{prj.location}", "radegast.nsi"),
      path.join("%{prj.location}", "openmetaverse_data", "**"),
      path.join("%{prj.location}", "character", "**"),
      path.join("%{prj.location}", "shader_data", "**")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
      path.join("%{prj.location}", "openmetaverse_data", ".svn", "**"),
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System.Data",
      "System.Drawing",
      "System.Web",
      "System.Windows.Forms",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "OpenMetaverse.Rendering.Meshmerizer",
      "fmodex-dotnet",
      "OpenTK",
      "OpenTK.GLControl",
      "Tools",
      "log4net",
      "CommandLine",
      "XMLRPC",
    }
    configuration "**/**.dylib"
      buildaction "Copy"
    configuration "**/fmodex.*"
      buildaction "Copy"
    configuration "**/openjpeg-dotnet*"
      buildaction "Copy"
    configuration "**/radegast.nsi"
      buildaction "Copy"
    configuration "**/openmetaverse_data/**"
      buildaction "Copy"
    configuration "**/character/**"
      buildaction "Copy"
    configuration "**/shader_data/**"
      buildaction "Copy"

  project "Radegast.Plugin.Alice"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Alice"))
    implibdir(path.join("..", "..", "bin"))
    targetdir(path.join("..", "..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs") ,
      path.join("%{prj.location}", "aiml", "**") ,
      path.join("%{prj.location}", "aiml_config", "**")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
    }
    links {
      "System.Xml.Linq",
      "System.Data",
      "System.Drawing",
      "System.Windows.Forms",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "AIMLbot",
      "Radegast.exe"
    }
    configuration "**/aiml/**"
      buildaction "Copy"
    configuration "**/aiml_config/**"
      buildaction "Copy"

  project "Radegast.Plugin.Demo"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Demo"))
    implibdir(path.join("..", "..", "bin"))
    targetdir(path.join("..", "..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
    }
    links {
      "System.Xml.Linq",
      "System.Data",
      "System.Drawing",
      "System.Windows.Forms",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "Radegast.exe"
    }

  project "Radegast.Plugin.Speech"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeech"))
    implibdir(path.join("..", "..", "..", "bin"))
    targetdir(path.join("..", "..", "..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "**.wav"),
      path.join("%{prj.location}", "**.resx"),
    }
    links {
      "System.Xml.Linq",
      "System.Data",
      "System.Data.DataSetExtensions",
      "System.Drawing",
      "System.Windows.Forms",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "Radegast.exe",
      "fmodex-dotnet.dll",
    }

  project "RadSpeechLin"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechLin"))
    implibdir(path.join("..", "..", "..", "bin"))
    targetdir(path.join("..", "..", "..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs"),
    }
    links {
      "System.Xml.Linq",
      "System.Data",
      "System.Data.DataSetExtensions",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "Radegast.exe",
      "Radegast.Plugin.Speech",
    }

  project "RadSpeechWin"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechWin"))
    implibdir(path.join("..", "..", "..", "bin"))
    targetdir(path.join("..", "..", "..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs"),
    }
    links {
      "System.Xml.Linq",
      "System.Data",
      "System.Data.DataSetExtensions",
      "System.Speech",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "Radegast.exe",
      "Radegast.Plugin.Speech",
    }

  project "RadSpeechMac"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechMac"))
    implibdir(path.join("..", "..", "..", "bin"))
    targetdir(path.join("..", "..", "..", "bin"))
    files {
      path.join("%{prj.location}", "**.cs"),
    }
    links {
      "System.Xml.Linq",
      "System.Data",
      "System.Data.DataSetExtensions",
      "System.Speech",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "Radegast.exe",
      "Radegast.Plugin.Speech",
      "Monobjc",
      "Monobjc.Cocoa"
    }
