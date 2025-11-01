using System.Text.RegularExpressions;

public class TextUtil
{
    public static int CountUniqueWords(List<string> texts)
    {
        var wordSet = new HashSet<string>();

        foreach (var text in texts)
        {
            // Remove special characters and split by whitespace
            var words = Regex.Split(text, @"\W+")
                             .Where(w => !string.IsNullOrEmpty(w))
                             .Select(w => w.ToLower());

            foreach (var word in words)
            {
                wordSet.Add(word);
            }
        }

        return wordSet.Count;
    }

    public static List<string> GetWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<string>();

        // Remove special characters (keep letters, digits, and spaces)
        string cleaned = Regex.Replace(input, @"[^a-zA-Z0-9\s]", " ");

        // Split by spaces, remove empty entries, convert to lowercase, and return as List
        List<string> words = cleaned
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLower())
            .ToList();

        return words;
    }
}
