// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;

namespace Bodoconsult.Finance.Payment.Banking.Test.Helpers;

internal class TestHelper
{
    /// <summary>
    /// Set a state message in the UI
    /// </summary>
    /// <param name="msg">Message to show</param>
    public static void SetState(string msg)
    {
        Debug.Print(msg);
    }
}