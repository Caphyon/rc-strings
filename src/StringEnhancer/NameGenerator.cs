using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class NameGenerator : IGenerator<string>
  {
    #region Members

    private string mText;

    #endregion

    #region Ctor

    public NameGenerator(string aText) => mText = aText;

    #endregion

    #region Public methods

    public string Generate()
    {
      var mostRelevantsWords = FindMostRelevantsWords();
      return string.Format("{0}{1}", TagConstants.kStringPreffix, 
        mostRelevantsWords.Count < 2 ? mostRelevantsWords[0] :
        mostRelevantsWords.Aggregate((item1, item2) => item1.ToUpper() + '_' + item2.ToUpper()));
    }

    #endregion

    #region Private methods

    private List<string> FindMostRelevantsWords()
    {
      var words = RemoveWords();
      var wordsTouple = ExtractWordsAndOccourences(words);
      return ExtractWords(wordsTouple);
    }

    private List<string> RemoveWords()
    {
      var words = mText
        .Split(Parse.kSplitResourceElementsChars)
        .Where(x => x.Length > ParseConstants.kLengthOfRelevantWord)
        .ToList();
      if (words.Count == 0)
        words = mText.Split(Parse.kSplitResourceElementsChars).ToList();

      return words;
    }

    private IEnumerable<dynamic> ExtractWordsAndOccourences(List<string> aWords)
    {
      return aWords
        .GroupBy(x => x)
        .Select(x => new
        {
          word = x.Key,
          NoOfOccourences = x.Count()
        })
        .OrderByDescending(x => x.NoOfOccourences)
        .Take(ParseConstants.kNumberOfWordsInStringName)
        .ToList();
    }

    private List<string> ExtractWords(IEnumerable<dynamic> aWordsTuple)
    {
      List<string> words = new List<string>();
      foreach (var w in aWordsTuple)
        words.Add(w.word.ToUpper());

      return words;
    }

    #endregion

  }
}
