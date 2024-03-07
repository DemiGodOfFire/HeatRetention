using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace HeatRetention
{
    public class ItemOakum:Item
    {

        public override void OnLoaded(ICoreAPI api)
        {
            this.Durability = ModConfigFile.Current.OakumDurability;
            base.OnLoaded(api);
        }

        public override int GetMaxDurability(ItemStack itemstack)
        {
            return base.GetMaxDurability(itemstack);
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (api.World.BlockAccessor.GetBlock(blockSel.Position) is BlockChisel &&
               api.World.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntity be)
            {
                var currentBehavior = be.Behaviors.FirstOrDefault(e => e is BlockEntityBehaviorHeatRetention, null);
                if (currentBehavior == null)
                {
                    BlockEntityBehavior behavior = api.World.ClassRegistry.CreateBlockEntityBehavior(be, "heatretention:HeatRetention");
                    behavior.Initialize(api, JsonObject.FromJson("{}"));
                    be.Behaviors.Add(behavior);
                    if((byEntity as EntityPlayer)?.Player.WorldData.CurrentGameMode != EnumGameMode.Creative)
                    {
                        DamageItem(api.World, byEntity, slot, ModConfigFile.Current.CostPerBlock);
                    }
                }
            }
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }
    }
}
