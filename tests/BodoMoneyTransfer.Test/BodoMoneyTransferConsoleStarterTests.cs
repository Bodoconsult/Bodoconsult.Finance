// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.IO;
using Bodoconsult.App.Helpers;
using BodoMoneyTransfer.Dal.App;
using NUnit.Framework;

namespace BodoMoneyTransfer.Test;

[TestFixture]
internal class BodoMoneyTransferConsoleStarterTests
{
    [Test]
    public void Ctor_ValidDefaultSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var result = new BodoMoneyTransferConsoleStarter([], true);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result.Workload, Is.Null);
            Assert.That(result.DataProtectionService, Is.Not.Null);
            Assert.That(result.CredentialsFileName, Is.Not.Null);
            Assert.That(result.CredentialsImportFileName, Is.Not.Null);
        }
    }

    [Test]
    public void LoadWorkload_ValidImportFile_WorkloadLoaded()
    {
        // Arrange 
        var result = new BodoMoneyTransferConsoleStarter([], true);

        FileSystemHelper.DeleteFile(result.CredentialsFileName);

        var workload = new Workload
        {
            ProductName = "Blubb"
        };

        // Assert

        var fileName = result.CredentialsImportFileName;
        SimpleJsonHelper.SerializeAsFile(fileName, workload);

        Assert.That(File.Exists(fileName));

        // Act  
        result.LoadWorkload();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Workload, Is.Not.Null);
            Assert.That(result.DataProtectionService, Is.Not.Null);
            Assert.That(result.CredentialsFileName, Is.Not.Null);
            Assert.That(result.CredentialsImportFileName, Is.Not.Null);
        }
    }

    [Test]
    public void LoadWorkload_ValidImportFileReimport_WorkloadLoaded()
    {
        // Arrange 
        var result = new BodoMoneyTransferConsoleStarter([], true);

        FileSystemHelper.DeleteFile(result.CredentialsFileName);

        var workload = new Workload()
        {
            ProductName = "Blubb"
        };

        // Assert
        var fileName = result.CredentialsImportFileName;
        SimpleJsonHelper.SerializeAsFile(fileName, workload);

        Assert.That(File.Exists(fileName));

        // Act  
        result.LoadWorkload();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Workload, Is.Not.Null);
            Assert.That(result.DataProtectionService, Is.Not.Null);
            Assert.That(result.CredentialsFileName, Is.Not.Null);
            Assert.That(result.CredentialsImportFileName, Is.Not.Null);
            Assert.That(File.Exists(result.CredentialsFileName));
        }

        // Act 2
        result = new BodoMoneyTransferConsoleStarter([], true);
        result.LoadWorkload();

        // Assert2
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Workload, Is.Not.Null);
            Assert.That(result.DataProtectionService, Is.Not.Null);
            Assert.That(result.CredentialsFileName, Is.Not.Null);
            Assert.That(result.CredentialsImportFileName, Is.Not.Null);
        }
    }

    //[Test]
    //public void Start_ValidImportFile_WorkloadProcessed()
    //{
    //    // Arrange 
    //    var starter = new BodoMoneyTransferConsoleStarter([], true);

    //    FileSystemHelper.DeleteFile(starter.CredentialsFileName);

    //    var workload = new Workload()
    //    {
    //        ProductName = "Blubb"
    //    };

    //    // Assert

    //    var fileName = starter.CredentialsImportFileName;
    //    SimpleJsonHelper.SerializeAsFile(fileName, workload);

    //    Assert.That(File.Exists(fileName));

    //    starter.LoadWorkload();

    //    // Act  
    //    var result = starter.Start();

    //    // Assert
    //    using (Assert.EnterMultipleScope())
    //    {
    //        Assert.That(result, Is.Zero);
    //    }
    //}
}