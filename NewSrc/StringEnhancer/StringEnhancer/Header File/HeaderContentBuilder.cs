using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace StringEnhancer
{
  public class HeaderContentBuilder : IBuilder<HeaderContent>
  {
    private readonly string mRCPath;
    private readonly Encoding mCodePage;

    private HeaderContent mHeaderContent;

    public HeaderContentBuilder(string aPath, Encoding aCodePage)
    {
      mRCPath = aPath;
      mCodePage = aCodePage;
    }

    public void Build()
    {
      mHeaderContent = new HeaderContent()
      {
        SortedHeaderResults = new Dictionary<string, List<HeaderItem>>(),
        NameToID = new Dictionary<string, string>()
      };

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
          obj.ID = IDNormalizer.NormalizeID(obj.ID);
          obj.ID = IDNormalizer.NormalizeReccurenceForID(obj.ID, mHeaderContent.NameToID);

          if (obj.ID == Constants.kInvalidID) continue;

          if (!mHeaderContent.SortedHeaderResults.ContainsKey(aPath))
          {
            mHeaderContent.SortedHeaderResults.Add(aPath, new List<HeaderItem>());
          }

          mHeaderContent.SortedHeaderResults[aPath].Add(obj);
          mHeaderContent.NameToID[obj.Name] = obj.ID;
        }
      }
    }
  }
}
