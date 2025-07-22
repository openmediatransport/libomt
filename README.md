# libomt

libomt is a C wrapper for the libomnet library.

libomt uses the .NET Native AOT Deployment to build a native shared library with C exports

## Requirements

.NET 8.0 or higher for Windows, Linux or MacOS
libvmx shared library

## Compiling

1. Setup a folder structure with both libomtnet and libomt as follows
[root]
/libomtnet
/libomt
2. The build process will reference and build the .NET Standard library in the ../libomtnet folder
3. Run the build script for the desired platform from the build directory.
4. A shared library and a .lib file should be generated in the platform native directory.
On Windows this is usually bin\Release\net8.0\win-x64\native

## Using the library in your project

1. Include the libomt.h file in your C/C++ application
2. Add the libomt.lib library

Note: libomtnet.dll is not needed as it is automatically built into the libomt shared library.