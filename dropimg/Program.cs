using SixLabors.ImageSharp;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace DropImg {

    internal sealed class Program {
        public const string toastTag = "conversion";
        public const string toastGroup = "dropimg";

        public static int Main(string[] args) {
            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated()) { return 0; }
            ToastNotificationManagerCompat.OnActivated += toastArgs => { return; };
            // Make builder for dinal toast
            var builder = new ToastContentBuilder();

            uint totalCount = (uint)args.Length;
            int successCount = 0, failCount = 0, skipCount = 0;

            if (args.Length > 0) {
                // Trigger progress toast
                SendToastWithProgress(total: totalCount);
                foreach (var (arg, index) in args.WithIndex()) {
                    try {
                        // validate that this is a path and 
                        string inFile = Path.GetFullPath(arg);
                        string outFile = Path.GetFileNameWithoutExtension(inFile) + ".png";

                        // if input and output file names are the same, skip this file (it's already a png)
                        if (inFile == outFile) { skipCount++; continue; }
                        // Load image file and save it as a PNG
                        Image.Load(inFile).SaveAsPng(outFile);
                        successCount++;
                    } catch (Exception ex) {
                        failCount++;
                        Console.WriteLine($"Error processing {arg}: {ex}");
                        continue;
                    }
                    UpdateProgress(count: (uint)index + 1, total: totalCount);
                }
                if (failCount > 0) {
                    builder.AddText($"Converted {successCount}, failed {failCount} images.");
                } else {
                    builder.AddText($"All images converted successfully!");
                }
                UpdateProgress(count: totalCount, total: totalCount);
            } else {
                builder.AddText("No images to process. Try dropping an image file on me!");
            }

            if (skipCount > 0) { builder.AddText($"Skipped {skipCount} images that were already PNGs"); }
            builder.Show(toast => {
                toast.ExpirationTime = DateTime.Now.AddMinutes(5);
                toast.Tag = toastTag;
                toast.Group = toastGroup;
            });
            return 0;
        }

        public static void SendToastWithProgress(uint total) {
            var content = new ToastContentBuilder()
                .AddText("Processing images...")
                .AddVisualChild(new AdaptiveProgressBar() {
                    Title = "DropImg",
                    Value = AdaptiveProgressBarValue.Indeterminate,
                    ValueStringOverride = new BindableString("string"),
                    Status = new BindableString("status")
                })
                .GetToastContent();

            var toast = new ToastNotification(content.GetXml()) {
                Tag = toastTag,
                Group = toastGroup,
                Data = new NotificationData { SequenceNumber = 1 }
            };
            toast.Data.Values["string"] = $"0/{total} images";
            toast.Data.Values["status"] = "Processing...";

            ToastNotificationManagerCompat.CreateToastNotifier().Show(toast);
        }

        public static void UpdateProgress(uint count, uint total) {
            var data = new NotificationData { SequenceNumber = (count + 1) };
            data.Values["string"] = $"{count}/{total} images";
            if (count == total) {
                data.Values["status"] = "Done!";
            } else {
                ToastNotificationManagerCompat.CreateToastNotifier().Update(data, toastTag, toastGroup);
            }
        }
    }

    internal static class EnumExtension {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (item, index));
    }
}