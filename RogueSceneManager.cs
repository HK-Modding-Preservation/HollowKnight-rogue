namespace rogue;

using System;
using rogue.NPCs;
internal static class RogueSceneManager
{
    internal const string shop_counter = "_Scenery/Shop Counter";
    internal const string gouBro_scene = "Mines_18_boss";
    internal const string gouBro = "Mega Zombie Beam Miner (1)";
    static tk2dSpriteAnimation gou_animation = null;
    static GameObject gouBro_go = null;
    internal const string bench_scene = "Mines_18";
    internal const string bench = "_Props/RestBench";
    static GameObject bench_go = null;
    internal const string bank_scene = "Fungus3_35";
    internal const string bank = "Bank Stand";
    static tk2dSpriteAnimation bank_animation = null;
    internal const string banker = "Banker";
    static tk2dSpriteAnimation banker_animation = null;
    internal const string white_fly_palace = "White_Palace_01";

    internal const string white_fly = "White Palace Fly";

    static GameObject fly_go = null;
    static GameObject bank_go;
    static GameObject banker_go;

    internal const string GG_level_scene = "GG_Atrium";

    internal const string GG_level_name = "gg_roof_door_pieces/GG_door_caps/gg_roof_lever";
    internal static void Init()
    {
        gouBro_go = PreloadManager.getGO(gouBro_scene, gouBro);
        gou_animation = UnityEngine.Object.Instantiate(gouBro_go.GetComponent<tk2dSpriteAnimator>().Library);
        GameObject.DontDestroyOnLoad(gou_animation);

        bank_animation = UnityEngine.Object.Instantiate(PreloadManager.getGO(bank_scene, bank).GetComponent<tk2dSpriteAnimator>().Library);
        GameObject.DontDestroyOnLoad(bank_animation);

        banker_animation = UnityEngine.Object.Instantiate(PreloadManager.getGO(bank_scene, banker).GetComponent<tk2dSpriteAnimator>().Library);
        GameObject.DontDestroyOnLoad(banker_animation);

        fly_go = null;
        bench_go = null;

    }
    static void AdjustBenchGO(GameObject bench_go)
    {
        bench_go.LocateMyFSM("Bench Control").FsmVariables.FindFsmVector3("Adjust Vector").Value = new Vector3(-0.5f, 0.1f, 0);
        bench_go.transform.position = new Vector3(128.8f, 61.7f, 0.1f);
        bench_go.name = "RestBench (1)";
    }
    internal static void GameLoadInit()
    {
        if (fly_go != null)
        {
            GameObject.Destroy(fly_go);
            fly_go = null;
        }

        fly_go = GameObject.Instantiate(PreloadManager.getGO(white_fly_palace, white_fly));
        fly_go.SetActive(false);
        GameObject.DontDestroyOnLoad(fly_go);
        if (bench_go != null)
        {
            GameObject.Destroy(bench_go);
            bench_go = null;
        }
        bench_go = GameObject.Instantiate(PreloadManager.getGO(bench_scene, bench));
        AdjustBenchGO(bench_go);
        bench_go.SetActive(false);
        GameObject.DontDestroyOnLoad(bench_go);
        HeroController.instance.Respawn();
    }
    internal static void PrePareShopGO(GameObject shop_go)
    {
        gouBro_go = new("gou_Bro");
        gouBro_go.transform.SetParent(shop_go.transform, false);
        gouBro_go.transform.localPosition = new Vector3(-0.1f, 0.3f, -0.009f);
        gouBro_go.AddComponent<tk2dSprite>();
        gouBro_go.AddComponent<tk2dSpriteAnimator>().Library = gou_animation;

        bank_go = new("bank");
        bank_go.transform.SetParent(shop_go.transform, false);
        bank_go.transform.localPosition = new Vector3(0.1f, 1.2f, 0);
        bank_go.AddComponent<tk2dSprite>();
        bank_go.AddComponent<tk2dSpriteAnimator>().Library = bank_animation;

        banker_go = new("banker");
        banker_go.transform.SetParent(shop_go.transform, false);
        banker_go.transform.localPosition = new Vector3(0, 0, 0);
        banker_go.AddComponent<tk2dSprite>();
        banker_go.AddComponent<tk2dSpriteAnimator>().Library = banker_animation;


    }

    static IEnumerator DelayShowDreamConvo(float delay, string msg)
    {
        yield return new WaitForSeconds(delay);
        Rogue.Instance.ShowDreamConvo(msg);
    }
    internal static void AddRoof(string scene_name)
    {
        void AddRoofGO(Vector3 pos, float left, float right)
        {
            GameObject roof = new("roof");
            roof.transform.position = pos;
            var col = roof.AddComponent<BoxCollider2D>();
            col.size = new Vector2(right - left + 4, 1f);
            col.offset = new Vector2((left + right) / 2, 0.5f);
            roof.layer = LayerMask.NameToLayer("Terrain");
        }
        if (BossSceneManager.arena_info.ContainsKey(scene_name) && !BossSceneManager.no_roof_boss_scene.Contains(scene_name))
        {
            var info = BossSceneManager.arena_info[scene_name];
            AddRoofGO(new Vector3(0, info.up + 1), info.left, info.right);
        }
    }
    internal static void ModifyScene(string scenename)
    {

        switch (scenename)
        {
            case "GG_Blue_Room":
                if (FlowerManager.open)
                {
                    NPCManager.npcs[typeof(Emilitia).Name].SetPosition(Emilitia.blue_house_pos);
                }
                break;
            case "GG_Engine":
                ItemManager.Instance.SetNoShop();
                if (GameInfo.in_rogue)
                {
                    ProcessManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "godseeker".Localize()));
                }
                fly_go.SetActive(false);
                break;
            case "GG_Workshop":
                if (FlowerManager.open)
                {
                    GameObject check = new("check");
                    check.AddComponent<Check>();
                }
                break;
            case "GG_Atrium_Roof":
                GameObject stair = GameObject.Instantiate(GameObject.Find("gg_plat_float_small"));
                stair.transform.SetPosition2D(new Vector2(118, 64));
                stair.SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    GameObject plat = GameObject.Instantiate(GameObject.Find("gg_plat_float_small"));
                    plat.transform.SetPosition2D(new Vector2(142.2207f, 43.5f + i * 5));
                    plat.SetActive(true);
                }
                SetLeveler();
                SetBench();
                // if (GameInfo.in_rogue)
                // {
                //     ItemManager.Instance.StartShopItem();
                //     ItemManager.Instance.SetShop(false);
                //     ResetAll();
                //     SetBench();
                //     SetGou();
                // }
                break;
            case "GG_Spa":
                if (GameInfo.in_rogue)
                {
                    GameInfo.spa_count++;
                    GiftFactory.UpdateWeight();
                    ItemManager.Instance.DestroyAllItems();
                    ItemManager.Instance.GiveReward(GameInfo.spa_count);
                    ItemManager.Instance.SetNoShop();
                    ResetAll();
                    switch (GameInfo.spa_count)
                    {

                        case 5:
                            NPCManager.npcs[typeof(CharmSlug).Name].SetPosition(new Vector3(78, 16, 0.1f));
                            ItemManager.Instance.CharmShopItem();
                            ItemManager.Instance.SetShop(true);
                            if (GameInfo.gameMode == GameInfo.GameMode.MODE1)
                                NPCManager.npcs[typeof(Nailsmith).Name].SetPosition(Nailsmith.spa_pos);
                            break;
                        case 1:
                            ItemManager.Instance.NormalShopItem();
                            ItemManager.Instance.SetShop(true);
                            SetBanker();
                            break;
                        case 3:
                            ItemManager.Instance.NormalShopItem();
                            ItemManager.Instance.SetShop(true);
                            SetBanker();
                            if (GameInfo.data.num_injured <= 10)
                            {
                                NPCManager.npcs[typeof(QuirrelNail).Name].SetPosition(QuirrelNail.spa_pos);
                            }
                            break;
                        case 2:
                            NPCManager.npcs[typeof(Xun).Name].SetPosition(Xun.spa_pos);
                            break;
                        case 4:
                            int t = UnityEngine.Random.Range(0, 3);
                            if (t == 0)
                            {
                                NPCManager.npcs[typeof(ElderBug).Name].SetPosition(ElderBug.spa_pos);
                            }
                            else if (t == 1)
                            {
                                NPCManager.npcs[typeof(Jiji).Name].SetPosition(Jiji.spa_pos);
                            }
                            else
                            {
                                NPCManager.npcs[typeof(WishPool).Name].SetPosition(WishPool.spa_pos);
                            }
                            break;
                        case 6:
                            ItemManager.Instance.NormalShopItem();
                            ItemManager.Instance.SetShop(true);
                            SetBanker();
                            NPCManager.npcs[typeof(Jinn).Name].SetPosition(Jinn.spa_pos);
                            break;
                        case 7:
                            ItemManager.Instance.NormalShopItem();
                            ItemManager.Instance.SetShop(true);
                            SetBanker();
                            break;
                        default:
                            break;
                    }

                }
                break;
            default:
                ResetAll();
                ItemManager.Instance.SetNoShop();
                break;
        }

    }

    internal static void SetGou()
    {
        gouBro_go.SetActive(true);
        gouBro_go.GetComponent<tk2dSpriteAnimator>().Play("Sleep");
        gouBro_go.GetComponent<tk2dSprite>().FlipX = true;
        gouBro_go.SetActive(true);
    }
    internal static void SetBanker()
    {
        bank_go.SetActive(true);
        banker_go.SetActive(true);
        bank_go.GetComponent<tk2dSpriteAnimator>().Play("Stand Idle");
        banker_go.GetComponent<tk2dSpriteAnimator>().Play("Idle");

    }
    internal static void SetBench()
    {
        bench_go.transform.SetPosition3D(128.8f, 61.7f, 0.1f);
        bench_go.SetActive(true);
    }
    internal static void SetLeveler()
    {
        NPCManager.npcs[typeof(Leveler).Name].SetPosition(Leveler.pos);
        GameObject go = NPCManager.npcs[typeof(Leveler).Name].go;
        if (!GameInfo.in_rogue)
        {
            go.GetComponent<tk2dSprite>().color = Color.white;
            go.GetComponent<tk2dSpriteAnimator>().Play("Lever Idle");
        }
        else
        {
            go.GetComponent<tk2dSpriteAnimator>().Play("Lever Activated");
            if (GameInfo.gameMode == GameInfo.GameMode.MODE0)
            {
                go.GetComponent<tk2dSprite>().color = Color.white;
            }
            else if (GameInfo.gameMode == GameInfo.GameMode.MODE1)
            {
                go.GetComponent<tk2dSprite>().color = Color.red;
            }
        }

    }
    internal static void ResetAll()
    {
        bench_go.SetActive(false);
        gouBro_go.SetActive(false);
        banker_go.SetActive(false);
        bank_go.SetActive(false);
        fly_go.SetActive(false);


    }
}