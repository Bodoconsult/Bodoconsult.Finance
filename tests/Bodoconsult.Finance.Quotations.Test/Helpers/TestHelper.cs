// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Bodoconsult.Finance.Quotations.Test.Helpers;

public static class TestHelper
{
    private static string _testDataPath;

    public static string TestDataPath
    {
        get
        {

            if (!string.IsNullOrEmpty(_testDataPath)) return _testDataPath;

            var path = new DirectoryInfo(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName).Parent.Parent.Parent.FullName;

            _testDataPath = Path.Combine(path, "TestData");

            if (!Directory.Exists(_testDataPath)) Directory.CreateDirectory(_testDataPath);

            return _testDataPath;
        }
    }

    /// <summary>
    /// Start an app by file name
    /// </summary>
    /// <param name="fileName"></param>
    public static void StartFile(string fileName)
    {

        if (!Debugger.IsAttached) return;

        Assert.IsTrue(File.Exists(fileName));

        var p = new Process { StartInfo = new ProcessStartInfo { UseShellExecute = true, FileName = fileName } };

        p.Start();

    }

    /// <summary>
    /// Get the access data for a web service (token) etc. from a text file
    /// </summary>
    /// <param name="fileName">File name without path</param>
    /// <returns></returns>
    public static string GetAccessData(string fileName)
    {

        fileName = Path.Combine(@"D:\Daten\Projekte\_work", fileName);

        return File.ReadAllText(fileName);

    }

    /// <summary>
    /// Set a state message in the UI
    /// </summary>
    /// <param name="msg">Message to show</param>
    public static void SetState(string msg)
    {
        Debug.Print(msg);
    }
}