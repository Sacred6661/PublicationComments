namespace CensorshipService.Services
{
    public interface ITextCensorshipService
    {
        string CensorText(string text);
        bool ContainsBannedWords(string text);
    }
}
