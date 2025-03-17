using rogue;
using UnityEngine.PlayerLoop;

internal class BossSceneManager
{
    internal static GameObject scene_manager;
    internal static GameObject thk_bc;

    internal static void Init(GameObject manager)
    {
        scene_manager = GameObject.Instantiate(manager);
        scene_manager.SetActive(false);
        GameObject.DontDestroyOnLoad(scene_manager);
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