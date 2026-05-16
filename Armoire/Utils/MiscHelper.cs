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
    private static string GetAssetsPath()
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

    private static Bitmap ConvertSysBmpToAvaBmp(System.Drawing.Bitmap bmp)
    {
        using var memory = new MemoryStream();
        bmp.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        return new Bitmap(memory);
    }

    public static Bitmap? GetAvaBmpFromImgPath(string imgPath)
    {
        if (string.IsNullOrEmpty(imgPath) || !File.Exists(imgPath))
            return null;
        return ConvertSysBmpToAvaBmp(new System.Drawing.Bitmap(Image.FromFile(imgPath), 60, 60));
    }

    public static Bitmap GetAvaBmpFromAssets(string assetFilename)
    {
        var assetsPath = GetAssetsPath();
        return new Bitmap(assetsPath + Path.DirectorySeparatorChar + assetFilename);
    }
}
