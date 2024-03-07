using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using HeatRetention.Extensions;



namespace HeatRetention
{
    public class ModConfigFile
    {
        public static ModConfigFile Current { get; set; } = null!;
        public int OakumDurability { get; set; }// = 64;
        public int CostPerBlock { get; set; } //= 1;
    }
   
        public class Core : ModSystem
    {

        public override void StartPre(ICoreAPI api)
        {
            ModConfigFile.Current = api.LoadOrCreateConfig<ModConfigFile>("HeatRetention.json");

            api.World.Config.SetInt("OakumDurability", ModConfigFile.Current.OakumDurability);
            api.World.Config.SetInt("CostPerBlock", ModConfigFile.Current.CostPerBlock);

            base.StartPre(api);
        }

        public override void Start(ICoreAPI api)
        {
            api.RegisterItemClass($"{Mod.Info.ModID}:ItemOakum", typeof(ItemOakum));

            api.RegisterBlockBehaviorClass($"{Mod.Info.ModID}:BlockHeatRetention", typeof(BlockBehaviorHeatRetention));

            api.RegisterBlockEntityBehaviorClass($"{Mod.Info.ModID}:HeatRetention", typeof(BlockEntityBehaviorHeatRetention));
        }
    }
}
