using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace HeatRetention
{
    public class BlockBehaviorHeatRetention : BlockBehavior
    {
        private int level = 3;

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
            handled = EnumHandling.PreventSubsequent;
            return level;
        }
       
    }

}
