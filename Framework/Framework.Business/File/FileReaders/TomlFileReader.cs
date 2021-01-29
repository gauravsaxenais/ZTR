namespace ZTR.Framework.Business.File.FileReaders
{
    using System;
    using Content;
    using EnsureThat;
    using Models;
    using Nett;

    /// <summary>
    /// TomlFileReader.
    /// </summary>
    public static class TomlFileReader
    {
        /// <summary>
        /// Reads the data from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        /// <exception cref="CustomArgumentException"></exception>
        public static T ReadDataFromString<T>(string data, TomlSettings settings = null) where T : class, new()
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(data, nameof(data));
            
            T fileData;
            try
            {
                settings ??= LoadLowerCaseTomlSettings();

                fileData = Toml.ReadString<T>(data, settings);
            }
            catch (Exception exception)
            {
                throw new CustomArgumentException(Resource.TomlParsingError, exception);
            }

            return fileData;
        }

        /// <summary>
        /// Loads the lower case toml settings.
        /// </summary>
        /// <returns></returns>
        public static TomlSettings LoadLowerCaseTomlSettings()
        {
            var fieldNamesToLowerCaseSettings = TomlSettings.Create(config => config
            .ConfigurePropertyMapping(mapping => mapping
            .UseKeyGenerator(standardGenerators => standardGenerators.LowerCase)));

            return fieldNamesToLowerCaseSettings;
        }
    }
}
