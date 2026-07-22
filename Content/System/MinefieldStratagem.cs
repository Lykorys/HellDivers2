using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace HellDivers2.Content.System
{
    public class MinefieldEntity : StratagemEntity
    {
        protected bool isActive = false;
        protected int strikeTimer = 0;
        protected int nbFired = 0;
        protected int totalNb;
        protected int delayBetweenMin;
        protected int delayBetweenMax;
        protected int delayBetween;
        protected float radius;
        public static int SpawnMinefield<T>(Vector2 position, int totalNb, int delayMin, int delayMax, float radius) where T : MinefieldEntity
        {
            Projectile projectile = Projectile.NewProjectileDirect(
                null, 
                position, 
                Vector2.Zero, 
                ModContent.ProjectileType<T>(), 
                0, 
                0f, 
                -1
            );
            if (projectile.ModProjectile is T minefield)
            {
                minefield.totalNb = totalNb;
                minefield.delayBetweenMin = delayMin;
                minefield.delayBetweenMax = delayMax;
                minefield.radius = radius;
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
                }
            }
            return projectile.whoAmI;
        }
        public override void onDetonate()
        {
            if (isActive) return; 
            Main.NewText("MineField "+Type+" Inbound!", Color.Red); 
            isActive = true;
            delayBetween = Main.rand.Next(delayBetweenMin,delayBetweenMax+1);
        }
        public override void AI()
        {
            if (isActive)
            {
                strikeTimer++;
                if (strikeTimer >= delayBetween)
                {   
                    strikeTimer = 0;
                    delayBetween = Main.rand.Next(delayBetweenMin,delayBetweenMax+1);
                    float randomDistance = Main.rand.NextFloat(-radius, radius);
                    Vector2 target = Projectile.position+ new Vector2(randomDistance,0f);
                    SpawnPayload(target);
                    nbFired++;
                    if (nbFired >= totalNb)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        protected virtual void SpawnPayload(Vector2 target)
        {
            Mine.SpawnMine(target);
        }

    }


    public class Mine : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.LandMine;
        public static int damage;
        public int radius;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if(proj.ModProjectile is not Mine && proj.Hitbox.Intersects(Projectile.Hitbox))
                {
                    proj.Kill();
                    onDetonate();
                    break;
                }
            }
        }
        public static void SpawnMineGeneric<T>(Vector2 target) where T : Mine
        {
            Projectile.NewProjectile(//TODO change so it spawn on the spawn and have the velocity move it to the spot
                null,
                target, 
                Vector2.Zero, 
                ModContent.ProjectileType<T>(), 
                damage, 
                0f, 
                Main.myPlayer
            );
        }
        public static void SpawnMine(Vector2 target) => SpawnMineGeneric<Mine>(target);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone){
            Main.NewText("HitNPC");
            onDetonate();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info){
            Main.NewText("HitPlayer");
            onDetonate();
        }
        public virtual void onDetonate()
        {
            Projectile.PrepareBombToBlow();
            Projectile.Kill();
        }

        public override void PrepareBombToBlow() {
            Projectile.tileCollide = false;
            Projectile.alpha = 255; 
            Projectile.Resize(radius*16, radius*16);
            Projectile.knockBack = 15f;
        }
        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
            for (int i = 0; i < 20; i++) {
                var smoke = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smoke.velocity *= 2.5f;
            }
            for (int j = 0; j < 20; j++) {
                var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                fireDust.noGravity = true;
                fireDust.velocity *= 7f;
                fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                fireDust.velocity *= 3f;
            }
            for (int k = 0; k < 6; k++) {
                var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3)), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.scale = 2f;
            }
        }
    }
}