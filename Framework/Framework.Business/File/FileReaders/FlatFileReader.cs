namespace ZTR.Framework.Business.File.FileReaders
{
    using System;
    using System.IO;
    using System.Text;

    public class FlatFileReader : IDisposable
    {
        private readonly StreamReader _reader;
        private int _currentLineNumber;
        private string _currentLine;
        private bool _disposed;

        public FlatFileReader(StreamReader streamReader)
        {
            _reader = streamReader;
        }

        public FlatFileReader(string path, Encoding encoding)
        {
            _reader = new StreamReader(path, encoding);
        }

        ~FlatFileReader()
        {
            Dispose(false);
        }

        public int CurrentLineNumber
        {
            get { return _currentLineNumber; }
        }

        public string CurrentLine
        {
            get { return _currentLine; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual bool ParseLine()
        {
#pragma warning disable IDE0055 // Fix formatting
            for (; ;)
#pragma warning restore IDE0055 // Fix formatting
            {
                _currentLine = _reader.ReadLine();
                if (_currentLine == null)
                {
                    _reader.Close();
                    _currentLine = string.Empty;
                    return false;
                }

                _currentLineNumber++;
                if (_currentLine.Length > 0)
                {
                    break;
                }
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _reader.Dispose();
            }

            _disposed = true;
        }
    }
}
