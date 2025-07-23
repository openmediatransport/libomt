dotnet publish ../libomt.sln -r osx-arm64 -c Release
install_name_tool -id libomt.dylib ../bin/Release/net8.0/osx-arm64/publish/libomt.dylib
install_name_tool -id libomt.dylib ../bin/Release/net8.0/osx-arm64/native/libomt.dylib
