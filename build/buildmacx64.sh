dotnet publish ../libomt.sln -r osx-x64 -c Release
install_name_tool -id libomt.dylib ../bin/Release/net8.0/osx-x64/publish/libomt.dylib
install_name_tool -id libomt.dylib ../bin/Release/net8.0/osx-x64/native/libomt.dylib
