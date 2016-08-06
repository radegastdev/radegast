#!/bin/bash
echo '// Placeholder for automated build systems to implement their build number.' > Radegast/RadegastBuild.cs
echo 'namespace Radegast ' >> Radegast/RadegastBuild.cs
echo '{ ' >> Radegast/RadegastBuild.cs
echo '    public static class RadegastBuild ' >> Radegast/RadegastBuild.cs
echo '    {' >> Radegast/RadegastBuild.cs
echo '        public const string VersionString = "'$BUILD_MAJOR'.'$BUILD_MINOR'.'$TRAVIS_BUILD_NUMBER'";' >> Radegast/RadegastBuild.cs
echo '        public const string BuildName = "'$GIT_TAG'";' >> Radegast/RadegastBuild.cs
echo '    }' >> Radegast/RadegastBuild.cs
echo '}' >> Radegast/RadegastBuild.cs
./runprebuild.sh
