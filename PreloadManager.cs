using rogue;
using rogue.Characters;
using rogue.ModBosses;

internal static class PreloadManager
{

    internal const string hive_scene = "Hive_03_c";
    internal const string hive_big_bee = "Big Bee";
    internal const string hive_bee_stinger = "Bee Stinger (5)";
    internal const string collector_scene = "GG_Collector";
    internal const string collector_name = "Battle Scene/Jar Collector";

    internal const string mushroom_scene = "Fungus2_29";
    internal const string bow_mushroom_name = "Mushroom Brawler";
    internal const string roller_mushroom_name = "Mushroom Roller (3)";

    internal const string grimm_scene = "Hive_03";
    internal const string grimm_name = "Flamebearer Spawn";
    internal const string hk_scene = "GG_Hollow_Knight";
    internal const string hk_slash_effect_name = "Battle Scene/HK Prime/Slashes";
    internal const string false_knight_scene = "GG_False_Knight";
    internal const string false_knight_name = "Battle Scene/False Knight New";



    internal static Dictionary<string, Dictionary<string, GameObject>> ReservedGO = new();

    internal static List<(string, string)> GetPreloadNames()
    {
        return new()
        {
            (hive_scene,hive_big_bee),
            (hive_scene,hive_bee_stinger),
            (collector_scene,collector_name),
            (mushroom_scene,bow_mushroom_name),
            (mushroom_scene,roller_mushroom_name),
            (grimm_scene,grimm_name),
            (hk_scene,hk_slash_effect_name),
            (false_knight_scene,false_knight_name),
            (RogueSceneManager.GG_level_scene, RogueSceneManager.GG_level_name),
            (ItemManager.item_scene, ItemManager.item_name),
            (ItemManager.shop_scene,ItemManager.shop_region),
            (ItemManager.shop_scene,ItemManager.shop_menu),
            (ItemManager.shop_scene,RogueSceneManager.shop_counter),
            (RogueSceneManager.gouBro_scene,RogueSceneManager.gouBro),
            (RogueSceneManager.bench_scene,RogueSceneManager.bench),
            (RogueSceneManager.bank_scene,RogueSceneManager.bank),
            (RogueSceneManager.bank_scene,RogueSceneManager.banker),
            (RogueSceneManager.white_fly_palace,RogueSceneManager.white_fly),
            (RogueUIManager.card_scene,RogueUIManager.card_name),
            (Shaman.beam_scene,Shaman.beam_name),
            (Shaman.butterfly_scene,Shaman.butterfly_name),
            (NailMaster.hk_scene,NailMaster.hk_name),
            (BossSceneManager.boss_scene_controler_scene,BossSceneManager.boss_scene_controler_name),
            (BossSceneManager.thk_scene,BossSceneManager.thk_name)
        };
    }

    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        ReservePreloadGO(preloadedObjects);
        BossManager.Init(ReservedGO);
        Uradiance.Init(ReservedGO);
        BattleChampions.Init(ReservedGO);
    }
    internal static GameObject getGO(string scene, string name)
    {
        return ReservedGO.ContainsKey(scene) && ReservedGO[scene].ContainsKey(name) ? ReservedGO[scene][name] : null;
    }
    internal static void ReservePreloadGO(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        var preloadNames = Rogue.Instance.GetPreloadNames();

        foreach (var (scene, name) in preloadNames)
        {
            Log(scene + " " + name);
            if (!ReservedGO.ContainsKey(scene))
            {
                ReservedGO.Add(scene, new Dictionary<string, GameObject>());
            }
            if (!ReservedGO[scene].ContainsKey(name))
            {
                var go = GameObject.Instantiate(preloadedObjects[scene][name]);
                if (go != null)
                {
                    ReservedGO[scene].Add(name, go);
                }
                go.SetActive(false);
                preloadedObjects[scene][name].SetActive(false);
                go.transform.SetParent(Rogue.Instance.rogue_go.transform, false);
            }
        }
    }
    static void Log(object msg)
    {
        Rogue.Instance.Log(msg);
    }
}