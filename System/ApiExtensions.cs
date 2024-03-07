using System;
using Vintagestory.API.Common;

namespace HeatRetention.Extensions
{
    public static class ApiExtensions
    {
        public static TConfig LoadOrCreateConfig<TConfig>(this ICoreAPI api, string filename) where TConfig : new()
        {
            try
            {
                var loadedConfig = api.LoadModConfig<TConfig>(filename);
                if (loadedConfig != null)
                {
                    api.StoreModConfig(loadedConfig, filename);
                    return loadedConfig;
                }
            }
            catch (Exception e)
            {
                api.World.Logger.Error("{0}", $"Failed loading file ({filename}), error {e}. Will initialize new one");
            }

            var newConfig = new TConfig();
            api.StoreModConfig(newConfig, filename);
            return newConfig;
        }
    }
}
