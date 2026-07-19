//TODO create a missile class that has power as a parametre 
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Build.Evaluation;
using Terraria.Audio;

namespace HellDivers2.Content.Stratagems
{
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
        public static void SpawnMissile(Vector2 target,MissileStats stats, bool fallStraight = true)
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
                ModContent.ProjectileType<Missile>(), 
                stats.damage, 
                0f, 
                Main.myPlayer, 
                stats.explosionRadius.X, // ai[0]: Width
                stats.explosionRadius.Y, // ai[1]: Height
                stats.fuse      // ai[2]: Fuse 
            );
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
}