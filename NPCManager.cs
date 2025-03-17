using rogue;
using rogue.NPCs;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.UIElements;


internal static class NPCManager
{
    static List<Type> types = new List<Type>(){
        typeof(NPC),
    };
    internal static Dictionary<string, NPC> npcs = new();
    const string elderbug_scene = "Town";
    const string elderbug_go = "_NPCs/Elderbug";
    const string jiji_scene = "Room_Ouiji";
    const string jiji_name = "Jiji NPC";
    const string jinn_scene = "Room_Jinn";
    const string jinn_name = "Jinn NPC";
    const string mask_maker_scene = "Room_Mask_Maker";
    const string mask_maker_mask = "Maskmaker NPC";
    const string mask_maker_body = "mm_body";
    const string mask_maker_desk = "Mask_Maker_Egg_Chamber_0008_Mid";
    const string xun_scene = "Room_Mansion";
    const string xun_name = "Xun NPC";
    const string emilitia_scene = "Ruins_House_03";
    const string emilitia_name = "Emilitia NPC";
    const string nailsmith_scene = "Room_nailsmith";
    const string nailsmith_name = "Nailsmith";
    const string quirrel_nail_scene = "Crossroads_50";
    const string quirrel_nail_name = "quirrel_death_nail";
    const string charm_slug_scene = "Room_Charm_Shop";
    const string charm_slug_name = "Charm Slug";
    const string charm_slug_desk = "shop_0000_a";
    internal static List<(string, string)> GetPreloadNames()
    {
        List<(string, string)> values = new List<(string, string)>()
        {
            (elderbug_scene,elderbug_go),
            (jiji_scene,jiji_name),
            (jinn_scene,jinn_name),
            (xun_scene,xun_name),
            (emilitia_scene,emilitia_name),
            (nailsmith_scene,nailsmith_name),
            (quirrel_nail_scene,quirrel_nail_name),
            (charm_slug_scene,charm_slug_name),
            (charm_slug_scene,charm_slug_desk)
        };
        return values;

    }
    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        NPC.template = GameObject.Instantiate(preloadedObjects[Rogue.card_scene][Rogue.card_name]);
        NPC.template.SetActive(false);
        GameObject.DontDestroyOnLoad(NPC.template);
        npcs.Add(typeof(ElderBug).Name, new ElderBug(preloadedObjects[elderbug_scene][elderbug_go].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Jiji).Name, new Jiji(preloadedObjects[jiji_scene][jiji_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Jinn).Name, new Jinn(preloadedObjects[jinn_scene][jinn_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Xun).Name, new Xun(preloadedObjects[xun_scene][xun_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Emilitia).Name, new Emilitia(preloadedObjects[emilitia_scene][emilitia_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(QuirrelNail).Name, new QuirrelNail(preloadedObjects[quirrel_nail_scene][quirrel_nail_name].GetComponent<SpriteRenderer>().sprite));
        npcs.Add(typeof(CharmSlug).Name, new CharmSlug(preloadedObjects[charm_slug_scene][charm_slug_name].GetComponent<tk2dSpriteAnimator>().Library, GameObject.Instantiate(preloadedObjects[charm_slug_scene][charm_slug_desk])));
        npcs.Add(typeof(Nailsmith).Name, new Nailsmith(preloadedObjects[nailsmith_scene][nailsmith_name].GetComponent<tk2dSpriteAnimator>().Library));
    }

}