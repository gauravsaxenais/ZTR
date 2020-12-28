namespace Business.RequestHandlers.Interfaces
{
    using Business.Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface maps with BlockManager
    /// and responsible for parsing toml files.
    /// </summary>
    public interface IBlockManager
    {
        /// <summary>
        /// Parses the toml files asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<object> GetBlocksAsObjectAsync();

        /// <summary>
        /// Gets the list of blocks.
        /// </summary>
        /// <param name="blockConfigDirectory">The block configuration directory.</param>
        /// <returns></returns>
        List<BlockJsonModel> GetListOfBlocks(DirectoryInfo blockConfigDirectory);
    }
}
