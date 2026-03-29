Name: gitstation
Version: %_version
Release: 1
Summary: Open-source & Free Git Gui Client
License: MIT
URL: https://albrtbc.github.io/
Source: https://github.com/albrtbc/gitstation/archive/refs/tags/v%_version.tar.gz
Requires: libX11.so.6()(%{__isa_bits}bit)
Requires: libSM.so.6()(%{__isa_bits}bit)
Requires: libicu
Requires: xdg-utils

%define _build_id_links none

%description
Open-source & Free Git Gui Client

%install
mkdir -p %{buildroot}/opt/gitstation
mkdir -p %{buildroot}/%{_bindir}
mkdir -p %{buildroot}/usr/share/applications
mkdir -p %{buildroot}/usr/share/icons
cp -f %{_topdir}/../../GitStation/* %{buildroot}/opt/gitstation/
ln -rsf %{buildroot}/opt/gitstation/gitstation %{buildroot}/%{_bindir}
cp -r %{_topdir}/../_common/applications %{buildroot}/%{_datadir}
cp -r %{_topdir}/../_common/icons %{buildroot}/%{_datadir}
chmod 755 -R %{buildroot}/opt/gitstation
chmod 755 %{buildroot}/%{_datadir}/applications/gitstation.desktop

%files
%dir /opt/gitstation/
/opt/gitstation/*
/usr/share/applications/gitstation.desktop
/usr/share/icons/*
%{_bindir}/gitstation

%changelog
# skip
