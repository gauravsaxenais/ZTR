namespace ZTR.Framework.DataAccess
{
    using System.Data;

    public static class DbCommandExtensions
    {
        public static void AddParameterWithValue<T>(this IDbCommand command, string name, T value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
