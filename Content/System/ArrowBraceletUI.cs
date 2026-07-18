/*
This namespace contains all the ui for the arrow section of the bracelet that includes :
The 4  slot of stratagems their respective cooldown and their combinations

*/
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria;
using HellDivers2.Content.System;
using Terraria.ModLoader;
//TODO fix clicking through the ui and space and arrow symbol and cooldown
//TODO toadd same ui as hd2 where 5 sec the icon bleed through the screen
namespace HellDivers2.Content.UI
{
    public class ArrowState : UIState
        {
            public ArrowPanel Panel { get; private set; }
            public override void OnInitialize()
            {
                Panel = new ArrowPanel();
                Panel.Activate();
                Append(Panel);
            }
        }
    public class ArrowPanel : UIPanel //The panel itself
    {
        private Vector2 offset;
        private bool isDragging;
        public override void OnInitialize()
        {
            int slotSize = 52;
            int padding = 6;
            BackgroundColor = new Color(20, 20, 20, 220); 
            BorderColor = new Color(255, 222, 0, 255);
            Width.Set(52*4+padding*5, 0);
            Height.Set(52*4+5*padding, 0);
            Left.Set(400, 0);
            Top.Set(400, 0);
            SetPadding(0);
            
            for(int i=0;i<4;i++)
            {
                StratLogo slot = new(i);
                slot.Width.Set(slotSize, 0);
                slot.Height.Set(slotSize, 0);
                slot.Left.Set(padding, 0);
                slot.Top.Set(padding +(i* (padding + slotSize)), 0);
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

    public class StratLogo(int slot) : UIElement
    {
        /*This class should contain the logo and the name of the stratagem*/
        private readonly int slotIndex = slot;
        public override void OnInitialize()
        {
            ArrowSequence sequenceUI = new(slotIndex);
            sequenceUI.Left.Set(60, 0f);
            sequenceUI.Top.Set(26, 0f);
            sequenceUI.Width.Set(144, 0f);
            sequenceUI.Height.Set(20, 0f);
            Append(sequenceUI);
            CooldownUI cooldownUI = new(slotIndex);
            cooldownUI.Left.Set(0, 0f);
            cooldownUI.Top.Set(0, 0f);
            cooldownUI.Width.Set(52, 0f);
            cooldownUI.Height.Set(52, 0f);
            Append(cooldownUI);
            StratNameUI stratName = new(slotIndex);
            stratName.Left.Set(60, 0f);
            stratName.Top.Set(6, 0f);
            stratName.Width.Set(144, 0f);
            stratName.Height.Set(20, 0f);
            Append(stratName);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
            if (modPlayer.stratagems[slotIndex] != null)
            {
                Item currentItem = modPlayer.stratagems[slotIndex].Item;
                if (currentItem != null && !currentItem.IsAir)
                {
                    Main.instance.LoadItem(currentItem.type);
                    Texture2D itemTexture = TextureAssets.Item[currentItem.type].Value;
                    
                    float itemScale = 1f;
                    Vector2 position = new(
                        dimensions.X + (dimensions.Width / 2f) - (itemTexture.Width * itemScale / 2f),
                        dimensions.Y + (dimensions.Height / 2f) - (itemTexture.Height * itemScale / 2f)
                    );
                    Color renderColor = modPlayer.stratagems[slotIndex].isInCD ? Color.Gray * 0.5f : Color.White;
                    spriteBatch.Draw(itemTexture, position, null, renderColor, 0f, Vector2.Zero, itemScale, SpriteEffects.None, 0f);
                }
            }
        }
    }
    public class StratNameUI(int slot) : UIElement
    {
        private readonly int slotIndex = slot;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
            StratagemPlaceable strat = modPlayer.stratagems[slotIndex];
            if ( strat == null || strat.Item == null)
                return;
            CalculatedStyle dimensions = GetDimensions();
            Vector2 startDrawPos = new(dimensions.X, dimensions.Y);
            Color textColor = strat.isInCD ? Color.Gray * 0.7f : Color.White;
            Utils.DrawBorderString(spriteBatch, strat.Name, startDrawPos, textColor, 0.75f);
        }
    }
    public class ArrowSequence(int slot) : UIElement
    {
        /*This class should contain the 2 layers of arrow that form the sequence to press */
        private readonly int slotIndex = slot;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
            
            if (slotIndex >= modPlayer.stratagems.Length || modPlayer.stratagems[slotIndex] == null) 
                return;
            StratagemPlaceable strat = modPlayer.stratagems[slotIndex];
            if (strat.isInCD) 
                return;
            CalculatedStyle dimensions = GetDimensions();
            int completedCount = strat.sequence.Count - strat.currentSequence.Count;
            float scale = 3f;
            float spacingX = 26f;
            float totalWidth = strat.sequence.Count * spacingX;
            Vector2 startDrawPos = new(dimensions.X + (dimensions.Width / 2f) - (totalWidth / 2f), dimensions.Y);

            for (int i = 0; i < strat.sequence.Count; i++)
            {
                Arrows arrowType = strat.sequence[i];
                string arrowCharacter = arrowType switch
                {
                    Arrows.UP => "U",
                    Arrows.DOWN => "D",
                    Arrows.LEFT => "L",
                    Arrows.RIGHT => "R",
                    _ => "?"
                };
                Color arrowColor = Color.White;
                if (i < completedCount) arrowColor = (i == completedCount - 1) ? new Color(255, 222, 0) : Color.Gray * 0.5f;
    
                Utils.DrawBorderString(spriteBatch, arrowCharacter, startDrawPos + new Vector2(i * spacingX, 0), arrowColor, scale);
            }
        }
    }
    public class CooldownUI(int slot) : UIElement
    {
        /*This class replace the arrow when its in cooldown*/
        private readonly int slotIndex = slot;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            HDPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HDPlayer>();
            
            if (slotIndex >= modPlayer.stratagems.Length || modPlayer.stratagems[slotIndex] == null) 
                return;

            StratagemPlaceable strat = modPlayer.stratagems[slotIndex];
            if (!strat.isInCD) 
                return;

            CalculatedStyle dimensions = GetDimensions();
            Texture2D overlayBackground = TextureAssets.MagicPixel.Value;
            float ratio = (float)strat.cooldown / strat.OriginalCD;
            
            Rectangle maskArea = new(
                (int)dimensions.X, 
                (int)(dimensions.Y + (dimensions.Height * (1f - ratio))), 
                (int)dimensions.Width, 
                (int)(dimensions.Height * ratio)
            );
            spriteBatch.Draw(overlayBackground, maskArea, Color.Black * 0.65f);

            string timerText = $"{Math.Ceiling(strat.cooldown / 60f)}s";
            Vector2 stringSize = FontAssets.MouseText.Value.MeasureString(timerText);
            Vector2 textPos = new(dimensions.X + (dimensions.Width / 2f) - (stringSize.X * 0.8f / 2f),dimensions.Y + (dimensions.Height / 2f) - (stringSize.Y * 0.8f / 2f));

            Utils.DrawBorderString(spriteBatch, timerText, textPos, Color.Red, 0.8f);
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class ArrowsSystem : ModSystem //The system regarding the ui
    {
        internal UserInterface userInterface;
        internal ArrowState uiState;

        public override void Load()
        {
            if (!Main.dedServ) 
            {
                userInterface = new UserInterface();
                uiState = new ArrowState();
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
