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
    internal static bool can_equip_everywhere = false;
    internal static bool SetCantUnequip(int charm)
    {
        PlayerData.instance.SetBool("gotCharm_" + charm, true);
        if (!PlayerData.instance.equippedCharms.Contains(charm))
        {
            PlayerData.instance.equippedCharms.Add(charm);
            PlayerData.instance.SetBool("equippedCharm_" + charm, true);
        }
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
        if (cant_unequip_charm.Contains(charm))
        {
            return false;
        }
        cant_unequip_charm.Add(charm);
        return true;
    }
    internal static bool SetCanEquip(int charm)
    {
        if (cant_unequip_charm.Contains(charm))
        {
            cant_unequip_charm.Remove(charm);
            return true;
        }
        return false;
    }
}