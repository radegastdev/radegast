solution "Radegast"
  configurations { "Debug", "Release" }
  platforms { "x86" }
  dotnetframework "4.5"
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

  project "Radegast"
    kind("WindowedApp")
    location("Radegast")
    icon(path.join("%{prj.location}", "radegast.ico"))
    files {
      path.join("%{prj.location}", "Properties", "**.cs"),
	  path.join("%{prj.location}", "FMOD", "*.cs"),
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "**.resx"),
      path.join("%{prj.location}", "openjpeg-dotnet*"),
      path.join("%{prj.location}", "*.config"),
      path.join("%{prj.location}", "*.so"),
      path.join("%{prj.location}", "*.dylib"),
      path.join("%{prj.location}", "fmod*.*"),
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
    nuget {
      "LibreMetaverse",
      "LibreMetaverse.Types",
      "LibreMetaverse.StructuredData",
      "LibreMetaverse.Rendering.Meshmerizer"
    }
    links {
      "System",
      "System.Core",
      "System.Data",
      "System.Drawing",
      "System.Web",
      "System.Web.Extensions",
      "System.Windows.Forms",
      "System.Xml",
      path.join("%{prj.location}", "assemblies", "OpenTK"),
      path.join("%{prj.location}", "assemblies", "OpenTK.GLControl"),
      path.join("%{prj.location}", "assemblies", "Tools"),
      path.join("%{prj.location}", "assemblies", "log4net"),
      path.join("%{prj.location}", "assemblies", "CommandLine"),
      path.join("%{prj.location}", "assemblies", "XMLRPC"),
    }
    configuration "**/**.dylib"
      buildaction "Copy"
    configuration "**/fmod*.*"
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
    nuget {
      "LibreMetaverse",
      "LibreMetaverse.Types",
      "LibreMetaverse.StructuredData"
    }
    dependson {
      "Radegast",
      "Radegast.Plugin.Speech"
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "System.Xml.Linq",
      "System.Windows.Forms",
      "Radegast.exe",
      path.join("%{prj.location}", "assemblies", "AIMLbot"),
    }
    configuration "**/aiml/**"
      buildaction "Copy"
    configuration "**/aiml_config/**"
      buildaction "Copy"

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
    nuget {
      "LibreMetaverse",
      "LibreMetaverse.Types",
      "LibreMetaverse.StructuredData"
    }
    dependson{
      "Radegast",
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "System.Windows.Forms",
      "Radegast",
    }

  project "Radegast.Plugin.IRC"
    kind("SharedLib")
    location(path.join("plugins", "Radegast.Plugin.IRC"))
    files {
      path.join("%{prj.location}", "**.cs"),
      path.join("%{prj.location}", "Properties", "**.cs")
    }
    excludes {
      path.join("%{prj.location}", "AssemblyInfo.cs"),
      path.join("%{prj.location}", "obj", "**")
    }
    nuget {
      "LibreMetaverse",
      "LibreMetaverse.Types",
      "LibreMetaverse.StructuredData"
    }
    dependson{
      "Radegast",
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
      "System.Windows.Forms",
      "System.Xml.Linq",
      "System.Data.DataSetExtensions",
      "Radegast",
      path.join("%{prj.location}", "assemblies", "Meebey.SmartIrc4net")
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
    nuget {
      "LibreMetaverse",
      "LibreMetaverse.Types",
      "LibreMetaverse.StructuredData"
    }
    dependson{
      "Radegast",
    }
    links {
      "System",
      "System.Core",
      "System.Xml",
      "System.Data",
      "System.Drawing",
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
    nuget {
      "LibreMetaverse",
      "LibreMetaverse.Types",
      "LibreMetaverse.StructuredData"
    }
    dependson{
      "Radegast",
    }
    links {
      "System",
      "System.Core",
      "System.Drawing",
      "System.Windows.Forms",
      "System.Xml.Linq",
      "System.Data.DataSetExtensions",
      "System.Data",
      "System.Xml",
      "Radegast.exe",
    }

    project "RadSpeechLin"
      kind("SharedLib")
      location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechLin"))
      files {
        path.join("%{prj.location}", "**.cs"),
      }
      excludes {
        path.join("%{prj.location}", "obj", "**")
      }
      nuget {
          "LibreMetaverse",
          "LibreMetaverse.Types",
          "LibreMetaverse.StructuredData"
      }
      dependson {
        "Radegast",
        "Radegast.Plugin.Speech"
      }
      links {
        "System",
        "System.Core",
        "System.Xml.Linq",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Xml",
        "Radegast.exe",
        "Radegast.Plugin.Speech",
      }

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
      nuget {
          "LibreMetaverse",
          "LibreMetaverse.Types",
          "LibreMetaverse.StructuredData"
      }
      dependson {
        "Radegast",
        "Radegast.Plugin.Speech"
      }
      links {
        "System",
        "System.Core",
        "System.Speech",
        "System.Xml.Linq",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Xml",
        "Radegast.exe",
        "Radegast.Plugin.Speech",
      }
  end

    project "RadSpeechMac"
      kind("SharedLib")
      location(path.join("plugins", "Radegast.Plugin.Speech", "RadSpeechMac"))
      files {
        path.join("%{prj.location}", "**.cs"),
      }
      excludes {
        path.join("%{prj.location}", "obj", "**")
      }
      nuget {
          "LibreMetaverse",
          "LibreMetaverse.Types",
          "LibreMetaverse.StructuredData"
      }
      dependson {
        "Radegast",
        "Radegast.Plugin.Speech"
      }
      links {
        "System",
        "System.Core",
        "System.Xml.Linq",
        "System.Data.DataSetExtensions",
        "System.Data",
        "System.Xml",
        "Radegast.exe",
        "Radegast.Plugin.Speech",
        path.join("%{prj.location}", "assemblies", "Monobjc.Cocoa"),
        path.join("%{prj.location}", "assemblies", "Monobjc"),
      }
