using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
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
            bool isRepairRecipe = IsRepair(recipe);
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

        public override void OnCreatedByCrafting(ItemSlot[] inSlots, ItemSlot outputSlot, GridRecipe recipe)
        {
            base.OnCreatedByCrafting(inSlots, outputSlot, recipe);

            // Prevent derp in the handbook
            if (outputSlot is DummySlot) return;

            if (IsCreate(recipe))
            {
                CalculateRepairValue(inSlots, outputSlot, out float repairValue, out _);

                int curDur =;
                int maxDur = GetMaxDurability(outputSlot.Itemstack);

                outputSlot.Itemstack.Attributes.SetInt("durability", Math.Min(maxDur, curDur ));

            }

                if (IsRepair(recipe))
            {
                CalculateRepairValue(inSlots, outputSlot, out float repairValue, out _);

                int curDur = outputSlot.Itemstack.Collectible.GetRemainingDurability(outputSlot.Itemstack);
                int maxDur = GetMaxDurability(outputSlot.Itemstack);

                outputSlot.Itemstack.Attributes.SetInt("durability", Math.Min(maxDur, (int)(curDur + maxDur * repairValue)));
            }
        }

        private void CalculateCreateValue(ItemSlot[] inSlots, ItemSlot outputSlot, out float createValue, out int matCostPerMatType)
        {

            int maxDur = GetMaxDurability(outputSlot.Itemstack);

        }


        public override bool ConsumeCraftingIngredients(ItemSlot[] inSlots, ItemSlot outputSlot, GridRecipe recipe)
        {
            // Consume as much materials in the input grid as needed
            if (IsRepair(recipe))
            {
                CalculateRepairValue(inSlots, outputSlot, out _, out int matCostPerMatType);

                foreach (var islot in inSlots)
                {
                    if (islot.Empty) continue;

                    if (islot.Itemstack?.Collectible == this) { islot.Itemstack = null; continue; }

                    islot.TakeOut(matCostPerMatType);
                }

                return true;
            }

            return false;
        }

        private void CalculateRepairValue(ItemSlot[] inSlots, ItemSlot outputSlot, out float repairValue, out int matCostPerMatType)
        {
            var origMatCount = GetOrigMatCount(inSlots, outputSlot);
            var oakumSlot = inSlots.FirstOrDefault(slot => slot.Itemstack?.Collectible is ItemOakum);
            int curDur = outputSlot.Itemstack.Collectible.GetRemainingDurability(oakumSlot?.Itemstack);
            int maxDur = GetMaxDurability(outputSlot.Itemstack);

            // How much 1x mat repairs in %
            float repairValuePerItem = 1f / origMatCount;
            // How much the mat repairs in durability
            float repairDurabilityPerItem = repairValuePerItem * maxDur;
            // Divide missing durability by repair per item = items needed for full repair 
            int fullRepairMatCount = (int)Math.Max(1, Math.Round((maxDur - curDur) / repairDurabilityPerItem));
            // Limit repair value to smallest stack size of all repair mats
            var minMatStackSize = GetInputRepairCount(inSlots);
            // Divide the cost amongst all mats
            var matTypeCount = GetRepairMatTypeCount(inSlots);

            var availableRepairMatCount = Math.Min(fullRepairMatCount, minMatStackSize * matTypeCount);
            matCostPerMatType = Math.Min(fullRepairMatCount, minMatStackSize);

            repairValue = (float)availableRepairMatCount / origMatCount ;
        }
              
        private static int GetRepairMatTypeCount(ItemSlot[] slots)
        {
            List<ItemStack> stackTypes = new();
            foreach (var slot in slots)
            {
                if (slot.Empty) continue;
                bool found = false;
                if (slot.Itemstack.Collectible is ItemOakum) continue;

                foreach (var stack in stackTypes)
                {
                    if (slot.Itemstack.Satisfies(stack))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    stackTypes.Add(slot.Itemstack);
                }
            }

            return stackTypes.Count;
        }

        private static int GetInputRepairCount(ItemSlot[] inputSlots)
        {
            OrderedDictionary<int, int> matcounts = new();
            foreach (var slot in inputSlots)
            {
                if (slot.Empty || slot.Itemstack.Collectible is ItemOakum) continue;
                var hash = slot.Itemstack.GetHashCode();
                matcounts.TryGetValue(hash, out int cnt);
                matcounts[hash] = cnt + slot.StackSize;
            }
            return matcounts.Values.Min();
        }

        private float GetOrigMatCount(ItemSlot[] inputSlots, ItemSlot outputSlot)
        {
            var stack = outputSlot.Itemstack;
            var matStack = inputSlots.FirstOrDefault(slot => !slot.Empty && slot.Itemstack.Collectible != this)?.Itemstack;

            var origMatCount = 0;

            foreach (var recipe in api.World.GridRecipes)
            {
                if ((recipe.Output.ResolvedItemstack?.Satisfies(stack) ?? false) && !IsRepair(recipe))
                {
                    foreach (var ingred in recipe.resolvedIngredients)
                    {
                        if (ingred == null) continue;

                        if (ingred.RecipeAttributes?["repairMat"].Exists == true)
                        {
                            var jstack = ingred.RecipeAttributes["repairMat"].AsObject<JsonItemStack>();
                            jstack.Resolve(api.World, string.Format("recipe '{0}' repair mat", recipe.Name));
                            if (jstack.ResolvedItemstack != null)
                            {
                                origMatCount += jstack.ResolvedItemstack.StackSize;
                            }
                        }
                        else
                        {
                            origMatCount += ingred.Quantity;
                        }
                    }

                    break;
                }
            }

            return origMatCount;
        }

        private static bool IsRepair(GridRecipe recipe)
        {
            return recipe.Name.ToString() == ($"{Core.ModId}:repair");
        }

        private static bool IsCreate(GridRecipe recipe)
        {
            return recipe.Name.ToString() == ($"{Core.ModId}:oakum");
        }
    }
}
