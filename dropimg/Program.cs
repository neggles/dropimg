using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;

IImageFormat format;

int argCount = 1;
foreach (var arg in args) {
    try {
        Console.WriteLine($"processing file {argCount}: {arg}");
        string inFile = Path.GetFullPath(arg);
        string outFile = Path.GetFileNameWithoutExtension(inFile) + ".png";

        if (inFile == outFile) {
            Console.WriteLine($"file {inFile} is already a png, skipping");
            continue;
        }

        using Image image = Image.Load(inFile, out format);
        image.Save(outFile, new PngEncoder());
        Console.WriteLine($"successfully converted {Path.GetFileName(inFile)} to {Path.GetFileName(outFile)}");

    } catch (Exception ex) {
        Console.WriteLine($"hit exception on image {argCount}: {ex}");
        continue;
    } finally {
        argCount++;
    }
}

Console.Write($"{Environment.NewLine}Press any key to exit...");
Console.ReadKey(true);
