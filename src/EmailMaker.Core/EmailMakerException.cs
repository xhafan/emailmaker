using System;

namespace EmailMaker.Core
{
    public class EmailMakerException : Exception
    {
        public EmailMakerException(string message)
            : base(message)
        {
        }
    }
}