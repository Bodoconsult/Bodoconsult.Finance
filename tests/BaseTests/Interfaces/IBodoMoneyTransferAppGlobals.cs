// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace BaseTests.Interfaces;

/// <summary>
/// Interface for finance specific app globals
/// </summary>
public interface ITestAppGlobals : IAppGlobals
{
    /// <summary>
    /// Credentials to use for unit tests
    /// </summary>
    public TestCredentials? TestCredentials { get; set; }
}