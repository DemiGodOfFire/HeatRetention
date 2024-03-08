using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace HeatRetention
{
    public class BlockEntityBehaviorHeatRetention : BlockEntityBehavior
    {
        public bool IsActive { get; private set; }

        public BlockEntityBehaviorHeatRetention(BlockEntity blockentity) : base(blockentity)
        {
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            IsActive = tree.GetBool("active");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetBool("active", IsActive);
        }

        public bool IsActivate()
        {
            if(!IsActive)
            {
                IsActive = true;
            }
            return IsActive;
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            dsc.AppendLine("IsActive " + IsActive.ToString());
        }

    }
}
