using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Armoire.ViewModels;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Armoire.Utils;

public class MiscHelper
{
    public static ItemViewModel CreateRandomItem(ContainerViewModel container)
    {
        if (
            NewItemViewModel.ExecutableNames is not { } exeNames
            || NewItemViewModel.Executables is not { } exes
            || NewItemViewModel.Icons is not { } icons
        )
            throw new InvalidOperationException(
                "Initialize `NewItemViewModel` static members before calling this method."
            );
        var rnd = new Random();
        var sampleItemIdx = rnd.Next(exeNames.Count);
        var sampleItemName = exeNames[sampleItemIdx];
        return new ItemViewModel(
            sampleItemName,
            exes[sampleItemName],
            icons[sampleItemName].ToBitmap(),
            container.SourceDrawerId
                ?? throw new InvalidOperationException(
                    "An `ItemViewModel` should not have a null `SourceDrawerId`."
                ),
            0,
            container,
            sampleItemName
        );
    }

    public static string GetAssetsPath()
    {
        var workingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectDirectoryDirInfo = Directory.GetParent(workingDirectory)?.Parent?.Parent?.Parent;

        if (projectDirectoryDirInfo is null)
            throw new InvalidOperationException("Invalid directory structure.");

        var assetsPath =
            projectDirectoryDirInfo
                .GetDirectories()
                .FirstOrDefault(dir => dir.Name == "Assets")
                ?.FullName ?? throw new InvalidOperationException("Assets folder not found.");

        return assetsPath;
    }

    public static void PrepopulateDatabase() { }

    public static Bitmap ConvertSysBmpToAvaBmp(System.Drawing.Bitmap bmp)
    {
        using var memory = new MemoryStream();
        bmp.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        return new Bitmap(memory);
    }

    public static Bitmap? GetAvaBmpFromExePath(string exePath)
    {
        if (string.IsNullOrEmpty(exePath))
            return null;
        var icon = Icon.ExtractAssociatedIcon(exePath);
        return icon == null ? null : ConvertSysBmpToAvaBmp(icon.ToBitmap());
    }

    public static Bitmap GetAvaBmpFromAssets(string assetFilename)
    {
        var assetsPath = GetAssetsPath();
        var fs = new FileStream(
            Path.Combine(assetsPath, assetFilename),
            FileMode.Open,
            FileAccess.Read
        );
        var image = Image.FromStream(fs);
        var bmp = new System.Drawing.Bitmap(image, 50, 50);
        return ConvertSysBmpToAvaBmp(bmp);
    }
}
