using Microsoft.Xna.Framework;
using HellDivers2.Content.System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace HellDivers2.Content.Stratagems
{
    public class AntiTankMinefieldPlaceable : StratagemPlaceable
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            sequence = new List<Arrows> { Arrows.RIGHT, Arrows.RIGHT, Arrows.DOWN, Arrows.LEFT, Arrows.RIGHT,Arrows.DOWN };//TODO
            givenStrat = ModContent.GetInstance<AntiTankMinefieldStratagem>();
            cooldown = 180;
            OriginalCD=180;
            resetSequence();
        }
    }
    public class AntiTankMinefieldStratagem : StratagemItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            timeToDetonate=120;
            color=System.Colors.red;
            Item.shoot = ModContent.ProjectileType<AntiTankMinefieldStratagemEntity>();
        }
    }
    public class AntiTankMinefieldStratagemEntity : StratagemEntity
    {

        private bool isActive = false;
        public override void onDetonate()
        {
            if (isActive) return; 
            isActive = true;
            Projectile.alpha = 255;
            int projIndex = MinefieldEntity.SpawnMinefield<AntiTankMinefieldEntity>(
                Projectile.Center,
                7,
                10,
                15,
                radius : 6*16
            );
            if (projIndex >= 0 && projIndex < Main.maxProjectiles && Main.projectile[projIndex].ModProjectile is MinefieldEntity minefield) minefield.onDetonate();
            Projectile.Kill();//TODO fix projectile despawning
        }
    }


    public class AntiTankMinefieldEntity : MinefieldEntity
    {
        protected override void SpawnPayload(Vector2 target)
        {
            AntiTankMine.SpawnMine(target);
        }
    }
    public class AntiTankMine : Mine
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            damage=450;
            radius=2;
        }
        public static new void SpawnMine(Vector2 target)
        {
            SpawnMineGeneric<AntiTankMine>(target);
        }
        public override void onDetonate()
        {
            Projectile.PrepareBombToBlow();
            Projectile.Kill();
        }
    }
}