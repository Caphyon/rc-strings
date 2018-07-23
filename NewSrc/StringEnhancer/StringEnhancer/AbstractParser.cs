using System;
using System.IO;
using System.Text;

namespace StringEnhancer
{
  public abstract class AbstractParser<TParserResult> : IDisposable
    where TParserResult : new()
  {
    protected StreamReader mFileStream;
    protected TParserResult mResult = new TParserResult();
    bool mNextChecked = false;

    public AbstractParser(string aPath)
    {
      mFileStream = new StreamReader(aPath);
    }

    public AbstractParser(string aPath, Encoding aCodePage)
    {
      mFileStream = new StreamReader(aPath, aCodePage);
    }

    // The method for actual parsing
    abstract protected void DoParse();
    // The condition HasNext() returns
    abstract protected bool HasNextCondition();
    // The result GetNext() returns
    abstract protected TParserResult GetResult();

    // Check for next by trying to generate next result
    public bool HasNext()
    {
      if (mNextChecked) { return HasNextCondition(); }
      
      DoParse(); // Try to generate next result

      mNextChecked = true; // Mark result as generated

      return HasNextCondition();
    }

    // Get next result
    public TParserResult GetNext()
    {
      if (mNextChecked) // If the result has already been generated
      {
        mNextChecked = false; // Reset validation for next result
      }
      else
      {
        DoParse(); // Try to generate next result
      }

      return GetResult();
    }

    public void Dispose()
    {
      mFileStream?.Dispose();
    }
  }
}