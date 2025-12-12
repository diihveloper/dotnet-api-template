using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.Regex;

namespace DiihTemplate.Core.Utils;

public static partial class StringExtensions
{
    [GeneratedRegex("([.?!])")]
    private static partial Regex SeparadoresExpression();

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial System.Text.RegularExpressions.Regex NotAlphaNumeric();

    [GeneratedRegex(@"\w+\b")]
    private static partial Regex WordBoundRegex();

    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string RemoveAccents(this string text)
    {
        StringBuilder sbReturn = new StringBuilder();
        var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
        foreach (char letter in arrayText)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);
        }

        return sbReturn.ToString();
    }

    public static string ToSlug(this string text)
    {
        return NotAlphaNumeric().Replace(text.RemoveAccents().ToLower(), "")
            .Replace(" ", "-")
            .Replace("--", "-");
    }

    public static long WordCount(this string text)
    {
        return WordBoundRegex().Count(text);
    }


    public static List<string> SplitPhrases(this string texto)
    {
        //var pattern = @"(.*?[.!?;] )";
        // @"((?:[^.?;!]{2,}\. )|.*?[!?;])"
        return Split(texto, @"(.*?\w{2,}[.!?;]\s)", RegexOptions.Singleline).Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    public static List<string> ToChunks(this string texto, int wordsCount = 5000,
        int overlap = 5)
    {
        if (texto.WordCount() < wordsCount)
        {
            return [texto];
        }

        var result = new List<string>();
        // Dividir texto em frases
        var phrases = texto.SplitPhrases();

        var builder = new StringBuilder();
        long wordCount = 0;
        string? lastChunk = null;
        for (int i = 0; i < phrases.Count(); i++)
        {
            var phrase = phrases.ElementAt(i);
            builder.Append($"{phrase} ");
            wordCount += phrase.WordCount();

            if (wordCount > wordsCount)
            {
                if (lastChunk != null)
                {
                    var lastPhrases = lastChunk.SplitPhrases().TakeLast(overlap);
                    builder.Insert(0, $"{string.Join(" ", lastPhrases)}\n");
                }

                lastChunk = builder.ToString();
                result.Add(lastChunk);
                builder.Clear();
                wordCount = 0;
            }
        }

        if (builder.Length > 0)
        {
            if (lastChunk != null)
            {
                var lastPhraseSplit = lastChunk.SplitPhrases();
                var lastPhrases = lastPhraseSplit.TakeLast(overlap);
                builder.Insert(0, $"{string.Join(" ", lastPhrases)}\n");
            }

            result.Add(builder.ToString());
        }


        return result;
    }
}