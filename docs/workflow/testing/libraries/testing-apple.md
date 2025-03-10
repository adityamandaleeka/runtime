# Testing Libraries on iOS, tvOS, and MacCatalyst

## Prerequisites

- Xcode 11.3 or higher
- a certificate and provisioning profile if using a device
- a simulator with a proper device type and OS version.
Go `Xcode > Window > Devices and Simulators` to revise the list of the available simulators and then `"+" button on bottom left > OS Version dropdown selection > Download more simulator runtimes` in case you need to download more simulators.

## Building Libs and Tests

You can build and run the library tests:
- on a simulator;
- on a device.

Run the following command in a terminal:
```
./build.sh mono+libs -os <TARGET_OS> -arch <TARGET_ARCHITECTURE>
```
where `<TARGET_OS>` is one of the following:
- iossimulator
- tvossimulator
- maccatalyst
- ios
- tvos

and `<TARGET_ARCHITECTURE>` is one of the following:
- x64
- arm64 (for device)

e.g., to build for an iOS simulator, run:
```
./build.sh mono+libs -os iossimulator -arch x64
```

Run tests one by one for each test suite on a simulator:
```
./build.sh libs.tests -os iossimulator -arch x64 -test
```

### Building for a device

In order to run the tests on a device:
- Set the `-os` parameter to a device-related value (see above)
- Specify `DevTeamProvisioning` (see [developer.apple.com/account/#/membership](https://developer.apple.com/account/#/membership), scroll down to `Team ID`).

For example:
```
./build.sh libs.tests -os ios -arch x64 -test /p:DevTeamProvisioning=H1A2B3C4D5
```
Other possible options are:
- to sign with an adhoc key by setting `/p:DevTeamProvisioning=adhoc`
- to skip signing all together by setting `/p:DevTeamProvisioning=-` .

[AppleAppBuilder](https://github.com/dotnet/runtime/blob/main/src/tasks/AppleAppBuilder/AppleAppBuilder.cs) generates temp Xcode projects you can manually open and resolve provisioning issues there using native UI and deploy to your devices.

### Running individual test suites

- The following shows how to run tests for a specific library:
```
./dotnet.sh build src/libraries/System.Numerics.Vectors/tests /t:Test /p:TargetOS=ios /p:TargetArchitecture=x64
```

Also you can run the built test app through Xcode by opening the corresponding `.xcodeproj` and setting up the right scheme, app, and even signing if using a local device.

There's also an option to run a `.app` through `xcrun`, which is simulator specific. Consider the following shell script:

```
    xcrun simctl shutdown <IOSSIMULATOR_NAME>
    xcrun simctl boot <IOSSIMULATOR_NAME>
    open -a Simulator
    xcrun simctl install <IOSSIMULATOR_NAME> <APP_BUNDLE_PATH>
    xcrun simctl launch --console booted <IOS_APP_NAME>
```

where
`<IOSSIMULATOR_NAME>` is a name of the simulator to start, e.g. `"iPhone 11"`,
`<APP_BUNDLE_PATH>` is a path to the iOS test app bundle,
`<IOS_APP_NAME>` is a name of the iOS test app

### Running the functional tests

There are [functional tests](https://github.com/dotnet/runtime/tree/main/src/tests/FunctionalTests/) which aim to test some specific features/configurations/modes on a target mobile platform.

A functional test can be run the same way as any library test suite, e.g.:
```
./dotnet.sh build /t:Test -c Release /p:TargetOS=iossimulator /p:TargetArchitecture=x64 src/tests/FunctionalTests/iOS/Simulator/PInvoke/iOS.Simulator.PInvoke.Test.csproj
```

Currently functional tests are expected to return `42` as a success code so please be careful when adding a new one.

### Running the runtime tests

Currently, only the `tracing/eventpipe` subset of runtime tests is enabled on iOS platforms.

The subset of runtime tests can be built by executing the following shell script:
```sh
./build.sh -arch arm64 -os ios -s mono+libs -c Release
./src/tests/build.sh os ios arm64 Release -mono tree tracing/eventpipe /p:LibrariesConfiguration=Release
```

The script generates an Apple bundle that can be executed using Xcode or XHarness.

### Viewing logs
- see the logs generated by the XHarness tool
- use the `Console` app on macOS:
`Command + Space`, type in `Console`, search the appropriate process (System.Buffers.Tests for example).

### Testing various configurations

It's possible to test various configurations by setting a combination of additional MSBuild properties such as `RunAOTCompilation`,`MonoEnableInterpreter`, and some more.

1. Interpreter Only

This configuration is necessary for hot reload scenarios.

To enable the interpreter, add `/p:RunAOTCompilation=true /p:MonoEnableInterpreter=true` to a build command.

2. AOT only

To build for AOT only mode, add `/p:RunAOTCompilation=true /p:MonoEnableInterpreter=false` to a build command.

3. AOT-LLVM

To build for AOT-LLVM mode, add `/p:RunAOTCompilation=true /p:MonoEnableInterpreter=false /p:MonoEnableLLVM=true` to a build command.

4. App Sandbox

To build the test app bundle with the App Sandbox entitlement, add `/p:EnableAppSandbox=true` to a build command.

### Test App Design
iOS/tvOS `*.app` (or `*.ipa`) is basically a simple [ObjC app](https://github.com/dotnet/runtime/blob/main/src/tasks/AppleAppBuilder/Templates/main-console.m) that inits the Mono Runtime. This Mono Runtime starts a simple xunit test
runner called XHarness.TestRunner (see https://github.com/dotnet/xharness) which runs tests for all `*.Tests.dll` libs in the bundle. There is also XHarness.CLI tool to deploy `*.app` and `*.ipa` to a target (device or simulator) and listens for logs via network sockets.

### Existing Limitations
- Simulator uses JIT mode only at the moment (to be extended with FullAOT and Interpreter)
- Interpreter is not enabled yet.
