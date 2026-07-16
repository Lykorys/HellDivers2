using System;
using System.Collections;
using System.Collections.Generic;
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
    public abstract class StratagemPlaceable : ModItem
    {
        /*
        This class represents the item you put in the bracelet
        */
        public List<Arrows> sequence = new();
        public List<Arrows> currentSequence;
        public StratagemItem givenStrat;
        public void resetSequence() => currentSequence= [.. sequence];
        public string showSeq()
        {
            string res="";
            foreach( var arr in sequence)
            {
                switch (arr)
                {
                    case Arrows.UP:
                        res+="UP ";
                        break;
                    case Arrows.DOWN:
                        res+="DOWN ";
                        break;
                    case Arrows.LEFT:
                        res+="LEFT ";
                        break;
                    case Arrows.RIGHT:
                        res+="RIGHT ";
                        break;
                }
            }
            return res;
        }
    }
    public abstract class StratagemItem : ModItem
    {
        /*
        This class represents the item you get after giving the right sequence with the bracelet
        */
        public int cooldown;
        public int timeToDetonate;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if(proj.ModProjectile is StratagemEntity strat) strat.timeToDetonate=timeToDetonate;
            return false;
        }
    }
    public abstract class StratagemEntity : ModProjectile
    {
        /*
        This class represents the projectile that spawn after throwing the stratagem
        */
        public int timeToDetonate { get; set; }
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
            if(timeToDetonate>0) timeToDetonate--;
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