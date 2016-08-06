#!/bin/bash
echo '// Placeholder for automated build systems to implement their build number.' > RadegastBuild.cs
echo 'namespace Radegast ' >> RadegastBuild.cs
echo '{ ' >> RadegastBuild.cs
echo '    public static class RadegastBuild ' >> RadegastBuild.cs
echo '    {' >> RadegastBuild.cs
echo '        public const string VersionString = "'$BUILD_MAJOR'.'$BUILD_MINOR'.'$TRAVIS_BUILD_NUMBER'";' >> RadegastBuild.cs
echo '        public const string BuildName = "'$GIT_TAG'";' >> RadegastBuild.cs
echo '    }' >> RadegastBuild.cs
echo '}' >> RadegastBuild.cs
./runprebuild.sh
