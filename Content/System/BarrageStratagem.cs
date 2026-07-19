using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HellDivers2.Content.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;
using System;
using Microsoft.Build.Evaluation;
using Stubble.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace HellDivers2.Content.Stratagems
{
    public struct MissileStats
    {
        public Vector2 velocity;
        public Vector2 explosionRadius;
        public int damage;
        public int fuse;
    }


    public class BarrageStratagemEntity : StratagemEntity
    {

        public MissileStats stats;
        private Vector2 originalPosition;
        private bool isActive = false;
        private int strikeTimer = 0;
        private int nbFired = 0;
        private static Asset<Effect> beamShader;
        private int totalNb;
        private int delayBetweenMin;
        private int delayBetweenMax;
        private int delayBetween;
        private float radius;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ) {
				beamShader = ModContent.Request<Effect>("HellDivers2/Content/Assets/Effects/Shaders/StratagemBeam", AssetRequestMode.ImmediateLoad);
			}
        }

        public static int SpawnBarrage(Vector2 position,Vector2 velocity, MissileStats missileStats, int totalNb, int delayMin, int delayMax, float radius)
        {
            Projectile projectile = Projectile.NewProjectileDirect(
                null, 
                position, 
                velocity, 
                ModContent.ProjectileType<BarrageStratagemEntity>(), 
                0, 
                0f, 
                -1
            );
            if (projectile.ModProjectile is BarrageStratagemEntity barrage)
            {
                barrage.stats = missileStats;
                barrage.totalNb = totalNb;
                barrage.delayBetweenMin = delayMin;
                barrage.delayBetweenMax = delayMax;
                barrage.radius = radius;
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
                }
            }
            return projectile.whoAmI;
        }
        public override void onDetonate()
        {
            if (isActive) return; 
            Main.NewText("Fire Mission: Small HE Barrage Inbound!", Color.Red); 
            isActive = true;
            Projectile.velocity = stats.velocity;
            Projectile.alpha = 255;
            originalPosition = Projectile.Center;
            delayBetween = Main.rand.Next(delayBetweenMin,delayBetweenMax+1);
        }
        public override void AI()
        {
            base.AI(); 
            if (isActive)
            {
                strikeTimer++;
                if (strikeTimer >= delayBetween)
                {
                    strikeTimer = 0;
                    delayBetween = Main.rand.Next(delayBetweenMin,delayBetweenMax+1);
                    float randomDistance = Main.rand.NextFloat(-radius, radius);
                    Vector2 target = Projectile.position+ new Vector2(randomDistance,0f);
                    Missile.SpawnMissile(
                        target,
                        stats,
                        true
                    );

                    nbFired++;

                    if (nbFired >= totalNb)
                    {
                        Main.NewText("Barrage complete. Returning to orbit.");
                        Projectile.Kill();
                    }
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, beamShader.Value, Main.GameViewMatrix.TransformationMatrix);

            Effect effect = beamShader.Value;
            effect.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["uColor"]?.SetValue(new Vector3(1f, 0f, 0f));
            effect.CurrentTechnique.Passes[0].Apply();
            int width = 10;
            int height = 480;
            Vector2 drawPos = originalPosition - Main.screenPosition - new Vector2(width / 2, height);
            Rectangle destination = new Rectangle((int)drawPos.X, (int)drawPos.Y, width, height);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, destination, Color.White);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}