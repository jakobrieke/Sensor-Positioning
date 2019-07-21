using System;
using System.Collections.Generic;

namespace Util
{
    /// <summary>
    ///     ''' ErrorManage class
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' </remarks>
    public class clsError
    {
        private List<clsErrorInfomation> m_errList = new List<clsErrorInfomation>();

        public enum ErrorType
        {
            NO_ERR,
            ERR_INIT,
            // ERR_OPT_MAXITERATION
            ERR_UNKNOWN
        }

        /// <summary>
        ///         ''' Error infomation class
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public class clsErrorInfomation
        {
            /// <summary>
            ///             ''' Default constructor(do not use)
            ///             ''' </summary>
            ///             ''' <remarks></remarks>
            private clsErrorInfomation()
            {
            }

            /// <summary>
            ///             ''' Constructor
            ///             ''' </summary>
            ///             ''' <param name="ai_setError"></param>
            ///             ''' <param name="ai_errorType"></param>
            ///             ''' <param name="ai_errorMsg"></param>
            ///             ''' <remarks></remarks>
            public clsErrorInfomation(bool ai_setError, ErrorType ai_errorType, string ai_errorMsg)
            {
                this.ErrorFlg = ai_setError;
                this.ErrorType = ai_errorType;
                this.ErrorMsg = ai_errorMsg;
            }

            public bool ErrorFlg { get; set; } = false;
            public ErrorType ErrorType { get; set; } = ErrorType.NO_ERR;
            public string ErrorMsg { get; set; } = string.Empty;
        }

        /// <summary>
        ///         ''' Default constructor
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public clsError()
        {
        }

        /// <summary>
        ///         ''' Set Error
        ///         ''' </summary>
        ///         ''' <param name="ai_setError"></param>
        ///         ''' <param name="ai_errorType"></param>
        ///         ''' <param name="ai_errorMsg"></param>
        ///         ''' <remarks></remarks>
        public void SetError(bool ai_setError, ErrorType ai_errorType, string ai_errorMsg = "")
        {
            this.m_errList.Add(new clsErrorInfomation(ai_setError, ai_errorType, ai_errorMsg));
        }

        /// <summary>
        ///         ''' Is error
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public bool IsError()
        {
            if (this.m_errList.Count == 0)
                return false;
            int index = this.m_errList.Count;
            return this.m_errList[index - 1].ErrorFlg;
        }

        /// <summary>
        ///         ''' Get Last error infomation
        ///         ''' </summary>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public clsErrorInfomation GetLastErrorInfomation()
        {
            if (this.m_errList.Count == 0)
                return new clsErrorInfomation(false, ErrorType.NO_ERR, "");
            int index = this.m_errList.Count;
            return this.m_errList[index - 1];
        }

        /// <summary>
        ///         ''' Clear error
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public void Clear()
        {
            this.m_errList.Clear();
        }

        /// <summary>
        ///         ''' Error Output
        ///         ''' </summary>
        ///         ''' <remarks></remarks>
        public static void Print(clsErrorInfomation ai_errInfomation)
        {
            if (ai_errInfomation.ErrorFlg == true)
                Console.WriteLine("IsError     :True");
            else
                Console.WriteLine("IsError     :False");
            Console.WriteLine("ErrorType   :" + ai_errInfomation.ErrorType);
            Console.WriteLine("ErrorMessage:" + ai_errInfomation.ErrorMsg);
        }
    }
}

