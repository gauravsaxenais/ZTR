namespace ZTR.M7Config.Business.RequestHandlers.Interfaces
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ZTR.M7Config.Business.Common.Models;
    using ZTR.M7Config.Business.Parsers.Core.Models;

    /// <summary>
    /// Config Generator interface methods.
    /// </summary>
    public interface IConfigManager
    {
        /// <summary>
        /// Creates the configuration asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Task<string> CreateConfigAsync(ConfigReadModel model);

        /// <summary>
        /// Updates the toml configuration.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        Task<bool> UpdateTomlConfig(string properties);

        /// <summary>
        /// Creates from HTML asynchronous.
        /// </summary>
        /// <param name="htmlFile">The HTML file.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        Task<string> CreateFromHtmlAsync(IFormFile htmlFile, IEnumerable<ModuleReadModel> values);
    }
}
