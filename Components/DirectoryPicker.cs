using System.Diagnostics;

public class LinuxDirectoryPicker
{
  public async Task<string?> DefaultPathAsync()
  {
    var gnpi = new ProcessStartInfo
    {
      FileName = "whoami",
      RedirectStandardOutput = true
    };
    var getusernamePSI = Process.Start(gnpi);
    if (getusernamePSI == null) return null;

    var username = (await getusernamePSI.StandardOutput.ReadToEndAsync()).Trim();
    await getusernamePSI.WaitForExitAsync();

    return string.Format("/home/{0}/Documents", username);
  }

  public async Task<string?> PickAsync()
  {
    string? startPath = await DefaultPathAsync();

    var zenityPSI = new ProcessStartInfo
    {
      FileName = "zenity",
      Arguments = string.Format("--file-selection --directory --filename {0}/", startPath),
      RedirectStandardOutput = true
    };

    var process = Process.Start(zenityPSI);
    if (process == null) return null;

    var path = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();

    return string.IsNullOrWhiteSpace(path) ? null : path.Trim();
  }

  public async Task<string?> PickAsync(string? prefferdPath)
  {
    var zenityPSI = new ProcessStartInfo
    {
      FileName = "zenity",
      Arguments = string.Format("--file-selection --directory --filename {0}/", prefferdPath),
      RedirectStandardOutput = true
    };

    var process = Process.Start(zenityPSI);
    if (process == null) return null;

    var path = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();

    return string.IsNullOrWhiteSpace(path) ? null : path.Trim();
  }
}
