using System.Collections.Generic;

namespace Business.Parsers.Core.Converter
{
    public interface ITree : IDictionary<string, object>
    {
    }
    public class Tree : Dictionary<string, object>, ITree
    {
      
    }   
}
