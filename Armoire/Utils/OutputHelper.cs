using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armoire.Utils;

public class OutputHelper
{
    public static string DocPath { get; } =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static void DebugPrintJson(object? obj, string suffix)
    {
        var jSets = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = [new StringEnumConverter()],
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };
        try
        {
            DebugPrintJsonOutput(JsonConvert.SerializeObject(obj, jSets), suffix);
        } catch (Exception ex)
        {
            Debug.WriteLine("Could not write JSON to debug. See the following exception: \n" + ex);
        }
    }

    public static void DebugPrintJsonOutput(string output, string suffix)
    {
        using var outputFile = new StreamWriter(
            Path.Combine(DocPath, "ArmoireDebugOutput_" + suffix + ".json")
        );
        outputFile.WriteLine(output);
    }
}
