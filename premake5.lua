solution "Radegast"
  configurations { "Debug", "Release" }
  platforms { "x86" }
  framework "4.0"
  language "C#"
  warnings "Extra"
  disablewarnings {"1591", "1574", "0419", "0618", "0414", "0169"}
  buildoptions { "/checked-", "/nostdlib-"}
  targetdir "bin"
  implibdir "bin"

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
    files {
      path.join("%{prj.location}", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
    }

  project "OpenMetaverse.StructuredData"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverse.StructuredData"))
    namespace("OpenMetaverse.StructuredData")
    files {
      path.join("%{prj.location}", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Xml",
      "OpenMetaverseTypes",
    }

  project "OpenMetaverse"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverse"))
    namespace("OpenMetaverse")
    files {
      path.join("%{prj.location}", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "log4net",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "SmartThreadPool",
      "XMLRPC",
      "zlib.net",
    }

  project "OpenMetaverse.Rendering.Meshmerizer"
    kind("SharedLib")
    location(path.join("libopenmetaverse", "OpenMetaverse.Rendering.Meshmerizer"))
    namespace("OpenMetaverse.Rendering")
    files {
      path.join("%{prj.location}", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "PrimMesher",
    }

  project "Radegast"
    kind("WindowedApp")
    location("Radegast")
    icon(path.join("%{prj.location}", "radegast.ico"))
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
      "System",
      "System.Core",
      "System.Data",
      "System.Drawing",
      "System.Web",
      "System.Windows.Forms",
      "System.Xml",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "OpenMetaverse.Rendering.Meshmerizer",
      path.join("%{prj.location}", "assemblies", "fmodex-dotnet"),
      path.join("%{prj.location}", "assemblies", "OpenTK"),
      path.join("%{prj.location}", "assemblies", "OpenTK.GLControl"),
      path.join("%{prj.location}", "assemblies", "Tools"),
      path.join("%{prj.location}", "assemblies", "log4net"),
      path.join("%{prj.location}", "assemblies", "CommandLine"),
      path.join("%{prj.location}", "assemblies", "XMLRPC"),
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
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs"),
      path.join("%{prj.location}", "aiml", "**"),
      path.join("%{prj.location}", "aiml_config", "**")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "System.Xml.Linq",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "System.Windows.Forms",
      path.join("%{prj.location}", "assemblies", "AIMLbot"),
      "Radegast.exe",
    }
    configuration "**/aiml/**"
      buildaction "Copy"
    configuration "**/aiml_config/**"
      buildaction "Copy"

  project "Radegast.Plugin.Demo"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Demo"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "System.Xml.Linq",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "System.Windows.Forms",
      "Radegast",
    }

  project "Radegast.Plugin.SimpleBuilder"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.SimpleBuilder"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "System.Windows.Forms",
      "Radegast",
    }
--[[
  project "Radegast.Plugin.EVOVend"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.EVOVend"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "System.Windows.Forms",
      "Radegast",
    }
--]]

  project "Radegast.Plugin.Speech"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeech"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "**.wav"),
      path.join("%{prj.location}", "**.resx"),
    }
    excludes {
      path.join("%{prj.location}", "obj", "**")
    }
    links {
      "OpenMetaverse",
      "OpenMetaverseTypes",
      "OpenMetaverse.StructuredData",
      "Radegast.exe",
      "fmodex-dotnet.dll",
      "System",
      "System.Core",
      "System.Drawing",
      "System.Windows.Forms",
      "System.Xml.Linq",
      "System.Data.DataSetExtensions",
      "System.Data",
      "System.Xml",
    }

  if os.is("linux") then
    project "RadSpeechLin"
      kind("SharedLib")
      location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechLin"))
      files {
        path.join("%{prj.location}", "**.cs"),
      }
      excludes {
        path.join("%{prj.location}", "obj", "**")
      }
      links {
        "OpenMetaverse",
        "OpenMetaverseTypes",
        "OpenMetaverse.StructuredData",
        "Radegast.exe",
        "Radegast.Plugin.Speech",
        "System",
        "System.Core",
        "System.Xml.Linq",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Xml",
      }
  end

  if os.is("windows") then
    project "RadSpeechWin"
      kind("SharedLib")
      location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechWin"))
      files {
        path.join("%{prj.location}", "**.cs"),
      }
      excludes {
        path.join("%{prj.location}", "obj", "**")
      }
      links {
        "OpenMetaverse",
        "OpenMetaverseTypes",
        "OpenMetaverse.StructuredData",
        "Radegast.exe",
        "Radegast.Plugin.Speech",
        "Radegast.exe",
        "System",
        "System.Core",
        "System.Speech",
        "System.Xml.Linq",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Xml",
      }
  end

  if os.is("macosx") then
    project "RadSpeechMac"
      kind("SharedLib")
      location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechMac"))
      files {
        path.join("%{prj.location}", "**.cs"),
      }
      excludes {
        path.join("%{prj.location}", "obj", "**")
      }
      links {
        "OpenMetaverse",
        "OpenMetaverseTypes",
        "OpenMetaverse.StructuredData",
        "Radegast.exe",
        "Radegast.Plugin.Speech",
        "Monobjc",
        "Monobjc.Cocoa",
        "Radegast.exe",
        "System",
        "System.Core",
        "System.Speech",
        "System.Xml.Linq",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Xml",
      }
  end
