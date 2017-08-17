using System.IO;
using System.Linq;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class RCFileWriter
  {
    #region Public methods

    public void WriteData(RCFileContent aContent, string aFilePath)
    {
      var stringTables = aContent.StringTablesDictionary.ToList();
      stringTables.Sort((pair1, pair2) => pair1.Value.RcOrder.CompareTo(pair2.Value.RcOrder));

      using (StreamWriter streamWriter = new StreamWriter(aFilePath, true, aContent.RcEncoding))
      {
        foreach( var stringTable in stringTables)
        {
          streamWriter.WriteLine(TagConstants.kTagStringTable);
          streamWriter.WriteLine(TagConstants.kTagBegin);

          stringTable.Value.Write(streamWriter);

          streamWriter.WriteLine(TagConstants.kTagEnd + "\r\n");
        }
        streamWriter.Write(aContent.EndRcFile);
      }
    }
    
    #endregion
  }
}
