using rogue;
using UnityEngine.PlayerLoop;

internal class BossSceneManager
{

    internal const string boss_scene_controler_scene = "GG_Vengefly_V";
    internal const string boss_scene_controler_name = "Boss Scene Controller";
    internal static GameObject scene_manager;
    internal const string thk_scene = "Room_Final_Boss_Core";
    internal const string thk_name = "Boss Control";
    internal static GameObject thk_bc;
    internal class ArenaInfo
    {
        internal float up;
        internal float down;
        internal float left;
        internal float right;
        internal ArenaInfo(float left, float right, float up, float down)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }
    }
    internal static List<string> no_roof_boss_scene = new()
    {
    "GG_Vengefly_V",
    "GG_Gruz_Mother_V",
    "GG_Ghost_Gorb_V",
    "GG_Ghost_Xero_V",
    "GG_Soul_Master",
    "GG_Oblobbles",
    "GG_Ghost_Marmu_V"




    };
    internal static Dictionary<string, ArenaInfo> arena_info = new Dictionary<string, ArenaInfo>()
    {
        { "GG_Vengefly_V",      new(39.86f,54.97f,23f,13.40812f) },
        { "GG_Gruz_Mother_V",   new(86.26f,102.73f,24.73f,15.40812f)},
        {"GG_False_Knight",     new(11.19f,45.7f,37.64f,27.40812f)},
        {"GG_Mega_Moss_Charger",new(28.3f,69.73f,18.52f,7.408125f)},
        {"GG_Hornet_1",         new(15.27f,37.73f,36.19f,28.40812f)},
        {"GG_Ghost_Gorb_V",     new(45.24f,65.87f,45.8f,33.39785f)},
        {"GG_Dung_Defender",     new(60.27f,91.73f,18.31f,7.408124f)},
        {"GG_Mage_Knight_V",     new(35.01f,58.93f,17.67f,5.408125f)},
        {"GG_Brooding_Mawlek_V", new(51.24f,71.71f,14f,4.408125f)},
        {"GG_Nailmasters",       new(30.01f,62.93f,17f,5.408125f)},
        {"GG_Ghost_Xero_V",      new(89.66f,115.13f,22.4f,11.34652f)},
        {"GG_Crystal_Guardian",  new(15.27f,43.73f,23.5f,11.40812f)},
        {"GG_Soul_Master",        new(5.27f,36.73f,42.28f,29.40812f)},
        {"GG_Oblobbles",          new(85.43f,119.73f,19.19f,6.408125f)},
        {"GG_Mantis_Lords_V",      new(17.24f,42.73f,19.57f,7.408124f)},
        {"GG_Ghost_Marmu_V",    new(58.43f,79.67f,20.24f,10.40812f)},
        {"GG_Flukemarm",        new(8.13f,30.93f,23.57784f,5.408124f)},
        {"GG_Broken_Vessel",     new(15.19f,37.9f,37.93f,28.4f)},
        {"GG_Ghost_Galien",      new(34.27f,70.73f,23.8f,14.40812f)},
        {"GG_Painter",           new(32.01f,60.6f,14.55f,5.408125f)},
        {"GG_Hive_Knight",      new(57.27f,80.72f,39.8f,27.40812f)},
        {"GG_Ghost_Hu",         new(32.29f,65.4f,15.7f,3.408125f)},
        {"GG_Collector_V",      new(39.47f,69.58f,105f,95.40811f)},
        {"GG_God_Tamer",        new(85.6f,119.66f,16f,6.43f)},
        {"GG_Grimm",            new(69.54f,102.73f,19f,6.408124f)},
        {"GG_Watcher_Knights",  new(24.6f,62.73f,82f,70.4f)},
        {"GG_Uumuu_V",      new(35.27f,70.73f,134f,106.4f)},
        {"GG_Nosk_Hornet",     new(35.3f,59.76f,23f,13.40812f)},
        {"GG_Sly",      new(35.35f,60.6f,17.7f,5.408125f)},
        {"GG_Hornet_2", new(15.6f,37.67f,39f,28.4f)},
        {"GG_Crystal_Guardian_2",new(19.44f,40.56f,23.7f,11.40812f)},
        {"GG_Lost_Kin",         new(15.6f,37.42f,40f,28.4f)},
        {"GG_Ghost_No_Eyes_V",  new(41.05f,59.99f,28f,7.3f)},
        {"GG_Traitor_Lord",     new(20.39f,59.73f,40f,29.4f)},
        {"GG_White_Defender",   new(60.6f,91.56f,19.6f,7.4f)},
        {"GG_Soul_Tyrant",      new(3.38f,36.6f,42f,29.4f)},
        {"GG_Ghost_Markoth_V",  new(16.6f,34.6f,24.8f,8.4f)},
        {"GG_Grey_Prince_Zote",new(6.6f,46.4f,18.9f,6.4f)},
        {"GG_Failed_Champion",  new(42.4f,76.57f,37.6f,27.4f)},
        {"GG_Grimm_Nightmare",  new(63.3f,108.79f,18.1f,6.4f)},
        {"GG_Hollow_Knight",new(29.33f,61.67f,18.9f,6.4f)}
    };

    internal static void Init()
    {
        scene_manager = GameObject.Instantiate(PreloadManager.getGO(boss_scene_controler_scene, boss_scene_controler_name));
        scene_manager.SetActive(false);
        GameObject.DontDestroyOnLoad(scene_manager);

        thk_bc = GameObject.Instantiate(PreloadManager.getGO(thk_scene, thk_name));
        thk_bc.SetActive(false);
        GameObject.DontDestroyOnLoad(thk_bc);
    }

    internal static void MoreBoss(BossSequence bossSequence)
    {
        var oriscenes = ReflectionHelper.GetField<BossSequence, BossScene[]>(bossSequence, "bossScenes");
        var scenes = oriscenes.ToList();
        static void AddBossScene(string scene_name, int index, List<BossScene> scenes)
        {
            BossScene bossScene = ScriptableObject.CreateInstance<BossScene>();
            bossScene.sceneName = scene_name;//"Room_Final_Boss_Core";
            bossScene.isHidden = false;
            bossScene.requireUnlock = false;
            scenes.Insert(index, bossScene);
        }
        // AddBossScene("GG_Collector_V", 0, scenes);
        // AddBossScene("GG_Hive_Knight", 1, scenes);
        // AddBossScene("GG_Ghost_Hu", 1, scenes);
        // AddBossScene("GG_Grimm", 1, scenes);
        // AddBossScene("GG_Ghost_Gorb_V", 1, scenes);

        ReflectionHelper.SetField<BossSequence, BossScene[]>(bossSequence, "bossScenes", scenes.ToArray());

    }



    // void Init()
    // {
    //     BossSequenceController
    //     BossScene bossScene = ScriptableObject.CreateInstance<BossScene>();
    //     bossScene.sceneName = name;
    //     bossScene.isHidden = false;
    //     bossScene.requireUnlock = false;
    //     bossScenes.Add(bossScene);
    //     Modding.Logger.Log(name);
    //     bossScene = null;
    // }
}