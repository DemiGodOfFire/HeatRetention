using HeatRetention.Extensions;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.ServerMods;

namespace HeatRetention
{
    public class ModConfigFile
    {
        public static ModConfigFile Current { get; set; } = null!;
        public int OakumDurability { get; set; } = 64;
        public int CostPerBlock { get; set; } = 1;
        public int QuantityFibers { get; set; } = 64;
    }

    public class Core : ModSystem
    {
        public static string? ModId { get; private set; }
        public static int Divider { get; private set; }

        public override void StartPre(ICoreAPI api)
        {
            ModConfigFile.Current = api.LoadOrCreateConfig<ModConfigFile>("heatretention.json");
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            var channel = api.Network.RegisterChannel("heatretention")
               .RegisterMessageType<ModConfigFile>();
            api.Event.PlayerJoin += byPlayer => { channel.SendPacket(ModConfigFile.Current, byPlayer); };
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Network.RegisterChannel("heatretention")
                .RegisterMessageType<ModConfigFile>()
                .SetMessageHandler<ModConfigFile>(packet =>
                {
                    ModConfigFile.Current = packet;
                });
        }

        public override void Start(ICoreAPI api)
        {
            ModId = Mod.Info.ModID;

            api.RegisterItemClass($"{ModId}:ItemOakum", typeof(ItemOakum));

            api.RegisterBlockBehaviorClass($"{ModId}:BlockHeatRetention", typeof(BlockBehaviorHeatRetention));

            api.RegisterBlockEntityBehaviorClass($"{ModId}:HeatRetention", typeof(BlockEntityBehaviorHeatRetention));
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            if (api.Side == EnumAppSide.Server)
            {
                Recipe(api);
            }

            foreach (var block in api.World.Blocks)
            {
                if (block.FirstCodePart() == "chiseledblock")
                {
                    BlockBehaviorHeatRetention blockBehavior = new(block);
                    blockBehavior.OnLoaded(api);
                    block.BlockBehaviors = block.BlockBehaviors.Append(blockBehavior);

                    if (api.Side == EnumAppSide.Server)
                    {
                        block.BlockEntityBehaviors = block.BlockEntityBehaviors.Append(
                            new BlockEntityBehaviorType() { Name = $"{ModId}:HeatRetention", properties = null });
                    }
                }
            }
        }

        private static void Recipe(ICoreAPI api)
        {
            foreach (var grecipe in api.World.GridRecipes)
            {
                if (grecipe.Name.ToString() != ($"{ModId}:oakum")) continue;
                {
                    int count = grecipe.Ingredients.Count;
                    if (count > 1)
                    {
                        HashSet<int> quantities = new();
                        foreach (var ingredient in grecipe.resolvedIngredients)
                        {
                            quantities.Add(ingredient.Quantity);
                        }

                        Divider = GCD(quantities.ToArray());

                        foreach (var ingredient in grecipe.resolvedIngredients)
                        {
                            ingredient.ResolvedItemstack.StackSize = ingredient.Quantity /= Divider;
                        }
                        //grecipe.Output.ResolvedItemstack.Attributes.SetInt("durability", ModConfigFile.Current.OakumDurability / Divider);

                    }
                    else
                    {
                        Divider = grecipe.resolvedIngredients[0].Quantity;
                        grecipe.resolvedIngredients[0].ResolvedItemstack.StackSize = grecipe.resolvedIngredients[0].Quantity = 1;
                        //grecipe.Output.ResolvedItemstack.Attributes.SetInt("durability", ModConfigFile.Current.OakumDurability / Divider);

                    }
                }
            }
        }

        private static int GCD(int[] numbers)
        {
            // Если в массиве менее двух чисел, вернуть 0
            if (numbers.Length < 2)
            {
                return 0;
            }

            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result = GCD(result, numbers[i]);
            }
            return result;
        }

        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}
