using Google.GenAI;
using Google.GenAI.Types;

public sealed class GeminiService
{
  private const string ModelName = "gemini-3-flash-preview";

  private readonly Client _client;
  private readonly GenerateContentConfig _modelConfig;

  public GeminiService(IConfiguration configuration)
  {
    var apiKey = configuration["Gemini:ApiKey"]
        ?? throw new InvalidOperationException("Gemini API key is missing.");

    _client = new Client(apiKey: apiKey);

    _modelConfig = new GenerateContentConfig
    {
      ThinkingConfig = new ThinkingConfig
      {
        ThinkingBudget = 0
      },
      SystemInstruction = new Content
      {
        Role = "system",
        // https://reflect.app/blog/best-ai-prompts-for-note-taking-part-1
        Parts = new List<Part>()
        {
          new() { Text = "You are an expert note-taking assistant for students, but don't repeat the user's question.\nReturn only the notes. Do not wrap your response in quotes. Do not offer anything else other than the notes in the response. Do not translate the text.\nMake sure you answer precisely without hallucination and prefer bullet points over walls of text.\nPresentation\n- Use Markdown features in your response: \n  - **Bold** text to **highlight keywords** in your response\n  - **Split long information into small sections** with h2 headers and a relevant emoji at the start of it (for example `## 🐧 Linux`). Bullet points are preferred over long paragraphs, unless you're offering writing support or instructed otherwise by the user.\n- Are there could be compared? You should firstly use a table to compare the main aspects, then elaborate or include relevant comments from online forums *after* the table. Make sure to provide other resources at the end of the notes." }
        }
      }
    };
  }

  public async Task<string?> GetResponseAsync(string userPrompt)
  {
    if (string.IsNullOrWhiteSpace(userPrompt))
      return null;

    var response = await _client.Models.GenerateContentAsync(
      ModelName,
      userPrompt,
      _modelConfig
    );

    return response.Candidates?
      .FirstOrDefault()?
      .Content?
      .Parts?
      .FirstOrDefault(p => !string.IsNullOrEmpty(p.Text))?
      .Text;
  }
}
