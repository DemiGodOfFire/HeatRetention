using System.Text;
using Vintagestory.API.Common;

namespace HeatRetention
{
    public class BlockEntityBehaviorHeatRetention : BlockEntityBehavior
    {
        public BlockEntityBehaviorHeatRetention(BlockEntity blockentity) : base(blockentity)
        {
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {

            dsc.AppendLine($"Test");

            base.GetBlockInfo(forPlayer, dsc);
        }

    }
}
