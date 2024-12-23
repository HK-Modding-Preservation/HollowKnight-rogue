using System.Collections.Generic;
namespace rogue; internal static class CharmHelper
{
    internal static HashSet<string> can_stand_equip_charm_scene_names = new()
    {
    "GG_Atrium_Roof",
    "GG_Engine",
    };
    internal static HashSet<int> cant_unequip_charm = new()
    {

    };
    internal static Dictionary<int, int> custom_cost = new();
}