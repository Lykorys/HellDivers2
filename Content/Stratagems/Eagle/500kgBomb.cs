using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HellDivers2.Content.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace HellDivers2.Content.Stratagems
{
    public class KGBombPlaceable : StratagemPlaceable
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            sequence = new List<Arrows> { Arrows.UP, Arrows.RIGHT, Arrows.DOWN, Arrows.DOWN, Arrows.DOWN };
            givenStrat = ModContent.GetInstance<KGBombStratagem>();
            cooldown = 180;
            OriginalCD=180;
            resetSequence();
        }
    }
    public class KGBombStratagem : StratagemItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            timeToDetonate=120;
            color=System.Colors.red;
            Item.shoot = ModContent.ProjectileType<KGBombStratagemEntity>();
        }
    }
    public class KGBombStratagemEntity : StratagemEntity
    {

        public override void onDetonate()
        {
            Main.NewText("Calling in Orbital Strike!"); 
            MissileStats stats = new MissileStats
            {
                velocity = new(0f,0f),
                explosionRadius = new Vector2(40, 60),
                damage = 250,
                fuse=50
            };
            Missile.SpawnMissile(
                Projectile.Center,
                stats,
                false
            );
            Projectile.Kill();
        }
    }
    /*
    public class KGBombProjectile : ModProjectile
    {

        public override void SetStaticDefaults() {
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;
        }
        public override void SetDefaults() {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 90;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            {
                fallThrough = false;
                return true; 
            }
        public override void AI() {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
                Projectile.PrepareBombToBlow();
            }
            else {
                var smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100);
                smokeDust.scale *= 1f + Main.rand.Next(10) * 0.1f;
                smokeDust.velocity *= 0.2f;
                smokeDust.noGravity = true;
            }
            //TODO modify this so it fall out of the sky at an angle
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 15f) {
                if (Projectile.velocity.Y == 0f) {
                    Projectile.velocity.X *= 0.95f;
                }
                Projectile.velocity.Y += 0.2f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.velocity=new Vector2(0f,0f);
            return false;
        }

        public override void PrepareBombToBlow() {
            Projectile.tileCollide = false;
            Projectile.alpha = 255; 
            // Resize the hitbox of the projectile for the blast "radius".
            // Rocket I: 128, Rocket III: 200, Mini Nuke Rocket: 250
            // Measurements are in pixels, so 128 / 16 = 8 tiles.
            Projectile.Resize(40*16, 60*16);
            // Set the knockback of the blast.
            // Rocket I: 8f, Rocket III: 10f, Mini Nuke Rocket: 12f
            Projectile.knockBack = 15f;
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);


            // Spawn a bunch of smoke dusts.
            for (int i = 0; i < 80; i++) {
                var smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smoke.velocity *= 2.5f;
            }

            // Spawn a bunch of fire dusts.
            for (int j = 0; j < 300; j++) {
                var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                fireDust.noGravity = true;
                fireDust.velocity *= 7f;
                fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                fireDust.velocity *= 3f;
            }

            // Spawn a bunch of smoke gores.
            for (int k = 0; k < 6; k++) {
                var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3)), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.scale = 2f;
            }
        }
    }*/
}