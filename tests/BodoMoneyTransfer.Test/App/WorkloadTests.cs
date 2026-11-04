// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.IO;
using Bodoconsult.App.Helpers;
using BodoMoneyTransfer.Dal.App;
using NUnit.Framework;

namespace BodoMoneyTransfer.Test.App;

[TestFixture]
internal class WorkloadTests
{
    [Test]
    public void Ctor_ValidDefaultSetup_PropsSetCorrectly()
    {
        // Arrange

        // Act
        var workload = new Workload
        {
            ProductName = "Hallo"
        };

        // Assert
        Assert.That(workload.ProductName.Length, Is.Not.Zero);

        var fileName = Path.Combine(Path.GetTempPath(), "cred.json");
        SimpleJsonHelper.SerializeAsFile(fileName, workload);

        FileSystemHelper.RunInDebugMode(fileName);
    }
    [Test]
    public void LoadJson_ValidDefaultSetup_SerializedAndDeserializedCorrectly()
    {
        // Arrange
        var workload = new Workload
        {
            ProductName = "Hallo"
        };

        Assert.That(workload.ProductName.Length, Is.Not.Zero);

        var json = SimpleJsonHelper.Serialize(workload);

        // Act
        var result = SimpleJsonHelper.LoadJsonString<Workload>(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProductName.Length, Is.EqualTo(workload.ProductName.Length));
    }
}