using System;

namespace MathUtil
{
  public class clsException : Exception
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
    private clsException()
    {
    }

    /// <summary>
    ///         ''' Constructor
    ///         ''' </summary>
    ///         ''' <param name="ai_series"></param>
    ///         ''' <remarks></remarks>
    public clsException(Series ai_series) : base(string.Format("ErrorCode:{0}", ai_series))
    {
    }

    /// <summary>
    ///         ''' Constructor
    ///         ''' </summary>
    ///         ''' <param name="ai_series"></param>
    ///         ''' <param name="ai_msg"></param>
    ///         ''' <remarks></remarks>
    public clsException(Series ai_series, string ai_msg) : base(string.Format(@"ErrorCode:{0}\nErrorMsg:{1}", ai_series, ai_msg))
    {
    }
  }
}