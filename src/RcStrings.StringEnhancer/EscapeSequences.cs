using System;
using System.Collections.Generic;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class EscapeSequences
  {
    #region Members

    private List<Tuple<string, string>> mResourceValueSequences = new List<Tuple<string, string>>
    {
      new Tuple<string, string>("\"\"", "\""),
      new Tuple<string, string>("&&", "&")
    };

    #endregion

    #region Public methods

    public string Escape(string aText)
    {
      foreach (var tuplu in mResourceValueSequences)
        aText = aText.Replace(tuplu.Item1, tuplu.Item2);
      return aText;
    }

    public string Format(string aText)
    {
      foreach (var tuplu in mResourceValueSequences)
        aText = aText.Replace(tuplu.Item2, tuplu.Item1);
      return aText;
    }

    #endregion
  }
}
