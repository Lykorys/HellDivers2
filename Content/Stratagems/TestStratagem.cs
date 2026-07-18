using System.Collections;
using System.Collections.Generic;
using HellDivers2.Content.System;
using Microsoft.Build.Evaluation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class TestStratagemPlaceable : StratagemPlaceable
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
        sequence = new List<Arrows> { Arrows.UP, Arrows.DOWN, Arrows.RIGHT, Arrows.LEFT, Arrows.UP };
        givenStrat =  ModContent.GetInstance<TestStratagem>();
        cooldown = 180;
        OriginalCD=180;
        
        resetSequence();
    }
}
public class TestStratagem : StratagemItem
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
        color=HellDivers2.Content.System.Colors.blue;
        Item.shoot = ModContent.ProjectileType<TestStratagemEntity>();
    }
}
public class TestStratagemEntity : StratagemEntity
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Grenade;
    public override void onDetonate()
    {
        Main.NewText("boom");//explose pas direct apres car fuse de la grenade
        Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.position,Projectile.velocity,ProjectileID.Grenade,10,1f);
        Projectile.Kill();
    }
}