//// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

//using Bodoconsult.Finance.Payment.Banking.Extensions;
//using NUnit.Framework;
//using System;

//namespace BodoMoneyTransfer.Test.Extensions;

//[TestFixture]
//internal class StringExtensionsTests
//{

//    [Test]
//    public void ToGuid_ValidGuid_ReturnsGuid()
//    {
//        // Arrange 
//        var uid = Guid.NewGuid();
//        var input = uid.ToString();

//        // Act  
//        var result = input.ToGuid();

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result, Is.EqualTo(uid));

//    }

//    [Test]
//    public void ToGuid_ValidGuidWithoutMinuses_ReturnsGuid()
//    {
//        // Arrange 
//        var uid = Guid.NewGuid();
//        var input = uid.ToPlainString();

//        // Act  
//        var result = input.ToGuid();

//        // Assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result, Is.EqualTo(uid));

//    }

//    [Test]
//    public void ToGuid_InvalidGuid_ReturnsNull()
//    {
//        // Arrange 
//        var input = "blubb";

//        // Act  
//        var result = input.ToGuid();

//        // Assert
//        Assert.That(result, Is.EqualTo(Guid.Empty));
//    }

//}