#!/bin/bash

cd `dirname "$0"`
mkdir bin 2>/dev/null
cp Radegast/assemblies/* bin

mono Radegast/prebuild.exe /target nant

cp -f NullBuild.txt plugins/Radegast.Plugin.Speech/RadSpeechWin/RadSpeechWin.dll.build
cp -f NullBuild.txt plugins/Radegast.Plugin.Speech/RadSpeechMac/RadSpeechMac.dll.build
cp -f NullBuild.txt plugins/Radegast.Plugin.Demo/Radegast.Plugin.Demo.dll.build

if [ x$1 == xnant ]; then
    nant -buildfile:Radegast.build
    RES=$?
    echo Build Exit Code: $RES

    if [ x$RES != x0 ]; then
	exit $RES
    fi

    if [ x$2 == xdist ]; then
        tar czvf radegast-latest.tgz bin
    fi
    
    exit $RES
fi
