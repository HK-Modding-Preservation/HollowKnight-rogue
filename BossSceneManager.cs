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
        AddBossScene("GG_Hive_Knight", 1, scenes);
        AddBossScene("GG_Ghost_Hu", 1, scenes);
        AddBossScene("GG_Grimm", 1, scenes);

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