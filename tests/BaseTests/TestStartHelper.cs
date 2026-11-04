// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using BaseTests.Apps;
using Bodoconsult.App.Windows.DataProtection;
using System.Diagnostics;

namespace BaseTests;

/// <summary>
/// Helper class for test assembly starts
/// </summary>
public static class TestStartHelper
{
    private static readonly TestCredentialManager<TestCredentials> CredentialManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    static TestStartHelper()
    {
        CredentialManager = new TestCredentialManager<TestCredentials>("Bodoconsult.Finance.Tests");
        CredentialManager.LoadCredentials();
    }

    /// <summary>
    /// Load test credentials
    /// </summary>
    public static void LoadCredentials()
    {
        var globals = TestAppGlobals.Instance;
        ArgumentNullException.ThrowIfNull(CredentialManager.Credentials);
        globals.TestCredentials = CredentialManager.Credentials;
    }

    /// <summary>
    /// Current credentials for testing
    /// </summary>
    public static TestCredentials? Credentials => CredentialManager.Credentials;
}