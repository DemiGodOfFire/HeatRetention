using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace HeatRetention
{
    public class HeatRetention : BlockEntityBehavior
    {
        public HeatRetention(BlockEntity blockentity) : base(blockentity)
        {
        }


        public override int GetHeatRetention(BlockPos pos, BlockFacing facing, ref EnumHandling handled)
        {
            handled = EnumHandling.PreventSubsequent;
            return level;
        }
    }
}
