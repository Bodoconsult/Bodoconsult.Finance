// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;

namespace BodoMoneyTransfer.Dal.App;

using Bodoconsult.App.DataProtection;

/// <summary>
/// Helper class for basic app features
/// </summary>
public static class AppHelper
{
    /// <summary>
    /// Load password data
    /// </summary>
    public static void LoadPasswordData()
    {
        PasswordHandler.Key1 = "2xos4FvahJ7aW24q74xpRid9+ZMsGk/VjMq5hb/SDC0=";
        PasswordHandler.Key2 = "X6AuNCO/w6yZ5rKFlG1KlXdIC2VMnS6Wbn0Z6HcRJTc=";
        PasswordHandler.Key3 = "x3StL+k1pNR1fBUGkxRiWfJxGiJTe6O7ujcHEvXauYA=";
        PasswordHandler.Salt = "mm6T8ciCg1l5FHq8rYjUxQ==";
    }

    /// <summary>
    /// Get the product version out of the assemblies file version
    /// </summary>
    /// <returns>Product version string</returns>
    public static string GetProductVersion()
    {
        // Set product version
        var assembly = Assembly.GetExecutingAssembly();
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

        if (fvi.FileVersion is null)
        {
            return "1.0";
        }

        var versiondata = fvi.FileVersion.Split(["."], StringSplitOptions.RemoveEmptyEntries);

        return $"{versiondata[0]}.{versiondata[1]}";
    }
}