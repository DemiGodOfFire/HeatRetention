using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace HeatRetention
{
    public class BlockBehaviorHeatRetention : BlockBehavior
    {
        private int level = 1;

        public BlockBehaviorHeatRetention(Block block) : base(block)
        {
        }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);

            if (properties["level"].Exists)
            {
                level = properties["level"].AsInt();
            }
        }

        public override int GetRetention(BlockPos pos, BlockFacing facing, EnumRetentionType type, ref EnumHandling handled)
        {
            var beh = block.GetBEBehavior<BlockEntityBehaviorHeatRetention>(pos);
            if (beh != null && beh.IsActive)
            {
                handled = EnumHandling.PreventSubsequent;
                return level;
            }

            return base.GetRetention(pos, facing, type, ref handled);
        }

    }

}
