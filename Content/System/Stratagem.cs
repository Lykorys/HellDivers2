using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HellDivers2.Content.System
{
    public enum Arrows
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    public enum Color
    {
        RED,
        BLUE
    }
    public abstract class Stratagem : ModItem
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
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = oldVelocity.X * -0.4f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f) {
				Projectile.velocity.Y = oldVelocity.Y * -0.4f;
			}
			return false;
        }
        public override void AI()
        {
            if(cooldown>0) cooldown--;
            else
            {
                onDetonate();
            }
            Projectile.ai[0] += 1f;
			if (Projectile.ai[0] > 15f) {
				if (Projectile.velocity.Y == 0f) {
					Projectile.velocity.X *= 0.95f;
				}
				Projectile.velocity.Y += 0.2f;
			}
			Projectile.rotation += Projectile.velocity.X * 0.1f;
        }
        public abstract void onDetonate();
    }
}