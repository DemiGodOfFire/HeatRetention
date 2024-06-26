using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace HeatRetention
{
    public class BlockBehaviorHeatRetention : BlockBehavior
    {
        ICoreAPI api = null!;

        public BlockBehaviorHeatRetention(Block block) : base(block)
        {
        }

        public override void OnLoaded(ICoreAPI api)
        {
            this.api = api;
            block.PlacedPriorityInteract = true;
        }
             
        public override int GetRetention(BlockPos pos, BlockFacing facing, EnumRetentionType type, ref EnumHandling handled)
        {
            var beh = block.GetBEBehavior<BlockEntityBehaviorHeatRetention>(pos);

            if (beh?.IsInsulated == true)
            {
                BlockEntityMicroBlock bemc = block.GetBlockEntity<BlockEntityMicroBlock>(pos);

                if (bemc?.BlockIds != null && bemc.BlockIds.Length > 0)
                {
                    Block block = api.World.GetBlock(bemc.BlockIds[0]);
                    var mat = block.BlockMaterial;
                    if (mat == EnumBlockMaterial.Ore || mat == EnumBlockMaterial.Stone || mat == EnumBlockMaterial.Soil || mat == EnumBlockMaterial.Ceramic)
                    {
                        handled = EnumHandling.PreventSubsequent;
                        return -1;
                    }

                    handled = EnumHandling.PreventSubsequent;
                    return 1;
                }
            }

            return base.GetRetention(pos, facing, type, ref handled);
        }
    }
}
