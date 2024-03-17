using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace HeatRetention
{
    public class BlockEntityBehaviorHeatRetention : BlockEntityBehavior
    {
        public bool IsInsulated { get; private set; }

        public BlockEntityBehaviorHeatRetention(BlockEntity blockentity) : base(blockentity)
        {
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            IsInsulated = tree.GetBool($"{Core.ModId}:insulated");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool($"{Core.ModId}:insulated", IsInsulated);
        }

        public bool IsActivate()
        {
            if (!IsInsulated)
            {
                IsInsulated = true;
                Blockentity.MarkDirty();
                return IsInsulated;
            }
            return false;
        }
               
        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            if (IsInsulated)
            {
                dsc.AppendLine(Lang.Get($"{Core.ModId}:insulated"));
            }
        }
    }
}
