// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Xunit;

[assembly: ActiveIssue("https://github.com/dotnet/runtime/issues/34690", TestPlatforms.Windows, TargetFrameworkMonikers.Netcoreapp, TestRuntimes.Mono)]
[assembly: SkipOnPlatform(TestPlatforms.Browser, "System.Net.Security is not supported on Browser")]
