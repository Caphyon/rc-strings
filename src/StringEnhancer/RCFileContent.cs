using System.Collections.Generic;
using System.Text;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class RCFileContent
  {
    #region Members

    private Dictionary<string, StringLine> mStringLines = new Dictionary<string, StringLine>();
    private Dictionary<int, StringTable> mStringTables = new Dictionary<int, StringTable>();

    private Dictionary<string, StringLine> mStringsWithEmptyFields = new Dictionary<string, StringLine>();
    private List<string> headerFiles = new List<string>();
    #endregion

    #region Properties

    public bool NewStringWasAdded { get; internal set; }
    public int CodePage { get; internal set; }
    public string EndRcFile { get; internal set; }
    public Encoding RcEncoding => CodePage != 0 ? Encoding.GetEncoding(CodePage) : Encoding.Unicode;
    public Dictionary<string, StringLine> GetStringLinesDictionary => mStringLines;
    public Dictionary<int, StringTable> StringTablesDictionary => mStringTables;
    public List<string> Headers => headerFiles;

    #endregion

    #region Ctor

    public RCFileContent() => NewStringWasAdded = false;
    #endregion

    #region Public methods

    public void AddInStringLines(StringLine aStringLine) =>
      mStringLines.Add(aStringLine.Name, aStringLine);

    public void AddInStringTables(int aStringTableNumber, StringLine aStringLine)
    {
      if (mStringTables.Count == 0)
        AddNewStringTable(0, 0);

      StringTable existingTable;
      if (!mStringTables.TryGetValue(aStringTableNumber, out existingTable))
        AddNewStringTable(aStringTableNumber, aStringTableNumber);

      mStringTables[aStringTableNumber].AddInformation(aStringLine);
    }

    public void AddNewStringTable(int aStringTableNumber, int aRcOrder) =>
      mStringTables.Add(aStringTableNumber, new StringTable(aStringTableNumber, aRcOrder));

    public int GetStringTableNumber(string aName) =>
      mStringLines[aName].Id / ParseConstants.kStringTableCapacity;

    public StringLine GetStringLine(string aName)
    {
      mStringLines.TryGetValue(aName, out StringLine stringLine);
      return stringLine;
    }

    public bool ResourceExists(string aName) => mStringLines.ContainsKey(aName);

    public int GetRcOrder(string aName) => mStringLines[aName].RcOrder;

    public void InitId(string aName, int aId) => mStringLines[aName].Id = aId;

    public bool ContainsStringTable(int aStringTableNumber) =>
      mStringTables.ContainsKey(aStringTableNumber);

    public bool ExistsStringTable(int aStringTableNumber) =>
      mStringTables.ContainsKey(aStringTableNumber);

    public bool ExistsId(int aId) =>
      !ExistsStringTable(aId / ParseConstants.kStringTableCapacity) ||
        mStringTables[aId / ParseConstants.kStringTableCapacity].
        IsPositionEmpty(aId % ParseConstants.kStringTableCapacity) == true ? false : true;

    public void AddNewStringWithEmptyFields(StringLine aStringLine) =>
      mStringsWithEmptyFields.Add(aStringLine.Name, aStringLine);

    public bool IsStringWithEmptyFields(string aStringName) =>
      mStringsWithEmptyFields.ContainsKey(aStringName);

    #endregion
  }
}
