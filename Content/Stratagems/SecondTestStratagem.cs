using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using HellDivers2.Content.System;
using Microsoft.Build.Evaluation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace HellDivers2.Content.Stratagems
{
    public class SecondTestStratagemPlaceable : StratagemPlaceable
    {
        
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoneJavelin;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.consumable = false;
            Item.maxStack = 1;
            Item.shootSpeed = 8f;
            sequence = new List<Arrows> { Arrows.RIGHT, Arrows.DOWN, Arrows.RIGHT, Arrows.LEFT, Arrows.UP };
            givenStrat = ModContent.GetInstance<SecondTestStratagem>();
            cooldown = 180;
            OriginalCD=180;
            resetSequence();
        }
    }
    public class SecondTestStratagem : StratagemItem
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoneJavelin;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.consumable = true;
            Item.maxStack = 1;
            Item.shootSpeed = 8f;
            timeToDetonate=120;
            color=System.Colors.blue;
            Item.shoot = ModContent.ProjectileType<TestStratagemEntity>();
        }
    }
    public class SecondTestStratagemEntity : StratagemEntity
    {

        public override void onDetonate()
        {
            Main.NewText("boom2");//explose pas direct apres car fuse de la grenade
            Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.position,Projectile.velocity,ProjectileID.Grenade,10,1f);
            Projectile.Kill();
        }
    }
}