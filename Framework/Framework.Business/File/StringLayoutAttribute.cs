namespace ZTR.Framework.Business.File
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class StringLayoutAttribute : Attribute
    {
        private int _startPosition;
        private int _endPosition;

        public StringLayoutAttribute(int startPosition, int endPosition)
        {
            if (startPosition >= 0 && endPosition >= startPosition)
            {
                _startPosition = startPosition;
                _endPosition = endPosition;
            }
            else
            {
                throw new ArgumentException("StartPosition must be > 0 and EndPosition must be greater than or equal to StartPosition");
            }
        }

        public int StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        public int EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }
    }
}
