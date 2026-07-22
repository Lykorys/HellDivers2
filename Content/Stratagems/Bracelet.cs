using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HellDivers2.Content.System;
using System.Collections.Generic;
using System.Linq;
using HellDivers2.Content.Config;
using Microsoft.Xna.Framework.Input;
using HellDivers2.Content.UI;

namespace HellDivers2.Content.Stratagems
{
	public class Bracelet : ModItem
	{
        public bool isInUse = false;
        public required HDPlayer hDPlayer;
        public override string Texture => "Terraria/Images/Item_" + ItemID.Pho;
        public override bool AltFunctionUse(Player player) => true;
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
		}

		public override bool? UseItem(Player player) {
            hDPlayer = player.GetModPlayer<HDPlayer>();
            if(player.altFunctionUse == 2)
            {
                var uiSystem = ModContent.GetInstance<StratagemSystem>();
                if (uiSystem != null)
                {
                    if (uiSystem.userInterface?.CurrentState == null) uiSystem.ShowUI();
                    else uiSystem.HideMyUI();
                }
            }
            else
            {
                isInUse=!isInUse;
                Main.NewText(isInUse);
                var uiSystem = ModContent.GetInstance<ArrowsSystem>();
                if (uiSystem != null)
                {
                    if (uiSystem.userInterface?.CurrentState == null) uiSystem.ShowUI();
                    else uiSystem.HideMyUI();
                }
            }
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
                    foreach(StratagemPlaceable strat in hDPlayer.stratagems){
                        Main.NewText(strat.showSeq());
                        Main.NewText(strat.Type);
                        Main.NewText(strat.isInCD);
                        if(strat.isInCD) continue;
                        if(strat.currentSequence.First()==pressed)
                        {
                            strat.currentSequence.RemoveAt(0);
                            Main.NewText("correct");
                            if (strat.currentSequence.Count == 0)
                            {
                                player.QuickSpawnItem(player.GetSource_ItemUse(Item), strat.givenStrat.Item.type, 1);
                                Main.NewText("give");
                                strat.resetSequence();
                                strat.isInCD=true;
                            }
                        }
                        else
                        {
                            strat.resetSequence(); 
                        } 
                    }
                }
            }
            else{}
        }
	}
}
