using System;

namespace TranslateViaWeb.Elements
{
    public class ElementActionException : Exception
    {
        public ElementActionException(string message) : base(message)
        {
        }
    }
}
