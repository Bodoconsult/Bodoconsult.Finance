// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using NUnit.Framework;

namespace FinanzDbUpdater.Bll.Test;

[TestFixture]
public class FinanzDbUpdaterConsoleStarterTests
{
    //[SetUp]
    //public void Setup()
    //{
    //}

    [Test]
    public void Start_NoCommandLineArgs_RunsSuccessful()
    {
        // Arrange
        var starter = new FinanzDbUpdaterConsoleStarter([]);

        // Act
        var result =  starter.Start();

        // Assert
        Assert.That(result, Is.Zero);
    }

    [Test]
    public void Start_CommandLineArg0_RunsSuccessful()
    {
        // Arrange
        var starter = new FinanzDbUpdaterConsoleStarter(["0"]);

        // Act
        var result = starter.Start();

        // Assert
        Assert.That(result, Is.Zero);
    }
}