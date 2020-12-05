namespace Business.Parsers
{
    using Business.Parser.Models;
    using EnsureThat;
    using Nett;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ProtoParsedMessage = Parser.Models.ProtoParsedMessage;

    public class ModuleParser
    {
        public JsonModel ReadFileAsJson(string fileContent, TomlSettings settings, ProtoParsedMessage protoParserMessage)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(fileContent, (nameof(fileContent)));
            
            var fileData = Toml.ReadString(fileContent, settings);
            var model = new JsonModel();
            model.Name = protoParserMessage.Name;

            var dictionary = fileData.ToDictionary();
            var module = (Dictionary<string, object>[])dictionary["module"];

            // here message.name means Power, j1939 etc.
            var moduleDetail = module.Where(dic => dic.Values.Contains(protoParserMessage.Name.ToLower())).FirstOrDefault();

            if (moduleDetail != null)
            {
                var configValues = new Dictionary<string, object>();
                
                if (moduleDetail.ContainsKey("config"))
                {
                    configValues = (Dictionary<string, object>)moduleDetail["config"];
                }

                MergeTomlWithProtoMessage(configValues, protoParserMessage, model);
            }

            return model;
        }

        public void MergeTomlWithProtoMessage(Dictionary<string, object> configValues, ProtoParsedMessage protoParsedMessage, JsonModel model)
        {
            for (int tempIndex = 0; tempIndex < configValues.Count; tempIndex++)
            {
                var key = configValues.ElementAt(tempIndex).Key;

                var messages = protoParsedMessage.Messages;
                var fields = protoParsedMessage.Fields;

                var field = fields.Where(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                var repeatedMessage = messages.Where(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase) && x.IsRepeated).FirstOrDefault();

                // its a field
                if (field != null)
                {
                    field.Id = tempIndex;
                    field.Value = GetFieldValue(configValues.ElementAt(tempIndex).Value);
                    model.Fields.Add(field);
                }
                
                else if (repeatedMessage != null)
                {
                    Dictionary<string, object>[] values = null;
                    var arrayMessages = repeatedMessage.Messages.Where(x => x.IsRepeated);

                    if (configValues.ElementAt(tempIndex).Value is Dictionary<string, object>[])
                    {
                        values = (Dictionary<string, object>[])configValues.ElementAt(tempIndex).Value;
                    }

                    // declare empty.
                    else values = new Dictionary<string, object>[0];

                    if (!arrayMessages.Any())
                    {
                        var fieldsWithData = GetFieldsData(repeatedMessage, values);
                        model.Arrays = fieldsWithData;
                    }

                    else
                    {
                        JsonModel[] jsonModels = new JsonModel[values.Length];
                        
                        for (int temp = 0; temp < values.Length; temp++)
                        {
                            jsonModels[temp] = new JsonModel();
                            jsonModels[temp].Id = temp;
                            MergeTomlWithProtoMessage(values[temp], repeatedMessage, jsonModels[temp]);
                        }

                        model.Arrays = jsonModels;
                    }
                }
            }
        }

        private bool IsValueType(object obj)
        {
            var objType = obj.GetType();
            return obj != null && objType.GetTypeInfo().IsValueType;
        }

        private object GetFieldValue(object field)
        {
            string result = string.Empty;

            Type stringType = typeof(string);
            Type fieldType = field.GetType();

            if (fieldType.IsArray)
            {
                result += "[";
                var element = ((IEnumerable)field).Cast<object>().FirstOrDefault();

                if (IsValueType(element))
                {
                    IEnumerable fields = field as IEnumerable;

                    foreach (var tempItem in fields)
                    {
                        result += tempItem + ",";
                    }

                    result = result.TrimEnd(',');
                }

                else if (stringType.IsAssignableFrom(element.GetType()))
                {
                    string[] stringFields = ((IEnumerable)field).Cast<object>()
                                                                .Select(x => x.ToString())
                                                                .ToArray();

                    stringFields = stringFields.ToList().Select(c => { c = $"\"{c}\""; return c; }).ToArray();

                    result += string.Join(",", stringFields);
                }

                result += "]";
            }

            else
            {
                if (IsValueType(field))
                {
                    result = field.ToString();
                }

                else
                {
                    result = $"\"{field}\":";
                }
            }

            return result;
        }

        private List<List<Field>> GetFieldsData(ProtoParsedMessage message, Dictionary<string, object>[] values)
        {
            var fields = message.Fields;
            var arrayOfDataAsFields = new List<List<Field>>();

            if (fields == null || !fields.Any() || values == null || !values.Any())
            {
                return arrayOfDataAsFields;
            }
            
            foreach (var dictionary in values)
            {
                // make a copy of first list;
                var copyFirstList = fields.Select(x => new Field() { Id = x.Id, DataType = x.DataType, Max = x.Max, Min = x.Min, Name = x.Name, Value = x.Value }).ToList();

                for (int tempIndex = 0; tempIndex < copyFirstList.Count; tempIndex++)
                {
                    object value = dictionary.ContainsKey(copyFirstList[tempIndex].Name) ? dictionary[copyFirstList[tempIndex].Name] : copyFirstList[tempIndex].Value;

                    if (value != null)
                    {
                        // fix the indexes.
                        copyFirstList[tempIndex].Id = tempIndex;
                        copyFirstList[tempIndex].Value = value;
                    }
                }

                arrayOfDataAsFields.Add(copyFirstList);
            }

            return arrayOfDataAsFields;
        }
    }
}
