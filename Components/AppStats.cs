public class AppState
{
  public string? CurrentDirectory { get; set; }

  public event Action? OnChange;

  public void SetDirectory(string? path)
  {
    CurrentDirectory = path;
    NotifyStateChanged();
  }

  void NotifyStateChanged() => OnChange?.Invoke();
}
