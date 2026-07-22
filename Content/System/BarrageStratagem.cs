using Microsoft.Xna.Framework;
using HellDivers2.Content.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;
namespace HellDivers2.Content.Stratagems
{
    public struct MissileStats
    {
        public Vector2 velocity;
        public Vector2 explosionRadius;
        public int damage;
        public int fuse;
    }


    public class BarrageStratagem : StratagemEntity
    {

        public MissileStats stats;
        protected Vector2 originalPosition;
        protected bool isActive = false;
        protected int strikeTimer = 0;
        protected int nbFired = 0;
        protected static Asset<Effect> beamShader;
        protected int totalNb;
        protected int delayBetweenMin;
        protected int delayBetweenMax;
        protected int delayBetween;
        protected float radius;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ) {
				beamShader = ModContent.Request<Effect>("HellDivers2/Content/Assets/Effects/Shaders/StratagemBeam", AssetRequestMode.ImmediateLoad);
			}
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 0;
        }
        public static int SpawnBarrage<T>(Vector2 position,Vector2 velocity, MissileStats missileStats, int totalNb, int delayMin, int delayMax, float radius) where T : BarrageStratagem
        {
            Projectile projectile = Projectile.NewProjectileDirect(
                null, 
                position, 
                velocity, 
                ModContent.ProjectileType<T>(), 
                0, 
                0f, 
                -1
            );
            if (projectile.ModProjectile is T barrage)
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
            Main.NewText("Fire Mission: "+Type+" Barrage Inbound!", Color.Red); 
            isActive = true;
            Projectile.alpha = 255;
            originalPosition = Projectile.Center;
            delayBetween = Main.rand.Next(delayBetweenMin,delayBetweenMax+1);
        }
        public override void AI()
        {
            if (isActive)
            {
                strikeTimer++;
                if (strikeTimer >= delayBetween)
                {   
                    strikeTimer = 0;
                    delayBetween = Main.rand.Next(delayBetweenMin,delayBetweenMax+1);
                    float randomDistance = Main.rand.NextFloat(-radius, radius);
                    Vector2 target = Projectile.position+ new Vector2(randomDistance,0f);
                    SpawnPayload(target);
                    nbFired++;

                    if (nbFired >= totalNb)
                    {
                        Main.NewText("Barrage complete. Returning to orbit.");
                        Projectile.Kill();
                    }
                }
            }
        }

        protected virtual void SpawnPayload(Vector2 target)
        {
            Missile.SpawnMissile(
                target,
                stats,
                true
            );
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
    public class BarrageHEStratagem : BarrageStratagem
    {
        protected override void SpawnPayload(Vector2 target)
        {
            Main.NewText("Classic");
            Missile.SpawnMissile(
                target,
                stats,
                true
            );
        }
    }
    public class BarrageNapalmStratagem : BarrageStratagem
    {
        protected override void SpawnPayload(Vector2 target)
        {
            Main.NewText("napalm");
            NapalmMissile.SpawnMissile(
                target,
                stats,
                true
            );
        }
    }
    public class BarrageGatlingStratagem : BarrageHEStratagem
    {
        protected override void SpawnPayload(Vector2 target)
        {
            Main.NewText("gatling");
            GatlingMissile.SpawnMissile(
                target,
                stats,
                true
            );
        }
    }
}