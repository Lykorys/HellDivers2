using HellDivers2.Content.System;
using Microsoft.Build.Evaluation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class TestStratagem : Stratagem
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
        Item.maxStack = 99;
        Item.shootSpeed = 8f;

        cooldown = 180;
        Item.shoot = ModContent.ProjectileType<TestStratagemEntity>();
    }
}
public class TestStratagemEntity : StratagemEntity
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Grenade;
    public override void onDetonate()
    {
        Main.NewText("boom");
        Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.position,Projectile.velocity,ProjectileID.Grenade,10,1f);
        Projectile.Kill();
    }
}
/*

public class Stratagem : ModItem
    {
        public Arrows[] sequence;
        public int cooldown;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if(proj.ModProjectile is StratagemEntity strat) strat.cooldown=cooldown;
            return false;
        }
    }
    public abstract class StratagemEntity : ModProjectile
    {
        public int cooldown { get; set; }
        public override void AI()
        {
            if(cooldown>0) cooldown--;
            else
            {
                onDetonate();
            }
        }
        public abstract void onDetonate();
    }*/