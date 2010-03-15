#!/bin/bash

cd `dirname "$0"`
mkdir bin 2>/dev/null
cp Radegast/assemblies/* bin

mono Radegast/Prebuild.exe /target nant

cp -f NullBuild.txt plugins/Radegast.Plugin.Speech/RadSpeechWin/RadSpeechWin.dll.build
cp -f NullBuild.txt plugins/Radegast.Plugin.Demo/Radegast.Plugin.Demo.dll.build

if [ x$1 == xnant ]; then
    nant -buildfile:Radegast.build
    RES=$?
    echo Build Exit Code: $RES
    if [ x$2 == xruntests ]; then
        nunit-console2 bin/Radegast.Tests.dll /exclude=Network /labels /xml=testresults.xml
    fi
    
    exit $RES
fi
