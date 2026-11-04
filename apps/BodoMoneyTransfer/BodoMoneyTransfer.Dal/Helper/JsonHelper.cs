// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BodoMoneyTransfer.Dal.Helper;

/// <summary>
/// Helper class to handle JSON in JobExecuter
/// </summary>
public class JsonHelper
{
    /// <summary>
    /// Load a object from a JSON file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static T LoadJsonFile<T>(string fileName)
    {
        T job;

        using var file = File.OpenText(fileName);
        var serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.All
        };

        job = (T)serializer.Deserialize(file, typeof(T));

        return job;
    }


    /// <summary>
    /// Serialize a object to JSON
    /// </summary>
    /// <param name="value">object to convert</param>
    /// <typeparam name="T">type of object to convert</typeparam>
    /// <returns></returns>
    public static string SerializeObject<T>(T value)
    {
        var sb = new StringBuilder(256);
        var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

        var jsonSerializer = new JsonSerializer
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        jsonSerializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        using (var jsonWriter = new JsonTextWriter(sw))
        {
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.IndentChar = '\t';
            jsonWriter.Indentation = 1;

            jsonSerializer.Serialize(jsonWriter, value, typeof(T));
        }

        return sw.ToString();
    }


    /// <summary>
    /// Save an object of type T as JSON to a file
    /// </summary>
    /// <param name="fileName">full path for the JSON file</param>
    /// <param name="data">object to serialize to JSON</param>
    /// <typeparam name="T">type of the object to serialize to JSON</typeparam>
    /// <returns>JSON string</returns>
    public static string SaveAsFile<T>(string fileName, T data)
    {

        var json = SerializeObject(data);

        //using (var file = File.CreateText(fileName))
        //{
        //    var serializer = new JsonSerializer
        //    {
        //        Formatting = Formatting.Indented,
        //        TypeNameHandling = TypeNameHandling.Objects,
        //        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        //    };
        //    serializer.Serialize(file, data);
        //}


        var dir = new FileInfo(fileName).Directory.FullName;

        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(fileName, json, Encoding.UTF8);

        return json;
    }
}