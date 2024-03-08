using HeatRetention.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace HeatRetention
{
    public class ModConfigFile
    {
        public static ModConfigFile Current { get; set; } = null!;
        public int OakumDurability { get; set; } = 64;
        public int CostPerBlock { get; set; } = 1;
    }

    public class Core : ModSystem
    {

        public override void StartPre(ICoreAPI api)
        {
            ModConfigFile.Current = api.LoadOrCreateConfig<ModConfigFile>("heatretention.json");
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            var channel = api.Network.RegisterChannel("heatretention")
               .RegisterMessageType<ModConfigFile>();
            api.Event.PlayerJoin += byPlayer => { channel.SendPacket(ModConfigFile.Current, byPlayer); };
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Network.RegisterChannel("heatretention")
                .RegisterMessageType<ModConfigFile>()
                .SetMessageHandler<ModConfigFile>(packet =>
                {
                    ModConfigFile.Current = packet;
                });
        }

        public override void Start(ICoreAPI api)
        {
            api.RegisterItemClass($"{Mod.Info.ModID}:ItemOakum", typeof(ItemOakum));

            api.RegisterBlockBehaviorClass($"{Mod.Info.ModID}:BlockHeatRetention", typeof(BlockBehaviorHeatRetention));

            api.RegisterBlockEntityBehaviorClass($"{Mod.Info.ModID}:HeatRetention", typeof(BlockEntityBehaviorHeatRetention));
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            foreach (var block in api.World.Blocks)
            {                
                if (block.FirstCodePart() == "chiseledblock")
                {
                    block.BlockBehaviors = block.BlockBehaviors.Append(new BlockBehaviorHeatRetention(block));
                    block.BlockEntityBehaviors = block.BlockEntityBehaviors.Append(
                        new BlockEntityBehaviorType() { Name = $"{Mod.Info.ModID}:HeatRetention", properties = null });
                }
            }
        }
    }
}
