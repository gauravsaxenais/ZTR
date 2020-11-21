namespace ZTR.Framework.Test.Fakes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoBogus;
    using ZTR.Framework.Business;

    public abstract class ModelFaker<TModel> : AutoFaker<TModel>
        where TModel : class, IModel
    {
        public ModelFaker()
        {
            if (typeof(IModelWithId).IsAssignableFrom(typeof(TModel)))
            {
                RuleFor(nameof(IModelWithId.Id), x => 0L);
            }
        }

        public async Task<List<TModel>> GenerateWithDependenciesAsync(int count, string ruleSets = null)
        {
            // added a loop so the setup is called once for each object.
            // This allows us to use set different dependecies on each generation.
            var items = new List<TModel>();
            for (int i = 0; i < count; i++)
            {
                await SetupDependenciesAsync().ConfigureAwait(false);

                items.Add(Generate(ruleSets));
            }

            return items;
        }

        public async Task<TModel> GenerateWithDependenciesAsync(string ruleSets = null)
        {
            await SetupDependenciesAsync().ConfigureAwait(false);

            return Generate(ruleSets);
        }

        protected virtual Task SetupDependenciesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
