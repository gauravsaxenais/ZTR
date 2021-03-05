namespace ZTR.M7Config.Business.Parsers.ProtoParser.Parser
{
    using Models;
    using EnsureThat;
    using Google.Protobuf;
    using Google.Protobuf.Reflection;
    using System.Linq;

    public class CustomMessageParser : ICustomMessageParser
    {
        /// <summary>
        /// Formats the specified protoParsedMessage as JSON.
        /// </summary>
        /// <param name="message">The protoParsedMessage to format.</param>
        /// <returns>The formatted protoParsedMessage.</returns>
        public ProtoParsedMessage Format(IMessage message)
        {
            EnsureArg.IsNotNull(message);

            var protoParserMessage = new ProtoParsedMessage();
            Format(message, protoParserMessage);

            return protoParserMessage;
        }

        /// <summary>
        /// Formats the specified protoParsedMessage as Proto parser protoParsedMessage.
        /// </summary>
        /// <param name="message">The protoParsedMessage to format.</param>
        /// <param name="protoParserProtoParsedMessage">The field to parse the formatted protoParsedMessage to.</param>
        /// <returns>The formatted protoParsedMessage.</returns>
        public ProtoParsedMessage Format(IMessage message, ProtoParsedMessage protoParserProtoParsedMessage)
        {
            EnsureArg.IsNotNull(message, nameof(message));
            EnsureArg.IsNotNull(protoParserProtoParsedMessage, nameof(protoParserProtoParsedMessage));

            ProcessMessageFields(protoParserProtoParsedMessage, message);
            return protoParserProtoParsedMessage;
        }

        private void ProcessMessageFields(ProtoParsedMessage protoParserProtoParsedMessage, IMessage message)
        {
            var fieldCollection = message.Descriptor.Fields.InFieldNumberOrder();
            for (var tempIndex = 0; tempIndex < fieldCollection.Count; tempIndex++)
            {
                if (fieldCollection[tempIndex].FieldType == FieldType.Message)
                {
                    var temp = new ProtoParsedMessage
                    {
                        Id = tempIndex,
                        Name = fieldCollection[tempIndex].Name,
                        IsRepeated = fieldCollection[tempIndex].IsRepeated
                    };

                    protoParserProtoParsedMessage.Messages.Add(temp);

                    IMessage cleanSubMessage = fieldCollection[tempIndex].MessageType.Parser.ParseFrom(ByteString.Empty);
                    ProcessMessageFields(temp, cleanSubMessage);
                }
                else
                {
                    var field = InitFieldValues(fieldCollection[tempIndex]);

                    if (field != null)
                    {
                        field.Id = tempIndex;
                        field.Name = fieldCollection[tempIndex].Name;

                        if (fieldCollection[tempIndex].FieldType == FieldType.Enum)
                        {
                            var firstValue = fieldCollection[tempIndex].EnumType.Values.First();
                            var lastValue = fieldCollection[tempIndex].EnumType.Values.Last();

                            field.Min = firstValue.Number;
                            field.Max = lastValue.Number;
                        }

                        protoParserProtoParsedMessage.Fields.Add(field);
                    }
                }
            }
        }

        private static Field InitFieldValues(FieldDescriptor descriptor)
        {
            return descriptor.FieldType switch
            {
                FieldType.Bool => new Field() { DataType = "bool", DefaultValue = false, Min = 0, Max = 0, Value = false, IsRepeated = descriptor.IsRepeated },
                FieldType.Bytes or FieldType.String => new Field() { DataType = "string", DefaultValue = string.Empty, Min = string.Empty, Max = string.Empty, Value = string.Empty, IsRepeated = descriptor.IsRepeated },
                FieldType.Double => new Field() { DataType = "double", DefaultValue = 0.0, Min = double.MinValue, Max = double.MaxValue, Value = 0.0, IsRepeated = descriptor.IsRepeated },
                FieldType.SInt32 or FieldType.Int32 or FieldType.SFixed32 => new Field() { DataType = "integer", DefaultValue = 0, Min = int.MinValue, Max = int.MaxValue, Value = 0, IsRepeated = descriptor.IsRepeated },
                FieldType.Fixed32 or FieldType.UInt32 => new Field() { DataType = "integer", DefaultValue = 0, Min = uint.MinValue, Max = uint.MaxValue, Value = 0, IsRepeated = descriptor.IsRepeated },
                FieldType.Fixed64 or FieldType.UInt64 => new Field() { DataType = "integer", DefaultValue = 0, Min = ulong.MinValue, Max = ulong.MaxValue, Value = 0, IsRepeated = descriptor.IsRepeated },
                FieldType.SFixed64 or FieldType.Int64 or FieldType.SInt64 => new Field() { DataType = "integer", DefaultValue = 0, Min = long.MinValue, Max = long.MaxValue, Value = 0, IsRepeated = descriptor.IsRepeated },
                FieldType.Enum => new Field() { DataType = "enum", DefaultValue = 0, Min = 0, Max = 0, Value = 0, IsRepeated = descriptor.IsRepeated },
                FieldType.Float => new Field() { DataType = "float", DefaultValue = 0.0, Min = float.MinValue, Max = float.MaxValue, Value = 0.0, IsRepeated = descriptor.IsRepeated },
                _ => null,
            };
        }
    }
}

