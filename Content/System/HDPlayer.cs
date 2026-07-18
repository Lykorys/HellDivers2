
using Terraria.ModLoader;
using Terraria;
namespace HellDivers2.Content.System
{
    public class HDPlayer : ModPlayer
    {
        public StratagemPlaceable[] stratagems = new StratagemPlaceable[4];// TODO add item slot for hold strat and for the arrows ui two set yellow under and remove white after correct and move 1 slot to the right
        public override void Initialize()
        {
            Item tempItem = new();
            tempItem.SetDefaults(ModContent.ItemType<TestStratagemPlaceable>());
            if (tempItem.ModItem is StratagemPlaceable testStratagem)
            {
                stratagems[0]=testStratagem;
            }
            tempItem.SetDefaults(ModContent.ItemType<KGBombPlaceable>());
            if (tempItem.ModItem is StratagemPlaceable rst)
            {
                stratagems[1]=rst;
            }
        }
        public override void PostUpdate()
        {
            foreach(var strat in stratagems)
            {
                if (strat != null && strat.isInCD)
                {
                    if (strat.cooldown == 0)
                    {
                        strat.isInCD=false;
                        strat.cooldown=strat.OriginalCD;
                    } 
                    else strat.cooldown--;
                } 
            }
        }
    }










} 