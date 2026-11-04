// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace FinanzDbUpdater;

internal class Program
{
    private static int Main(string[] args)
    {
        var starter = new FinanzDbUpdaterConsoleStarter(args);
        return starter.Start();
    }
}