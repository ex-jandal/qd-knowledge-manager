using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using System.Text.Json;

public class YoutubeTranscriptService
{
  private readonly YoutubeDL ytdlp = new();

  public YoutubeTranscriptService()
  {
    ytdlp.YoutubeDLPath = "/usr/bin/yt-dlp";
    ytdlp.FFmpegPath = "/usr/bin/ffmpeg";
    ytdlp.OutputFolder = "/home/abu_jandal/Documents/QD_test";

    Directory.CreateDirectory(ytdlp.OutputFolder);
  }

  public async Task<string?> GetTranscriptAsync(string url, string lang = "en.*")
  {
    foreach (var f in Directory.GetFiles(ytdlp.OutputFolder, "*.json3"))
      File.Delete(f);

    OptionSet options = new()
    {
      SkipDownload = true,
      WriteAutoSubs = true,
      WriteSubs = true,
      SubLangs = lang,
      SubFormat = "json3",
    };

    var result = await ytdlp.RunVideoDownload(url, overrideOptions: options);

    if (!result.Success)
      return null;

    var jsonFile = Directory
      .GetFiles(ytdlp.OutputFolder, "*.json3")
      .OrderByDescending(File.GetLastWriteTime)
      .FirstOrDefault();

    if (jsonFile == null)
      return null;

    var json = await File.ReadAllTextAsync(jsonFile);
    return CleanJson3(json);
  }

  private static string CleanJson3(string json)
  {
    using var doc = JsonDocument.Parse(json);
    var events = doc.RootElement.GetProperty("events");

    var lines = new List<string>();
    string last = "";

    foreach (var e in events.EnumerateArray())
    {
      if (!e.TryGetProperty("segs", out var segs))
        continue;

      var text = string.Concat(
        segs.EnumerateArray()
          .Select(s => s.GetProperty("utf8").GetString())
      ).Trim();

      if (string.IsNullOrWhiteSpace(text))
        continue;

      if (text == last)
        continue;

      lines.Add(text);
      last = text;
    }

    return string.Join(" ", lines);
  }
}
