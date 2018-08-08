using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StringEnhancer
{
  public class HeaderContentBuilder : IBuilder<HeaderContent>
  {
    private readonly string mRCPath;
    private readonly Encoding mCodePage;

    private HeaderContent mHeaderContent;

    public HeaderContentBuilder(string aRCPath, Encoding aCodePage)
    {
      mRCPath = aRCPath;
      mCodePage = aCodePage;
    }

    public void Build()
    {
      mHeaderContent = new HeaderContent();

      var headerFiles = HeaderNamesExtractor.ExtractHeaderNames(mRCPath, mCodePage);

      foreach (var headerName in headerFiles)
      {
        if (headerName.StartsWith("<") && headerName.EndsWith(">"))
          continue;

        var absolutePath = Path.Combine(Path.GetDirectoryName(mRCPath), headerName);
        if (!File.Exists(absolutePath) || (mHeaderContent.SortedHeaderResults.ContainsKey(absolutePath)))
          continue;
        ParseHeaderFile(absolutePath, mCodePage);
      }
    }

    public HeaderContent GetResult() => mHeaderContent;

    private void ParseHeaderFile(string aPath, Encoding aCodePage)
    {
      using (var idParser = new HeaderParser(aPath, aCodePage))
      {
        while (idParser.HasNext())
        {
          var obj = idParser.GetNext();

          IDTrimmer.TrimEnd(obj.ID);
          IDNormalizer.NormalizeRecurrenceForID(obj.ID, mHeaderContent);
          if (!IDValidator.IsValidWithoutRecurrenceCheck(obj.ID)) continue;

          mHeaderContent.NameToID[obj.Name] = new HeaderId(obj.ID);

          IDNormalizer.NormalizeHexaID(obj.ID);

          if (!mHeaderContent.SortedHeaderResults.ContainsKey(aPath))
          {
            mHeaderContent.SortedHeaderResults.Add(aPath, new List<HeaderItem>());
          }

          mHeaderContent.SortedHeaderResults[aPath].Add(obj);
        }

        if (mHeaderContent.SortedHeaderResults.ContainsKey(aPath))
          mHeaderContent.SortedHeaderResults[aPath].Sort(new HeaderResultComparerByID());
      }
    }
  }
}
