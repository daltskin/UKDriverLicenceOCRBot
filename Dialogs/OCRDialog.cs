namespace UKDriverLicenceOCRBot
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Dialogs;
    using System.Net.Http;
    using System.Linq;
    using System.Collections.Generic;

    [Serializable]
    public class OCRDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var activity = await argument as Activity;

            // Check where we're getting the URL to the image from (attachment won't work using the Emulator)
            string contentUrl = "";
            if (activity.Attachments.Count > 0)
            {
                contentUrl = activity.Attachments[0].ContentUrl;
            }
            else
            {
                contentUrl = activity.Text;
            }

            Uri uri;
            if (!(Uri.TryCreate(contentUrl, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            {
                await context.PostAsync($"Give me a valid url");
            }
            else
            {
                // Look busy
                var busy = context.MakeMessage();
                busy.Type = ActivityTypes.Typing;
                await context.PostAsync(busy);

                var ocrRespones = await ComputerVision.GetOCRResult(uri);
                if (ocrRespones == null)
                {
                    await context.PostAsync($"I didn't like that one, try another.");
                }
                else
                {
                    var lineContent = new List<string>();
                    foreach (var line in ocrRespones.Regions.SelectMany(l => l.Lines))
                    {
                        lineContent.Add(string.Join(" ", line.Words.ToList().Select(word => word.Text).ToList()));
                    }

                    // Cheap validation :)
                    if (lineContent.Contains("DRIVING LICENCE"))
                    {
                        UKLicence license = new UKLicence
                        {
                            LastName = lineContent.Where(t => t.StartsWith("1")).FirstOrDefault()?.Substring(2)?.Trim(),
                            FirstNames = lineContent.Where(t => t.StartsWith("2")).FirstOrDefault()?.Substring(2)?.Trim(),
                            LicenseNumber = lineContent.Where(t => t.StartsWith("5")).FirstOrDefault()?.Substring(2)?.Trim(),
                            Address = lineContent.Where(t => t.StartsWith("8")).FirstOrDefault()?.Substring(2)?.Trim()
                        };

                        await context.PostAsync($"Valid driving license detected.");
                        await context.PostAsync($"Name: {license.FirstNames} {license.LastName}\n\nNumber: {license.LicenseNumber}\n\nAddress: {license.Address}");
                    }
                    else if (ocrRespones.Regions.Count > 0)
                    {
                        // Just smash all the text together found for output
                        var text = ocrRespones.Regions.ToList().Select(region =>
                            string.Join("\n\n", region.Lines.ToList().Select(line =>
                                 string.Join(" ", line.Words.ToList().Select(word => word.Text).ToArray())).ToArray())).First();
                        await context.PostAsync($"I can see the following: {text}");
                    }
                    else
                    {
                        await context.PostAsync($"What is that?");
                    }
                }
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}