namespace StringEnhancer
{
  public class RangeBuilder : IBuilder<RangeModel>
  {
    RangeModel mRangeModel;
    int mStart, mEnd;

    public RangeBuilder(int aStart, int aEnd)
    {
      mStart = aStart;
      mEnd = aEnd;
    }

    public void Build()
    {
      mRangeModel = new RangeModel()
      {
        Start = mStart,
        End = mEnd
      };
    }

    public RangeModel GetResult() => mRangeModel;
  }
}
