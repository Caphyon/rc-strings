using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.StringEnhancer
{
  public class EmptyRangeManager
  {
    private SortedSet<EmptyRange> mEmptyRanges = new SortedSet<EmptyRange>
        (new GenericComparer<EmptyRange>((obj1, obj2) => obj1.StartPosition.CompareTo(obj2.StartPosition)));

    public bool SkipFirst { get; private set; }
    public int PreviousId { get; private set; }

    public int GetLastPosition
    {
      get => mEmptyRanges.Last().StopPosition;
    }

    public EmptyRangeManager() => SkipFirst = true;

    public void FindEmptyRanges(HeaderFilesContent aHeaderContent)
    {
      List<KeyValuePair<string, string>> headerElements = aHeaderContent.SortByIdValue();

      if (headerElements.Count <= 1)
        return;

      foreach( var pairElement in headerElements )
      {
        int id = -1; 
        if ( ParseUtility.TransformToDecimal(pairElement.Value, out id) == false )
          continue;
        if (SkipFirst)
        {
          SkipFirst = false;
          PreviousId = id;
          continue;
        }
        if (PreviousId + 1 > id - 1)
        {
          PreviousId = id;
          continue;
        }
        SaveEmptyRange(PreviousId + 1, id - 1);
        PreviousId = id;
      }
    }

    private void SaveEmptyRange(int aStart, int aStop)
    {
      EmptyRange emptyRange = new EmptyRange(aStart, aStop);
      mEmptyRanges.Add(emptyRange);
    }

    public SortedSet<EmptyRange> GetEmptyRanges() => mEmptyRanges;

  }
}
