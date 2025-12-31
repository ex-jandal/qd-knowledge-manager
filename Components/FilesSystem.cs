public class FileSystem
{
  private readonly string name;
  private readonly bool isDirectory;
  private readonly bool hidden = false;
  private string icon;
  private readonly string path;

  public FileSystem(string path)
  {
    if (string.IsNullOrWhiteSpace(path))
      throw new ArgumentException("Path cannot be null or empty.", nameof(path));

    this.path = path;
    isDirectory = Directory.Exists(path); ;

    name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));

    hidden = !string.IsNullOrEmpty(name) && name.StartsWith(".");

    if (isDirectory)
    {
      icon = "";
    }
    else
    {
      icon = "󰈙";
    }
  }

  public IEnumerable<FileSystem> GetChildren(int orderMethod)
  {
    if (!isDirectory)
      return Enumerable.Empty<FileSystem>();

    try
    {

      if (orderMethod == 0)
      {
        return Directory
          .EnumerateFileSystemEntries(path)
          .Select(p => new FileSystem(p))
          .OrderByDescending(f => f.IsDirectory())
          .ThenByDescending(f => f.GetName(), StringComparer.OrdinalIgnoreCase);
      }
      else
      {
        return Directory
          .EnumerateFileSystemEntries(path)
          .Select(p => new FileSystem(p))
          .OrderByDescending(f => f.IsDirectory())
          .ThenBy(f => f.GetName(), StringComparer.OrdinalIgnoreCase);
      }
    }
    catch
    {
      return Enumerable.Empty<FileSystem>();
    }
  }

  public bool IsDirectory() => isDirectory;
  public bool IsHidden() => hidden;

  public void SetIcon(string icon) => this.icon = icon;
  public string GetIcon() => icon;

  public string GetName() => name;
  public string GetFullPath() => path;
}
