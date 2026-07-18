using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace HellDivers2.Content.System
{
    public enum Arrows
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    public enum Colors
    {
        red,
        blue
    }
    public abstract class StratagemPlaceable : ModItem
    {
        /*
        This class represents the item you put in the bracelet
        */
        public List<Arrows> sequence = [];
        public List<Arrows> currentSequence;
        public StratagemItem givenStrat;
        public int cooldown;
        public int OriginalCD;
        public bool isInCD=false;
        public void resetSequence() => currentSequence= [.. sequence];
        public string showSeq()
        {
            string res="";
            foreach( var arr in sequence)
            {
                switch (arr)
                {
                    case Arrows.UP:
                        res+="UP ";
                        break;
                    case Arrows.DOWN:
                        res+="DOWN ";
                        break;
                    case Arrows.LEFT:
                        res+="LEFT ";
                        break;
                    case Arrows.RIGHT:
                        res+="RIGHT ";
                        break;
                }
            }
            return res;
        }
    }
    public abstract class StratagemItem : ModItem
    {
        /*
        This class represents the item you get after giving the right sequence with the bracelet
        */
        public Colors color;
        public int timeToDetonate;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if(proj.ModProjectile is StratagemEntity strat)
            {
                strat.timeToDetonate=timeToDetonate;
                strat.color=color;
            }
            return false;
        }
    }
    public abstract class StratagemEntity : ModProjectile
    {
        /*
        This class represents the projectile that spawn after throwing the stratagem
        */
        public Colors color;
        public int timeToDetonate { get; set; }
        public static float treshold = 4f;
        public bool isMoving =true;
        private static Asset<Effect> beamShader;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }
        public override void SetStaticDefaults() {
			if (!Main.dedServ) {
				beamShader = ModContent.Request<Effect>("HellDivers2/Content/Assets/Effects/Shaders/StratagemBeam", AssetRequestMode.ImmediateLoad);
			}
		}
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true; 
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!isMoving) return false;
            if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = oldVelocity.X * -0.4f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f) {
				Projectile.velocity.Y = oldVelocity.Y * -0.4f;
			}
            if(Math.Abs(Projectile.velocity.X)<treshold*2 && Math.Abs(Projectile.velocity.Y)<treshold)//make the X treshold bigger beecause projectile tends to move more on the horizontal
            {
                Projectile.velocity=Vector2.Zero;
                isMoving=false;
            }
			return false;
        }
        public override void AI()
        {
            if(isMoving){
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] > 15f) {
                    if (Projectile.velocity.Y == 0f) {
                        Projectile.velocity.X *= 0.95f;
                    }
                    Projectile.velocity.Y += 0.2f;
                }
                Projectile.rotation += Projectile.velocity.X * 0.1f;
            }
            else
            {
                if(timeToDetonate>0) timeToDetonate--;
                else
                {
                    onDetonate();
                }
            }
            
            
        }
        public abstract void onDetonate();
        public override void PostDraw(Color lightColor)
        {
            if (!isMoving)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, beamShader.Value, Main.GameViewMatrix.TransformationMatrix);

                Effect effect = beamShader.Value;
                effect.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
                effect.Parameters["uColor"]?.SetValue((color == Colors.blue) ? new Vector3(0f, 0.5f, 1f) : new Vector3(1f, 0f, 0f));
                effect.CurrentTechnique.Passes[0].Apply();
                int width = 10;
                int height = 480;
                Vector2 drawPos = Projectile.Center - Main.screenPosition - new Vector2(width / 2, height);
                Rectangle destination = new Rectangle((int)drawPos.X, (int)drawPos.Y, width, height);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, destination, Color.White);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}