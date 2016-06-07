#!/bin/bash

cd `dirname "$0"`
mkdir bin 2>/dev/null

if [ "$(uname -s)" == "Darwin" ]; then
  ./build/macosx/premake5 --os=macosx vs2010
else
  ./build/linux/premake5 --os=linux vs2010
fi

if [ x$1 == xbuild ]; then
    xbuild /p:Configuration=Release Radegast.sln
    RES=$?
    echo Build Exit Code: $RES

    if [ x$RES != x0 ]; then
	exit $RES
    fi

    if [ x$2 == xdist ]; then
        tar czvf radegast-latest.tgz bin
    fi

    exit $RES
else
    echo "Now run:"
    echo
    echo "xbuild Radegast.sln"
fi
