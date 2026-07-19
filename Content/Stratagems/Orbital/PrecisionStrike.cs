using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HellDivers2.Content.System;
using Terraria;
using Terraria.ModLoader;
namespace HellDivers2.Content.Stratagems
{
    public class PrecisionStrikePlaceable : StratagemPlaceable
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            sequence = new List<Arrows> { Arrows.RIGHT, Arrows.RIGHT, Arrows.UP};
            givenStrat = ModContent.GetInstance<PrecisionStrikeStratagem>();
            cooldown = 180;
            OriginalCD=180;
            resetSequence();
        }
    }
    public class PrecisionStrikeStratagem : StratagemItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            timeToDetonate=120;
            color=Colors.red;
            Item.shoot = ModContent.ProjectileType<PrecisionStrikeStratagemEntity>();
        }
    }
    public class PrecisionStrikeStratagemEntity : StratagemEntity
    {
        public override void onDetonate()
        {
            Main.NewText("Calling in Orbital Strike!"); 
            MissileStats stats = new MissileStats
            {
                velocity = new(0f,200f),
                explosionRadius = new Vector2(10, 10),
                damage = 600,
                fuse=0
            };
            Missile.SpawnMissile(
                Projectile.Center,
                stats
            );
            Projectile.Kill();
        }
    }
}
