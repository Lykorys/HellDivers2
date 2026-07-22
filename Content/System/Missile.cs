//TODO create a missile class that has power as a parametre 
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Build.Evaluation;
using Terraria.Audio;

namespace HellDivers2.Content.System
{
    public struct MissileStats
    {
        public Vector2 velocity;
        public Vector2 explosionRadius;
        public int damage;
        public int fuse;
    }

    public class Missile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RocketI;
        private int X;
        private int Y;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 2f;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
        public static void SpawnMissileGeneric<T>(Vector2 target,MissileStats stats, bool fallStraight = true) where T : Missile
        {
            float spawnDistance = 1200f;
            float radians;
            if (fallStraight)
            {
                radians = MathHelper.PiOver2; 
            }
            else
            {
                float randomDegrees = Main.rand.NextFloat(75f, 105f);
                Main.NewText(randomDegrees);
                radians = MathHelper.ToRadians(randomDegrees);
            }
            Vector2 direction = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
            Vector2 spawnPos = target - (direction * spawnDistance);
            Vector2 velocity = direction * 12f; 
            Projectile.NewProjectile(
                null,
                spawnPos, 
                velocity, 
                ModContent.ProjectileType<T>(), 
                stats.damage, 
                0f, 
                Main.myPlayer, 
                stats.explosionRadius.X, // ai[0]: Width
                stats.explosionRadius.Y, // ai[1]: Height
                stats.fuse      // ai[2]: Fuse 
            );
        }
        public static void SpawnMissile(Vector2 target, MissileStats stats, bool fallStraight = true)
        {
            SpawnMissileGeneric<Missile>(target, stats, fallStraight);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Projectile.velocity.ToRotation();
            Projectile.rotation = Projectile.localAI[0] + MathHelper.PiOver2;
        }
        public override void AI()
        {
            if(Projectile.velocity!=Vector2.Zero)Projectile.velocity.Y += 0.2f; 
            else
            {
                
                if(Projectile.ai[2]!=0) Projectile.ai[2]--;
                else
                {
                    X = (int)Projectile.ai[0];
                    Y= (int)Projectile.ai[1];
                    Projectile.PrepareBombToBlow();
                    Projectile.Kill();
                }
            }
        }

        public override void PrepareBombToBlow() {
            Projectile.tileCollide = false;
            Projectile.alpha = 255; 
            Projectile.Resize(X*16, Y*16);
            Projectile.knockBack = 15f;
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            for (int i = 0; i < 80; i++) {
                var smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smoke.velocity *= 2.5f;
            }
            for (int j = 0; j < 300; j++) {
                var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                fireDust.noGravity = true;
                fireDust.velocity *= 7f;
                fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                fireDust.velocity *= 3f;
            }
            for (int k = 0; k < 6; k++) {
                var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3)), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.scale = 2f;
            }
        }
        
    }



    public class NapalmMissile : Missile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RocketII;
        public new static void SpawnMissile(Vector2 target, MissileStats stats, bool fallStraight = true)
        {
            SpawnMissileGeneric<NapalmMissile>(target, stats, fallStraight);
        }
        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
            int nbFireProjectiles = Main.rand.Next(13, 16);
            for (int i = 0; i < nbFireProjectiles; i++) {
                Vector2 fireVelocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f));
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    fireVelocity,
                    ModContent.ProjectileType<FireResidue>(),
                    (int)(Projectile.damage * 0.6f),
                    0f,
                    Projectile.owner
                );
            }
        }
    }
    public class FireResidue : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_" + ProjectileID.MolotovFire;
        
        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.scale = 2f;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }

        public override void AI() {
            Projectile.velocity.Y += 0.2f;
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.5f);
            d.noGravity = true;
            Lighting.AddLight(Projectile.Center, 0.8f, 0.4f, 0.1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.velocity.X *= 0.5f;
            if (Projectile.velocity.Y != oldVelocity.Y) {
                Projectile.velocity.Y = 0f;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.OnFire, 240);
        }
    }
    public class GatlingMissile : Missile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RocketII;
        public new static void SpawnMissile(Vector2 target, MissileStats stats, bool fallStraight = true)
        {
            float spawnDistance = 1200f;
            float radians = fallStraight ? MathHelper.PiOver2 : MathHelper.ToRadians(Main.rand.NextFloat(75f, 105f));

            Vector2 direction = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
            Vector2 spawnPos = target - (direction * spawnDistance);
            Vector2 velocity = direction * 16f; 

            Projectile.NewProjectile(
                null,
                spawnPos,
                velocity,
                ProjectileID.BulletHighVelocity,
                stats.damage,
                1f,
                Main.myPlayer
            );

            SoundEngine.PlaySound(SoundID.Item11, spawnPos);
        }
        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
            int nbFireProjectiles = Main.rand.Next(13, 16);
            for (int i = 0; i < nbFireProjectiles; i++) {
                Vector2 fireVelocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-6f, -2f));
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    fireVelocity,
                    ModContent.ProjectileType<FireResidue>(),
                    (int)(Projectile.damage * 0.6f),
                    0f,
                    Projectile.owner
                );
            }
        }
    }
}