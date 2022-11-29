using SixLabors.ImageSharp;
using Microsoft.Toolkit.Uwp.Notifications;

class DropImg {

    public static int Main(string[] args) {
        int ret = 0;
#if DEBUG
        ret = ConsoleMain(args);
#else
        ret = ToastMain(args);
#endif
        return ret;
    }

    static int ConsoleMain(string[] args) {
        if (args.Length > 0) {
            int argCount = 1;
            foreach (var arg in args) {
                try {
                    Console.WriteLine($"processing file {argCount}/{args.Length}: {arg}");

                    string inFile = Path.GetFullPath(arg);
                    string outFile = Path.GetFileNameWithoutExtension(inFile) + ".png";

                    if (inFile == outFile) {
                        Console.WriteLine($"file {inFile} is already a png, skipping");
                        continue;
                    }

                    using Image image = Image.Load(inFile);
                    image.SaveAsPng(outFile);
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
        } else {
            Console.WriteLine("received no args; try dropping an image!");
            Console.ReadKey(true);
        }
        return 0;
    }

    static int ToastMain(string[] args) {
        var builder = new ToastContentBuilder();

        if (args.Length > 0) {
            int argCount = args.Length;
            int index = 0, processed = 0, skipped = 0;
            builder.AddText($"Processing {argCount} image files");

            foreach (var arg in args) {
                try {
                    // validate that this is a path and 
                    string inFile = Path.GetFullPath(arg);
                    string outFile = Path.GetFileNameWithoutExtension(inFile) + ".png";

                    // if input and output file names are the same, skip this file (it's already a png)
                    if (inFile == outFile) { skipped++; continue; }

                    // Load image file and save it as a PNG
                    Image.Load(inFile).SaveAsPng(outFile);

                    // add to toast
                    builder.AddText($"{index + 1}/{argCount}: converted to {Path.GetFileName(outFile)}");
                    processed++;
                } catch (Exception ex) {
                    builder.AddText($"{index + 1}/{argCount}: ERROR! {ex}");
                }
            }
            builder.AddText($"Done! {processed} images converted, {skipped} skipped.");
        } else {
            builder.AddText("No images to process. Try dropping an image file on me!");
        }

        // show toast and exit
        builder.Show();
        return 0;
    }

}
