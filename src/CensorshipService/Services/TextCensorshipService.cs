using System.Text.RegularExpressions;

namespace CensorshipService.Services
{

    public class TextCensorshipService : ITextCensorshipService
    {
        private readonly HashSet<string> _bannedWords;

        public TextCensorshipService(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "banned_words.txt");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Banned words file not found: {path}");
            }

            _bannedWords = File.ReadAllLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim().ToLower())
                .ToHashSet();
        }

        public bool ContainsBannedWords(string text)
        {
            var words = text.ToLower().Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return words.Any(word => _bannedWords.Contains(word.ToLower()));
        }

        public string CensorText(string text)
        {
            // Найпростіша реалізація з заміною заборонених слів на '*'
            foreach (var bannedWord in _bannedWords)
            {
                var pattern = $@"\b{Regex.Escape(bannedWord)}\b";
                text = Regex.Replace(text, pattern, new string('*', bannedWord.Length), RegexOptions.IgnoreCase);
            }
            return text;
        }
    }
}
