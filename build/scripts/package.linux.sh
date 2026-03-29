#!/usr/bin/env bash

set -e
set -o
set -u
set pipefail

arch=
appimage_arch=
target=
case "$RUNTIME" in
    linux-x64)
        arch=amd64
        appimage_arch=x86_64
        target=x86_64;;
    linux-arm64)
        arch=arm64
        appimage_arch=arm_aarch64
        target=aarch64;;
    *)
        echo "Unknown runtime $RUNTIME"
        exit 1;;
esac

APPIMAGETOOL_URL=https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage

cd build

if [[ ! -f "appimagetool" ]]; then
    curl -o appimagetool -L "$APPIMAGETOOL_URL"
    chmod +x appimagetool
fi

rm -f GitStation/*.dbg
rm -f GitStation/*.pdb

mkdir -p GitStation.AppDir/opt
mkdir -p GitStation.AppDir/usr/share/metainfo
mkdir -p GitStation.AppDir/usr/share/applications

cp -r GitStation GitStation.AppDir/opt/gitstation
desktop-file-install resources/_common/applications/gitstation.desktop --dir GitStation.AppDir/usr/share/applications \
    --set-icon com.albrtbc.GitStation --set-key=Exec --set-value=AppRun
mv GitStation.AppDir/usr/share/applications/{sourcegit,com.albrtbc.GitStation}.desktop
cp resources/_common/icons/gitstation.png GitStation.AppDir/com.albrtbc.GitStation.png
ln -rsf GitStation.AppDir/opt/gitstation/gitstation GitStation.AppDir/AppRun
ln -rsf GitStation.AppDir/usr/share/applications/com.albrtbc.GitStation.desktop GitStation.AppDir
cp resources/appimage/gitstation.appdata.xml GitStation.AppDir/usr/share/metainfo/com.albrtbc.GitStation.appdata.xml

ARCH="$appimage_arch" ./appimagetool -v GitStation.AppDir "gitstation-$VERSION.linux.$arch.AppImage"

mkdir -p resources/deb/opt/gitstation/
mkdir -p resources/deb/usr/bin
mkdir -p resources/deb/usr/share/applications
mkdir -p resources/deb/usr/share/icons
cp -f GitStation/* resources/deb/opt/gitstation
ln -rsf resources/deb/opt/gitstation/gitstation resources/deb/usr/bin
cp -r resources/_common/applications resources/deb/usr/share
cp -r resources/_common/icons resources/deb/usr/share
# Calculate installed size in KB
installed_size=$(du -sk resources/deb | cut -f1)
# Update the control file
sed -i -e "s/^Version:.*/Version: $VERSION/" \
    -e "s/^Architecture:.*/Architecture: $arch/" \
    -e "s/^Installed-Size:.*/Installed-Size: $installed_size/" \
    resources/deb/DEBIAN/control
# Build deb package with gzip compression
dpkg-deb -Zgzip --root-owner-group --build resources/deb "gitstation_$VERSION-1_$arch.deb"

rpmbuild -bb --target="$target" resources/rpm/SPECS/build.spec --define "_topdir $(pwd)/resources/rpm" --define "_version $VERSION"
mv "resources/rpm/RPMS/$target/gitstation-$VERSION-1.$target.rpm" ./
