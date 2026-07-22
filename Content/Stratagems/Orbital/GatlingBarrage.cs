using System.Collections.Generic;
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
    public class GatlingBarragePlaceable : StratagemPlaceable
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            sequence = new List<Arrows> { Arrows.RIGHT, Arrows.RIGHT, Arrows.DOWN, Arrows.LEFT, Arrows.RIGHT,Arrows.DOWN };//TODO
            givenStrat = ModContent.GetInstance<GatlingBarrageStratagem>();
            cooldown = 180;
            OriginalCD=180;
            resetSequence();
        }
    }
    public class GatlingBarrageStratagem : StratagemItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            timeToDetonate=120;
            color=System.Colors.red;
            Item.shoot = ModContent.ProjectileType<GatlingBarrageStratagemEntity>();
        }
    }
    public class GatlingBarrageStratagemEntity : StratagemEntity
    {

        private bool isActive = false;
        public override void onDetonate()
        {
            if (isActive) return; 
            isActive = true;
            Projectile.alpha = 255;
            MissileStats missile = new MissileStats
            {
                velocity = new Vector2(0f, 45f),
                explosionRadius = new Vector2(8f, 8f),
                damage = 250
            };
            int projIndex = BarrageStratagem.SpawnBarrage<BarrageGatlingStratagem>(//TODO stats
                Projectile.Center, 
                Vector2.Zero,
                missile, 
                50, 
                0, 
                5, 
                15*16
            );
            if (projIndex >= 0 && projIndex < Main.maxProjectiles && Main.projectile[projIndex].ModProjectile is BarrageStratagem barrage) barrage.onDetonate();
            Projectile.Kill();//TODO fix projectile despawning
        }
    }
}