namespace ZTR.M7Config.Business.Parsers.Core.Converter
{
    using System.Collections.Generic;
    using System.Linq;
    using ZTR.Framework.Business;

    internal class Extractor : IExtractor<ITree>
    {
        private readonly ConvertConfig _config;
        public Extractor(ConvertConfig config)
        {         
            _config = config;
        }

        private object ExtractFields<T>(object value , T dictionary) where T : IDictionary<string, object>
        {
            if (IsValueEmpty(value) && dictionary.ContainsKey(ConvertConfig.Fields))
            {
                var dict = (T)Convert((object[])dictionary[ConvertConfig.Fields]);

                if (dict.Any())
                {
                    value = dict;
                }
            }

            return value;
        }
        private object ExtractArray<T>(object value, T dictionary) where T : IDictionary<string, object>
        {
            if (IsValueEmpty(value) && dictionary.ContainsKey(ConvertConfig.Arrays))
            {
                var ary = (object[])dictionary[ConvertConfig.Arrays];
                if (ary.Length > 0)
                {
                    value = ary.ToList().Select(o =>
                    {
                        if (o is object[] v)
                        {
                            var res = Convert(v);
                            return res;
                        }
                        return null;
                    }).ToArray();
                }
            }
            return value;
        }
        private object Extractvalue<T>(T dictionary) where T : Dictionary<string,object>
        {
            object value = null;           
            if (dictionary.ContainsKey(ConvertConfig.Value))
            {
                value = dictionary[ConvertConfig.Value];
            }
            value = ExtractFields(value, dictionary);
            value = ExtractArray(value, dictionary);
            return value ?? string.Empty;
        }

        private static bool IsValueEmpty(object value)
        {
            if (value == null)
                return true;

            return string.IsNullOrEmpty(value.ToString());
        }

        public ITree Convert(object[] input) 
        {
            var dict = new Tree();
            input.ToList().ForEach(u =>
            {
                var o = (Tree)u;
                if (o.ContainsKey(ConvertConfig.Name))
                {
                    dict.Add(o[ConvertConfig.Name].ToString(), Extractvalue(o));
                }


                if (_config.EnableHidden && !dict.Keys.Contains(ConvertConfig.VisibleKey))
                {
                    var v = o.Keys.FirstOrDefault(x => x.Compares(ConvertConfig.VisibleKey));
                    if(v != null)
                    {
                        dict.Add(ConvertConfig.VisibleKey, o[ConvertConfig.VisibleKey]);
                    }                  
                }

            });
            return dict;
        }
    }
}
