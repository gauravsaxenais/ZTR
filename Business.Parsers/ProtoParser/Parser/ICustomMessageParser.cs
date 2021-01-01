namespace Business.Parsers.ProtoParser.Parser
{
    using Business.Parsers.ProtoParser.Models;
    using Google.Protobuf;

    public interface ICustomMessageParser
    {
        /// <summary>
        /// Formats the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Proto parsed message.</returns>
        ProtoParsedMessage Format(IMessage message);

        /// <summary>
        /// Formats the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="protoParserMessage">The proto parser message.</param>
        /// <returns>Proto parsed message.</returns>
        ProtoParsedMessage Format(IMessage message, ProtoParsedMessage protoParserMessage);
    }
}
