Remove-Item -Path build\GitStation\*.pdb -Force
Compress-Archive -Path build\GitStation -DestinationPath "build\gitstation_${env:VERSION}.${env:RUNTIME}.zip" -Force