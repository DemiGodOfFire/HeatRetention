using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

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
            if (!IsActive)
            {
                IsActive = true;
                Blockentity.MarkDirty();
                return IsActive;
            }
            return false;
        }

        public void Deactivation(BlockPos pos)
        {
            if (IsActive)
            {
                IsActive = !IsActive;
                Blockentity.MarkDirty();
                ItemStack stack = new(Api.World.GetItem(new AssetLocation("flaxfibers")))
                {
                    StackSize = ModConfigFile.Current.CostPerBlock
                };
                Api.World.SpawnItemEntity(stack, new Vec3d(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5));
            }
        }

        public bool OnInteract(IPlayer byPlayer, BlockSelection blockSel)
        {
            bool isCreative = byPlayer.WorldData.CurrentGameMode == EnumGameMode.Creative;
            bool tryAccess = Api.World.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak);
            ItemSlot handslot = byPlayer.InventoryManager.ActiveHotbarSlot;
            if (tryAccess && handslot?.Itemstack?.Item?.Code.FirstCodePart() == "knife")
            {
                ItemStack stack = new(Api.World.GetItem(new AssetLocation("flaxfibers")))
                {
                    StackSize = ModConfigFile.Current.CostPerBlock
                };
                Api.World.SpawnItemEntity(stack, blockSel.Position.ToVec3d() + blockSel.HitPosition);
                if (!isCreative)
                {
                    handslot.Itemstack.Item.DamageItem(Api.World, byPlayer.Entity, handslot);
                }
                return true;
            }

            return false;
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            base.GetBlockInfo(forPlayer, dsc);
            dsc.AppendLine("IsActive " + IsActive.ToString());
        }
    }
}
