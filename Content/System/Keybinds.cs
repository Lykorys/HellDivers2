using Terraria.ModLoader;
namespace HellDivers2.Content.Config
{
	public class KeybindSystem : ModSystem
	{
		public static ModKeybind LeftArrow { get; private set; }
        public static ModKeybind RightArrow { get; private set; }
        public static ModKeybind UpArrow { get; private set; }
        public static ModKeybind DownArrow { get; private set; }

		public override void Load() {
			LeftArrow = KeybindLoader.RegisterKeybind(Mod, "LeftArrow", "q");
            RightArrow = KeybindLoader.RegisterKeybind(Mod, "RightArrow", "d");
            UpArrow = KeybindLoader.RegisterKeybind(Mod, "UpArrow", "z");
            DownArrow = KeybindLoader.RegisterKeybind(Mod, "DownArrow", "s");

		}
		public override void Unload() {
			LeftArrow = null;
            RightArrow = null;
            UpArrow = null;
            DownArrow = null;
		}
	}
}