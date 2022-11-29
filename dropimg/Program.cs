using SixLabors.ImageSharp;
using Microsoft.Toolkit.Uwp.Notifications;

class DropImg {

    public static int Main(string[] args) {
        if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated()) { return 0; }
        ToastNotificationManagerCompat.OnActivated += toastArgs => { return; };

        var builder = new ToastContentBuilder();
        int skipped = 0;

        if (args.Length > 0) {
            int argCount = args.Length;
            builder.AddText($"Processing {argCount} images:");
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
                    builder.AddText($"{Path.GetFileName(inFile)} converted to {Path.GetFileName(outFile)}");
                } catch (Exception ex) {
                    builder.AddText($"ERROR on {arg}: {ex}");
                    break;
                }
            }
        } else {
            builder.AddText("No images to process. Try dropping an image file on me!");
        }

        if (skipped > 0) { builder.AddText($"Skipped {skipped} images that were already PNGs"); }
        builder.Show(toast => { toast.ExpirationTime = DateTime.Now.AddMinutes(5); });
        return 0;
    }
}
