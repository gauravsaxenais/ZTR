using System.IO;
using System.Text;

namespace Business.Parsers.ProtoParser.Parser
{
    public class ProcessBufferHandler
    {
        public Stream stream;
        public StringBuilder sb;
        public Encoding encoding;
        public State state;

        public enum State
        {
            Running,
            Stopped
        }

        public ProcessBufferHandler(Stream stream, Encoding encoding)
        {
            this.stream = stream;
            this.sb = new StringBuilder();
            this.encoding = encoding;
            state = State.Running;
        }
        public void ProcessBuffer()
        {
            sb.Append(new StreamReader(stream, encoding).ReadToEnd());
        }

        public string ReadToEnd()
        {
            return sb.ToString();
        }

        public void Stop()
        {
            state = State.Stopped;
        }
    }
}
