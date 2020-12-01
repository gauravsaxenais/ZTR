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
    using System.Text;

    public class ModuleParser
    {
        public string ReadFileAsJson(string fileContent, TomlSettings settings, Parser.Models.Array message)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(fileContent, (nameof(fileContent)));

            var json = string.Empty;

            var fileData = Toml.ReadString(fileContent, settings);

            var dictionary = fileData.ToDictionary();

            var module = (Dictionary<string, object>[])dictionary["module"];

            // here message.name means Power, j1939 etc.
            var moduleDetail = module.Where(dic => dic.Values.Contains(message.Name.ToLower())).FirstOrDefault();

            if (moduleDetail != null)
            {
                var configValues = (Dictionary<string, object>)moduleDetail["config"];

                WriteData(configValues, message, ref json);
            }

            return json;
        }

        public static void WriteData(Dictionary<string, object> configValues, Parser.Models.Array message, ref string json)
        {
            json += "[";
            foreach (var temp in configValues.Select((Entry, Index) => new { Entry, Index }))
            {
                var key = temp.Entry.Key;
                var value = temp.Entry.Value;
                var index = temp.Index;

                var fields = message.Fields;
                var arrays = message.Arrays;

                var foundField = fields.Where(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                var foundArray = arrays.Where(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                json += "{";

                json += $"\"id\": {index}" + ", ";

                // its a field
                if (foundField != null)
                {
                    json += $"\"name\": \"{foundField.Name}\"" + ", ";
                    json += $"\"min\": {foundField.Min}, ";
                    json += $"\"max\": {foundField.Max}, ";
                    json += $"\"value\": " + GetFieldValue(value) + ",";
                    json += $"\"datatype\": \"{foundField.DataType}\"";
                }

                if (foundArray != null)
                {
                    if (!foundArray.Arrays.Any())
                    {
                        json += $"\"name\": \"{foundArray.Name}\"" + ", ";
                        json += $"\"datatype\": " + (foundArray.IsRepeated ? "\"array\"" : "\"notarray\"") + ", ";
                        json += $"\"args\":" + (foundArray.IsRepeated ? "[" : string.Empty);
                        json += WriteMessageField(foundArray.Fields, (Dictionary<string, object>[])value);
                        json += foundArray.IsRepeated ? "]" : string.Empty;
                    }

                    else
                    {
                        foreach (var msg in foundArray.Arrays)
                        {
                            WriteData((Dictionary<string, object>)value, msg, ref json);
                        }
                    }
                }

                json += "}";
                json += ",";
            }

            json = json.TrimEnd(',');
            json += "]";
        }

        private static bool IsValueType(object obj)
        {
            var objType = obj.GetType();
            return obj != null && objType.GetTypeInfo().IsValueType;
        }

        private static object GetFieldValue(object field)
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

        public static string WriteMessageField(List<Field> fields, Dictionary<string, object>[] values)
        {
            var json = new StringBuilder();
            if (fields == null || !fields.Any() || values == null || !values.Any())
            {
                return json.ToString();
            }

            foreach (var dictionary in values)
            {
                json.Append("[");
                for (int temp = 0; temp < fields.Count; temp++)
                {
                    json.Append("{");
                    object value = dictionary.ContainsKey(fields[temp].Name) ? dictionary[fields[temp].Name] : fields[temp].Value;

                    json.Append($"\"id\": {temp}, \"name\": \"{fields[temp].Name} \", \"min\": {fields[temp].Min}, \"max\": {fields[temp].Max}, \"value\": {value}, \"datatype\": \"{fields[temp].DataType}\"");

                    json.Append("}");
                    json.Append(",");
                }

                if (json.Length > 0)
                {
                    json.Length--;
                }

                json.Append("]");

                json.Append(",");
            }

            if (json.Length > 0)
            {
                json.Length--;
            }

            return json.ToString();
        }
    }
}
