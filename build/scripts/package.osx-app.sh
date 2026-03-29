#!/usr/bin/env bash

set -e
set -o
set -u
set pipefail

cd build

mkdir -p GitStation.app/Contents/Resources
mv GitStation GitStation.app/Contents/MacOS
cp resources/app/App.icns GitStation.app/Contents/Resources/App.icns
sed "s/SOURCE_GIT_VERSION/$VERSION/g" resources/app/App.plist > GitStation.app/Contents/Info.plist
rm -rf GitStation.app/Contents/MacOS/GitStation.dsym
rm -f GitStation.app/Contents/MacOS/*.pdb

zip "gitstation_$VERSION.$RUNTIME.zip" -r GitStation.app
