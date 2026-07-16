using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HellDivers2.Content.System;
using System.Collections.Generic;
using System.Linq;
using HellDivers2.Content.Config;

namespace HellDivers2.Content
{
	public class Bracelet : ModItem
	{
        public bool isInUse = false;
        public List<StratagemPlaceable> stratagems = new(4);
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pho;
		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 20;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = false;
            Item tempItem = new();
            tempItem.SetDefaults(ModContent.ItemType<TestStratagemPlaceable>());
            if (tempItem.ModItem is StratagemPlaceable testStratagem)
            {
                stratagems.Add(testStratagem);
            }
            Item tItem = new();
            tempItem.SetDefaults(ModContent.ItemType<SecondTestStratagemPlaceable>());
            if (tempItem.ModItem is StratagemPlaceable rst)
            {
                stratagems.Add(rst);
            }
		}

		public override bool? UseItem(Player player) {
            isInUse=!isInUse;
            Main.NewText(isInUse);
            return true;
        }
        public override void HoldItem(Player player)
        {
            if (isInUse)
            {
                Arrows? pressed = null;
                if (KeybindSystem.UpArrow.JustPressed) pressed= Arrows.UP;
                if (KeybindSystem.DownArrow.JustPressed) pressed= Arrows.DOWN;
                if (KeybindSystem.LeftArrow.JustPressed) pressed = Arrows.LEFT;
                if (KeybindSystem.RightArrow.JustPressed) pressed = Arrows.RIGHT;
                if (pressed.HasValue)
                {
                    foreach(StratagemPlaceable strat in stratagems){
                        Main.NewText(strat.showSeq());
                        if(strat.currentSequence.First()==pressed)
                        {
                            strat.currentSequence.RemoveAt(0);
                            Main.NewText("correct");
                            if (strat.currentSequence.Count == 0)
                            {
                                Main.NewText("give");
                                strat.resetSequence();
                            }
                            
                        }
                        else
                        {
                            strat.resetSequence(); 
                            Main.NewText("reset");
                        } 
                    }
                }
            }
            else{ Main.NewText(stratagems);}
        }
	}
}
