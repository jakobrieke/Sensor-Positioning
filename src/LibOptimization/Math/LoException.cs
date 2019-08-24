using System;

namespace LoMath
{
  public class LoException : Exception
  {
    public enum Series
    {
      UnknownError,
      NotImpementaion,
      NotSquare,
      DifferRowNumberAndCollumnNumber,
      NotComputable,
      DifferElementNumber
    }

    /// <summary>
    ///         ''' Default constructor
    ///         ''' </summary>
    ///         ''' <remarks></remarks>
    private LoException()
    {
    }

    /// <summary>
    ///         ''' Constructor
    ///         ''' </summary>
    ///         ''' <param name="ai_series"></param>
    ///         ''' <remarks></remarks>
    public LoException(Series ai_series) : base(string.Format("ErrorCode:{0}", ai_series))
    {
    }

    /// <summary>
    ///         ''' Constructor
    ///         ''' </summary>
    ///         ''' <param name="ai_series"></param>
    ///         ''' <param name="ai_msg"></param>
    ///         ''' <remarks></remarks>
    public LoException(Series ai_series, string ai_msg) : base(string.Format(@"ErrorCode:{0}\nErrorMsg:{1}", ai_series, ai_msg))
    {
    }
  }
}