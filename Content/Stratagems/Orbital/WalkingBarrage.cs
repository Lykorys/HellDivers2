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
    public class WalkingBarragePlaceable : StratagemPlaceable
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            sequence = new List<Arrows> { Arrows.RIGHT, Arrows.RIGHT, Arrows.DOWN, Arrows.LEFT, Arrows.RIGHT,Arrows.DOWN };
            givenStrat = ModContent.GetInstance<WalkingBarrageStratagem>();
            cooldown = 180;
            OriginalCD=180;
            resetSequence();
        }
    }
    public class WalkingBarrageStratagem : StratagemItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            timeToDetonate=120;
            color=System.Colors.red;
            Item.shoot = ModContent.ProjectileType<WalkingBarrageStratagemEntity>();
        }
    }
    public class WalkingBarrageStratagemEntity : StratagemEntity
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
            int projIndex = BarrageStratagem.SpawnBarrage<BarrageHEStratagem>(
                Projectile.Center, 
                new Vector2(2,0),
                missile, 
                15, 
                35, 
                55, 
                15*16
            );
            if (projIndex >= 0 && projIndex < Main.maxProjectiles && Main.projectile[projIndex].ModProjectile is BarrageStratagem barrage) barrage.onDetonate();
            Projectile.Kill();//TODO fix projectile despawning
        }
    }
}