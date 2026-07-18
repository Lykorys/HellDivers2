using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria;
using HellDivers2.Content.System;
using Terraria.ModLoader;


namespace HellDivers2.Content.UI
{
    public class StratagemUIState : UIState
    {
        public StratagemPanel Panel { get; private set; }
        public override void OnInitialize()
        {
            Panel = new StratagemPanel();
            Panel.Activate();
            Append(Panel);
        }
    }
    public class StratagemPanel : UIPanel //The panel itself
    {
        private Vector2 offset;
        private bool isDragging;
        public override void OnInitialize()
        {
            int slotSize = 52;
            int padding = 6;
            BackgroundColor = Color.Black;
            BorderColor = Color.DarkBlue;
            Width.Set(52*4+padding*5, 0);
            Height.Set(52+2*padding, 0);
            Left.Set(400, 0);
            Top.Set(400, 0);
            SetPadding(0);
            
            for(int i=0;i<4;i++)
            {
                StratagemUIElm slot = new(i);
                slot.Width.Set(slotSize, 0);
                slot.Height.Set(slotSize, 0);
                slot.Left.Set(padding + (i * (slotSize + padding)), 0);
                slot.Top.Set(padding, 0);
                Append(slot);
            }
        }
        //Next part is taken from ExampleMod github
        public override void LeftMouseDown(UIMouseEvent evt) {
			base.LeftMouseDown(evt);
			if (evt.Target == this) {
				DragStart(evt);
			}
		}
        public override void LeftMouseUp(UIMouseEvent evt) {
			base.LeftMouseUp(evt);
			if (evt.Target == this) {
				DragEnd(evt);
			}
		}
        private void DragStart(UIMouseEvent evt) {
			offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			isDragging = true;
		}

		private void DragEnd(UIMouseEvent evt) {
			Vector2 endMousePosition = evt.MousePosition;
			isDragging = false;
			Left.Set(endMousePosition.X - offset.X, 0f);
			Top.Set(endMousePosition.Y - offset.Y, 0f);
			Recalculate();
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			if (ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}

			if (isDragging) {
				Left.Set(Main.mouseX - offset.X, 0f);
				Top.Set(Main.mouseY - offset.Y, 0f);
				Recalculate();
			}
			var parentSpace = Parent != null 
                ? Parent.GetDimensions().ToRectangle() 
                : new Rectangle(0, 0, Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height);
			if (!GetDimensions().ToRectangle().Intersects(parentSpace)) {
				Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
				Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
				Recalculate();
			}
        }
    }



    public class StratagemUIElm(int index) : UIElement //The slot
    {
        private readonly int slot = index;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true; 
                HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
                if (modPlayer.stratagems[slot] != null)
                {
                    Item currentItem = modPlayer.stratagems[slot].Item;
                    if (currentItem != null)
                    {
                        Main.HoverItem = currentItem.Clone();
                        Main.instance.MouseText(string.Empty);
                    }
                }
            }
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.blockMouse = true;
            HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
            Item mouseItem = Main.mouseItem;
            if(!Main.playerInventory) return;
            if (mouseItem.ModItem is StratagemPlaceable newStratagem)//case holding placeable
            {
                StratagemPlaceable oldStratagem = modPlayer.stratagems[slot];
                Item clone = mouseItem.Clone();
                modPlayer.stratagems[slot] = (StratagemPlaceable)clone.ModItem;
                if (oldStratagem != null) Main.mouseItem = oldStratagem.Item.Clone(); 
                else
                {
                    Main.mouseItem = new Item();
                    Main.mouseItem.SetDefaults(ItemID.None);//Turntoair because stack of placeable is always one
                }
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab);
            }
            else if((mouseItem == null || mouseItem.IsAir) && modPlayer.stratagems[slot] != null)//case holding nothing placeable in slot
            {
                StratagemPlaceable stratToRemove = modPlayer.stratagems[slot];
                Main.mouseItem = stratToRemove.Item.Clone(); 
                modPlayer.stratagems[slot] = null;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Grab);
            }
        }



        protected override void DrawSelf(SpriteBatch spriteBatch)//TODO fix
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Texture2D slotTexture = TextureAssets.InventoryBack9.Value;
            spriteBatch.Draw(slotTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, dimensions.Width / slotTexture.Width, SpriteEffects.None, 0f);
            HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
            if (slot < modPlayer.stratagems.Length && modPlayer.stratagems[slot] != null)
            {
                Item currentItem = modPlayer.stratagems[slot].Item;
                if (currentItem != null && !currentItem.IsAir)
                {
                    Main.instance.LoadItem(currentItem.type);
                    Texture2D itemTexture = TextureAssets.Item[currentItem.type].Value;
                    float itemScale = 1f;
                    if (itemTexture.Width > dimensions.Width || itemTexture.Height > dimensions.Height)
                    {
                        itemScale = (itemTexture.Width > itemTexture.Height) ? dimensions.Width / itemTexture.Width : dimensions.Height / itemTexture.Height;
                    }
                    Vector2 position = new(
                        dimensions.X + (dimensions.Width / 2f) - (itemTexture.Width * itemScale / 2f),
                        dimensions.Y + (dimensions.Height / 2f) - (itemTexture.Height * itemScale / 2f)
                    );
                    spriteBatch.Draw(itemTexture, position, null, Color.White, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                }
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class StratagemSystem : ModSystem //The system regarding the ui
    {
        internal UserInterface userInterface;
        internal StratagemUIState uiState;

        public override void Load()
        {
            if (!Main.dedServ) 
            {
                userInterface = new UserInterface();
                uiState = new StratagemUIState();
                uiState.Activate();
            }
        }  

        public override void UpdateUI(GameTime gameTime)
        {
            if (userInterface?.CurrentState != null)
            {
                userInterface.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "HellDivers2: Stratagem UI",
                    delegate
                    {
                        if (userInterface?.CurrentState != null)
                        {
                            userInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        internal void ShowUI() {
            userInterface?.SetState(uiState);
        }

        internal void HideMyUI() {
            userInterface?.SetState(null);
        }

    }
}