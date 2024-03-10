using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace HeatRetention
{
    public class ItemOakum : Item
    {

        public override void OnLoaded(ICoreAPI api)
        {
            this.Durability = ModConfigFile.Current.OakumDurability;
            base.OnLoaded(api);
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (blockSel != null && api.World.BlockAccessor.GetBlock(blockSel.Position) is BlockChisel &&
                api.World.BlockAccessor.GetBlockEntity(blockSel.Position)
                .GetBehavior<BlockEntityBehaviorHeatRetention>().IsActivate())
            {
                DamageItem(api.World, byEntity, slot, ModConfigFile.Current.CostPerBlock);
                handling = EnumHandHandling.PreventDefaultAction;
                return;
            }

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        public override void OnHandbookRecipeRender(ICoreClientAPI capi, GridRecipe recipe, ItemSlot dummyslot, double x, double y, double z, double size)
        {
            bool isRepairRecipe = recipe.Name.ToString() == ($"{Core.ModId}:repair");
            int prevDura = 0;
            if (isRepairRecipe)
            {
                prevDura = dummyslot.Itemstack.Collectible.GetRemainingDurability(dummyslot.Itemstack);
                dummyslot.Itemstack.Attributes.SetInt("durability", 1);
            }

            base.OnHandbookRecipeRender(capi, recipe, dummyslot, x, y, z, size);

            if (isRepairRecipe)
            {
                dummyslot.Itemstack.Attributes.SetInt("durability", prevDura);
            }
        }


    }
}
