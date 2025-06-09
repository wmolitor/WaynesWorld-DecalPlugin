using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaynesWorld
{
    public partial class PluginCore
    {
        private double DistanceToSelf(WorldObject obj)
        {
            return CoreManager.Current.WorldFilter.Distance(Core.CharacterFilter.Id, obj.Id);
        }

        ///////////////////////////////////////
        // Find Healing Kit
        private int Find_Heal_Kit()
        {
            int id = 0;

            try
            {
                WorldObjectCollection w_oc = Core.WorldFilter.GetInventory();
                IEnumerator<WorldObject> w_enum = w_oc.GetEnumerator();
                WorldObject w_obj;

                if (w_oc.Count > 0)
                {
                    do
                    {
                        w_obj = w_enum.Current;
                        if (w_obj.ObjectClass == ObjectClass.HealingKit)
                        {
                            id = w_obj.Id;
                            break;
                        }
                    } while (w_enum.MoveNext());
                }
            }

            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }

            return (id);
        }

        ///////////////////////////////////////
        /// Dictionary of Materials
        /// <summary>
        /// Look up material name by ID
        /// 
        static Dictionary<int, string> materials = new Dictionary<int, string>
        {
            { 0x00000000, "Not Found" },
            { 0x00000001, "Ceramic" },
            { 0x00000002, "Porcelain" },
            { 0x00000004, "Linen" },
            { 0x00000005, "Satin" },
            { 0x00000006, "Silk" },
            { 0x00000007, "Velvet" },
            { 0x00000008, "Wool" },
            { 0x0000000A, "Agate" },
            { 0x0000000B, "Amber" },
            { 0x0000000C, "Amethyst" },
            { 0x0000000D, "Aquamarine" },
            { 0x0000000E, "Azurite" },
            { 0x0000000F, "Black Garnet" },
            { 0x00000010, "Black Opal" },
            { 0x00000011, "Bloodstone" },
            { 0x00000012, "Carnelian" },
            { 0x00000013, "Citrine" },
            { 0x00000014, "Diamond" },
            { 0x00000015, "Emerald" },
            { 0x00000016, "Fire Opal" },
            { 0x00000017, "Green Garnet" },
            { 0x00000018, "Green Jade" },
            { 0x00000019, "Hematite" },
            { 0x0000001A, "Imperial Topaz" },
            { 0x0000001B, "Jet" },
            { 0x0000001C, "Lapis Lazuli" },
            { 0x0000001D, "Lavender Jade" },
            { 0x0000001E, "Malachite" },
            { 0x0000001F, "Moonstone" },
            { 0x00000020, "Onyx" },
            { 0x00000021, "Opal" },
            { 0x00000022, "Peridot" },
            { 0x00000023, "Red Garnet" },
            { 0x00000024, "Red Jade" },
            { 0x00000025, "Rose Quartz" },
            { 0x00000026, "Ruby" },
            { 0x00000027, "Sapphire" },
            { 0x00000028, "Smokey Quartz" },
            { 0x00000029, "Sunstone" },
            { 0x0000002A, "Tiger Eye" },
            { 0x0000002B, "Tourmaline" },
            { 0x0000002C, "Turquoise" },
            { 0x0000002D, "White Jade" },
            { 0x0000002E, "White Quartz" },
            { 0x0000002F, "White Sapphire" },
            { 0x00000030, "Yellow Garnet" },
            { 0x00000031, "Yellow Topaz" },
            { 0x00000032, "Zircon" },
            { 0x00000033, "Ivory" },
            { 0x00000034, "Leather" },
            { 0x00000035, "Armoredillo Hide" },
            { 0x00000036, "Gromnie Hide" },
            { 0x00000037, "Reed Shark Hide" },
            { 0x00000039, "Brass" },
            { 0x0000003A, "Bronze" },
            { 0x0000003B, "Copper" },
            { 0x0000003C, "Gold" },
            { 0x0000003D, "Iron" },
            { 0x0000003E, "Pyreal" },
            { 0x0000003F, "Silver" },
            { 0x00000040, "Steel" },
            { 0x00000042, "Alabaster" },
            { 0x00000043, "Granite" },
            { 0x00000044, "Marble" },
            { 0x00000045, "Obsidian" },
            { 0x00000046, "Sandstone" },
            { 0x00000047, "Serpentine" },
            { 0x00000049, "Ebony" },
            { 0x0000004A, "Mahogany" },
            { 0x0000004B, "Oak" },
            { 0x0000004C, "Pine" },
            { 0x0000004D, "Teak" }
        };

    }
}
