using System;
using System.Collections.Generic;
using System.Linq;

namespace StringEnhancer
{
  public class IDGenerator
  {
    private SortedSet<int> emptyIDs;
    public static bool RandomID { get; set; }

    public IDGenerator(HeaderContent aHeaderContent, string aHeaderPath)
    {
      emptyIDs = new SortedSet<int>(Enumerable.Range(0, Constants.kMaxID + 1));
      foreach (var headerItem in aHeaderContent.SortedHeaderResults[aHeaderPath])
      {
        emptyIDs.Remove(Convert.ToInt32(headerItem.ID));
      }
    }

    public int Generate()
    {
      if (RandomID)
        return GenerateRandom();
      else
        return GenerateFirstAvailable();
    }

    private int GenerateFirstAvailable()
    {
      return emptyIDs.FirstOrDefault();
    }

    private int GenerateRandom()
    {
      var random = new Random();
      return emptyIDs.ElementAtOrDefault(random.Next(emptyIDs.Count));
    }
  }
}