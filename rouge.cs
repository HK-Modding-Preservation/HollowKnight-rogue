using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Policy;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using IL.InControl.UnityDeviceProfiles;
using IL.TMPro;
using InControl;
using JetBrains.Annotations;
using Modding;
using Modding.Converters;
using Mono.Cecil;
using Mono.Math.Prime.Generator;
using Newtonsoft.Json;
using Satchel;
using Satchel.BetterMenus;
using Satchel.Futils;
using Satchel.Futils.Serialiser;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngineInternal;
using XInputDotNetPure;

namespace rogue;


public class Rogue : Mod, ICustomMenuMod, IGlobalSettings<setting>
{
    internal static Rogue Instance;

    public Rogue() : base("Rogue")
    {
        Instance = this;
    }

    public override string GetVersion()
    {
        return "1.0.0.2";
    }
    public class actions : PlayerActionSet
    {
        public PlayerAction refresh;
        public PlayerAction start;
        public PlayerAction reset;

        public PlayerAction over;
        public actions()
        {
            refresh = new PlayerAction("rogue_refresh", this);
            start = new PlayerAction("rogue_start", this);
            reset = new PlayerAction("rogue_reset", this);
            over = new PlayerAction("rogue_over", this);
        }

    }
    public enum giftname
    {
        test,
        charm_fengqun,
        charm_compass,
        charm_chongge,
        charm_yingke,
        charm_baldur,
        charm_wangnu,
        charm_quick_focus,
        charm_blue_heart,
        charm_blue_hexin,
        charm_defender,
        charm_xichong,
        charm_jingji,
        charm_jiaoao,
        charm_wenti,
        charm_chenzhong,
        charm_fengli,
        charm_mogu,
        charm_xiuchang,
        charm_saman,
        charm_bushou,
        charm_shihun,
        charm_zigong,
        charm_xinzang,
        charm_tanlan,
        charm_power,
        charm_dingyao,
        charm_jiaoni,
        charm_wuen,
        charm_fengchao,
        charm_wumeng,
        charm_dash_master,
        charm_kuaipi,
        charm_faniu,
        charm_deep_facus,
        charm_tuibian,
        charm_void_heart,

        charm_feimaotui,

        charm_meng_zhi_dun,
        charm_bian_zhi_zhe,
        charm_wuyou,
        add_2_mask,
        add_1_vessel,
        add_3_notch,
        nail_upgrade,
        get_fireball,
        get_scream,
        get_quake,
        get_dash,
        get_double_jump,
        get_zhua_lei_ding,
        get_one_skill_up,
        get_dash_slash,
        get_upward_slash,
        get_cyclone,
        get_any_spell,
        get_any_nail_art,
        get_any_shift,
        get_a_big_gift,
        get_500_geo,
        get_power,
        get_saman,
        get_quick_and_wenti,
        get_jiaoao,
        get_faniu_and_bushou,
        get_shihun,
        get_kuaiju_shenju_wuen,
        get_badeer_yingke_wuyou,
        get_zigong_bianzhizhe_chongge,
        get_any_2_charms,
        get_one_egg,
        get_xichong,
        get_birthright,
        warrior,
        mage,
        ranger,
        nail_master,
        shaman,
        hunter,
        uunn,
        joni,
        guarder,
        moth,
        grey_prince,
        grimm,
        Tuk,
        defender,
        mantis,
        collector,
        shop_keeper_key,
        shop_add_1_notch_1,
        shop_add_1_notch_2,
        shop_add_1_notch_3,
        shop_add_1_notch_4,
        shop_nail_upgrade,
        shop_random_gift,
        shop_any_charm_1,
        shop_any_charm_2,
        shop_any_charm_3,
        shop_any_charm_4,
        shop_egg,
        shop_super_dash,
        shop_wall_jump,
        shop_dream_nail,
        shop_add_1_mask,
        shop_add_1_vessel,
        shop_prettey_key,
        shop_refresh,
        shop_acid_swim,
        shop_double_jump,
        shop_dash,

    }

    public Dictionary<string, string> name_and_desc = new Dictionary<string, string>
    {
    };

    Dictionary<giftname, (float, int)> charm_weight_and_price = new Dictionary<giftname, (float, int)>
        {
            {giftname.charm_compass,(1,50)},
            {giftname.charm_fengqun,(1,50)},
            {giftname.charm_yingke,(1,200)},
            {giftname.charm_bushou,(0.8f,350)},
            {giftname.charm_saman,(0.4f,800)},
            {giftname.charm_shihun,(0.8f,450)},
            {giftname.charm_dash_master,(1f,150)},
            {giftname.charm_feimaotui,(1,150)},
            {giftname.charm_chongge,(8,300)},
            {giftname.charm_tuibian,(1,200)},
            {giftname.charm_xinzang,(1,300)},
            {giftname.charm_tanlan,(5,200)},
            {giftname.charm_power,(0.4f,800)},
            {giftname.charm_faniu,(0.6f,500)},
            {giftname.charm_wenti,(1,200)},
            {giftname.charm_chenzhong,(1,200)},
            {giftname.charm_kuaipi,(0.4f,600)},
            {giftname.charm_xiuchang,(1,200)},
            {giftname.charm_jiaoao,(0.8f,350)},
            {giftname.charm_wangnu,(1,50)},
            {giftname.charm_jingji,(1,100)},
            {giftname.charm_baldur,(1f,150)},
            {giftname.charm_xichong,(0.4f,600)},
            {giftname.charm_defender,(1,100)},
            {giftname.charm_zigong,(0.8f,200)},
            {giftname.charm_quick_focus,(0.8f,300)},
            {giftname.charm_deep_facus,(0.8f,300)},
            {giftname.charm_blue_heart,(1,150)},
            {giftname.charm_blue_hexin,(1,250)},
            {giftname.charm_jiaoni,(0.8f,50)},
            {giftname.charm_fengchao,(1,250)},
            {giftname.charm_mogu,(1,150)},
            {giftname.charm_fengli,(1,150)},
            {giftname.charm_wuen,(0.8f,300)},
            {giftname.charm_dingyao,(5,300)},
            {giftname.charm_bian_zhi_zhe,(1,250)},
            {giftname.charm_wumeng,(5,250)},
            {giftname.charm_meng_zhi_dun,(1,200)},
            {giftname.charm_wuyou,(0.8f,300)}
        };
    public bool inRogue = false;
    public setting _set = new();
    public static actions self_actions = new();
    private string item_scene = "Tutorial_01";
    private string item = "_Props/Chest/Item/Shiny Item (1)";

    private string shop_scene = "Room_shop";

    private string shop_region = "Basement Closed/Shop Region";

    private string shop_menu = "Shop Menu";
    private string shop_counter = "_Scenery/Shop Counter";

    private string gouBro_scene = "Mines_18_boss";

    private string gouBro = "Mega Zombie Beam Miner (1)";

    private string bench_scene = "Mines_18";

    private string bench = "_Props/RestBench";

    private string bank_scene = "Fungus3_35";
    private string bank = "Bank Stand";
    private string banker = "Banker";

    private string white_palace = "White_Palace_01";

    private string white_fly = "White Palace Fly";
    public GameObject charms;

    public GameObject shiny_item = null;

    public GameObject menu_go = null;

    public GameObject shop_go = null;

    public GameObject fly_go = null;
    public int getAnyCharm = 0;

    public int refresh_num = 3;

    public int relive_num = 0;
    public string charmString;

    // public List<gift> smallRewards = new();
    public Dictionary<giftname, gift> smallRewards = new();
    public List<gift> actSmallRewards = new();
    public Dictionary<giftname, gift> bigRewards = new();

    public List<gift> actBigRewards = new();

    public Dictionary<giftname, gift> shopRewards = new();
    public List<gift> actShopRewards = new();

    public Dictionary<giftname, gift> charmRewards = new();
    public List<gift> actCharmRewards = new();

    public bool ToggleButtonInsideMenu => true;

    public ItemManager itemManager;

    public Dictionary<GameObject, GameObject> shop_items = new();

    public int spa_count = 0;

    public SpriteAtlas all_sprites;

    public static giftname? role = null;

    public static bool getBirthright = false;


    public tk2dSpriteAnimation gou_animation = null;

    public tk2dSpriteAnimation bank_animation = null;
    public tk2dSpriteAnimation banker_animation = null;

    public GameObject bench_go = null;

    public static int pretty_key_num = 0;



    public class gift
    {
        public bool active = true;
        [NonSerialized]
        public int id;
        [NonSerialized]
        public int level;
        [NonSerialized]
        public int price;

        [NonSerialized]
        public Action<giftname> reward;


        [NonSerialized]
        public giftname giftname;

        [NonSerialized]
        public string name = "";

        [NonSerialized]
        public string desc = "";


        [NonSerialized]
        public float weight = 0;

        [NonSerialized]
        public bool showConvo = true;

        [NonSerialized]
        public Sprite sprite = null;

        [NonSerialized]
        public Func<giftname, Sprite> getSprite = null;
        [NonSerialized]
        public bool force_active = false;
        [NonSerialized]
        public Vector2 scale = Vector2.zero;
    }

    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)>
        {
            ("Tutorial_01","_Props/Chest/Item/Shiny Item (1)"),
            (shop_scene,shop_region),
            (shop_scene,shop_menu),
            (shop_scene,shop_counter),
            (gouBro_scene,gouBro),
            (bench_scene,bench),
            (bank_scene,bank),
            (bank_scene,banker),
            (white_palace,white_fly)
        };
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        base.Initialize(preloadedObjects);
        Lang.init();
        shiny_item = UnityEngine.Object.Instantiate(preloadedObjects[item_scene][item]);
        shiny_item.LocateMyFSM("Shiny Control").ChangeTransition("PD Bool?", "COLLECTED", "Fling?");
        shiny_item.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(shiny_item);

        var gou_go = preloadedObjects[gouBro_scene][gouBro];
        gou_animation = UnityEngine.Object.Instantiate(gou_go.GetComponent<tk2dSpriteAnimator>().Library);
        UnityEngine.Object.DontDestroyOnLoad(gou_animation);
        gou_go.SetActive(false);

        var bank_go = preloadedObjects[bank_scene][bank];
        bank_animation = UnityEngine.Object.Instantiate(bank_go.GetComponent<tk2dSpriteAnimator>().Library);
        UnityEngine.Object.DontDestroyOnLoad(bank_animation);
        bank_go.SetActive(false);

        var banker_go = preloadedObjects[bank_scene][banker];
        banker_animation = UnityEngine.Object.Instantiate(banker_go.GetComponent<tk2dSpriteAnimator>().Library);
        UnityEngine.Object.DontDestroyOnLoad(banker_animation);
        banker_go.SetActive(false);

        fly_go = UnityEngine.Object.Instantiate(preloadedObjects[white_palace][white_fly]);
        UnityEngine.Object.DontDestroyOnLoad(fly_go);
        fly_go.SetActive(false);

        bench_go = UnityEngine.Object.Instantiate(preloadedObjects[bench_scene][bench]);
        UnityEngine.Object.DontDestroyOnLoad(bench_go);
        bench_go.SetActive(false);


        menu_go = UnityEngine.Object.Instantiate(preloadedObjects[shop_scene][shop_menu]);

        menu_go.LocateMyFSM("shop_control").ChangeTransition("Stock?", "FINISHED", "Open Window");
        // menu_go.LocateMyFSM("shop_control").ChangeTransition("Stock?", "NO STOCK", "Open Window");
        var fsm = menu_go.FindGameObjectInChildren("Item List").LocateMyFSM("Item List Control");
        fsm.GetAction<SetTextMeshProText>("Get Details Init", 2).textString = fsm.FsmVariables.GetFsmString("Item Name Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details Init", 6).textString = fsm.FsmVariables.GetFsmString("Item Desc Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details", 3).textString = fsm.FsmVariables.GetFsmString("Item Name Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details", 6).textString = fsm.FsmVariables.GetFsmString("Item Desc Convo");
        menu_go.FindGameObjectInChildren("Confirm").FindGameObjectInChildren("UI List").LocateMyFSM("Confirm Control").InsertCustomAction("Special Type?", (fsm) =>
        {
            giftname type = (giftname)(fsm.FsmVariables.GetFsmInt("Special Type").Value - 18);
            Log(type);
            if (smallRewards.ContainsKey(type))
            {
                smallRewards[type].reward(type);
                itemManager.DisplayStates();
                if (smallRewards[type].showConvo) ShowConvo(smallRewards[type].desc);
            }
            else if (bigRewards.ContainsKey(type))
            {
                bigRewards[type].reward(type);
                itemManager.DisplayStates();
                if (bigRewards[type].showConvo) ShowConvo(bigRewards[type].desc);
            }
            else if (shopRewards.ContainsKey(type))
            {
                shopRewards[type].reward(type);
                itemManager.DisplayStates();
                if (shopRewards[type].showConvo) ShowConvo(shopRewards[type].name);
            }
            else if (charmRewards.ContainsKey(type))
            {
                charmRewards[type].reward(type);
                itemManager.DisplayStates();
                if (charmRewards[type].showConvo) ShowConvo(charmRewards[type].name);

            }
            UpdateWeight();
        }, 1);
        menu_go.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(menu_go);

        shop_go = UnityEngine.Object.Instantiate(preloadedObjects[shop_scene][shop_region]);
        shop_go.AddComponent<SpriteRenderer>().sprite = preloadedObjects[shop_scene][shop_counter].GetComponent<SpriteRenderer>().sprite;
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Intro Convo?", "YES", "Shop Up");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Intro Convo?", "NO", "Shop Up");

        shop_go.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(shop_go);


        GameObject rogue_go = new GameObject("rogue_go");
        UnityEngine.Object.DontDestroyOnLoad(rogue_go);
        itemManager = rogue_go.AddComponent<ItemManager>();
        On.PlayMakerFSM.OnEnable += CharmsInit;
        RewardInit();
        On.BossSequenceController.FinishLastBossScene += EndScene;
        On.BossSequenceController.SetupNewSequence += BeginScene;
        ModHooks.TakeHealthHook += new_relive;
        On.HeroController.Awake += OnSavegameLoad;

    }

    public void adjust_menu_go(GameObject menu_go)
    {
        menu_go.LocateMyFSM("shop_control").ChangeTransition("Stock?", "FINISHED", "Open Window");
        // menu_go.LocateMyFSM("shop_control").ChangeTransition("Stock?", "NO STOCK", "Open Window");
        menu_go.LocateMyFSM("shop_control").FsmVariables.FindFsmString("No Stock Event").Value = "ISELDA";
        menu_go.LocateMyFSM("shop_control").GetAction<SetFsmString>("Iselda", 4).setValue = "rouge_introduction";
        menu_go.LocateMyFSM("shop_control").GetAction<CallMethodProper>("Iselda", 5).parameters[0].stringValue = "rouge_introduction";
        menu_go.LocateMyFSM("shop_control").GetAction<CallMethodProper>("Iselda", 5).parameters[1].stringValue = "rouge";
        var fsm = menu_go.FindGameObjectInChildren("Item List").LocateMyFSM("Item List Control");
        fsm.GetAction<SetTextMeshProText>("Get Details Init", 2).textString = fsm.FsmVariables.GetFsmString("Item Name Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details Init", 6).textString = fsm.FsmVariables.GetFsmString("Item Desc Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details", 3).textString = fsm.FsmVariables.GetFsmString("Item Name Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details", 6).textString = fsm.FsmVariables.GetFsmString("Item Desc Convo");
        if (menu_go.FindGameObjectInChildren("Confirm").FindGameObjectInChildren("UI List").LocateMyFSM("Confirm Control").GetState("Special Type?").Actions.Length < 3)
        {
            menu_go.FindGameObjectInChildren("Confirm").FindGameObjectInChildren("UI List").LocateMyFSM("Confirm Control").InsertCustomAction("Special Type?", (fsm) =>
            {
                giftname type = (giftname)(fsm.FsmVariables.GetFsmInt("Special Type").Value - 18);
                Log(type);
                if (smallRewards.ContainsKey(type))
                {
                    smallRewards[type].reward(type);
                    itemManager.DisplayStates();
                    if (smallRewards[type].showConvo) ShowConvo(smallRewards[type].desc);
                }
                else if (bigRewards.ContainsKey(type))
                {
                    bigRewards[type].reward(type);
                    itemManager.DisplayStates();
                    if (bigRewards[type].showConvo) ShowConvo(bigRewards[type].desc);
                }
                else if (shopRewards.ContainsKey(type))
                {
                    shopRewards[type].reward(type);
                    itemManager.DisplayStates();
                    if (shopRewards[type].showConvo) ShowConvo(shopRewards[type].name);
                }
                else if (charmRewards.ContainsKey(type))
                {
                    charmRewards[type].reward(type);
                    itemManager.DisplayStates();
                    if (charmRewards[type].showConvo) ShowConvo(charmRewards[type].name);

                }
                UpdateWeight();
            }, 1);
        }
    }
    public void adjust_shop_go(GameObject shop_go)
    {
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Intro Convo?", "YES", "Shop Up");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Intro Convo?", "NO", "Shop Up");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Check Facing", "TURN RIGHT", "Turn Hero Left");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Check Facing", "FINISHED", "Turn Hero Left");
        if (shop_go.LocateMyFSM("Shop Region").GetState("Out Of Range").Actions.Length == 2)
        {
            shop_go.LocateMyFSM("Shop Region").InsertCustomAction("Out Of Range", (fsm) =>
            {
                fsm.gameObject.FindGameObjectInChildren("gou_Bro").GetComponent<tk2dSpriteAnimator>().Play("Sleep");
            }, 2);
        }
        if (shop_go.LocateMyFSM("Shop Region").GetState("Shop Up").Actions.Length == 2)
        {
            shop_go.LocateMyFSM("Shop Region").InsertCustomAction("Shop Up", (fsm) =>
            {
                fsm.gameObject.FindGameObjectInChildren("gou_Bro").GetComponent<tk2dSpriteAnimator>().Play("Wake");
            }, 2);
        }
        GameObject gou = new("gou_Bro");
        gou.transform.SetParent(shop_go.transform);
        gou.transform.localPosition = new Vector3(-0.1f, 0.3f, -0.009f);
        gou.AddComponent<tk2dSprite>();
        gou.AddComponent<tk2dSpriteAnimator>().Library = gou_animation;


        GameObject bank = new("bank");
        bank.transform.SetParent(shop_go.transform);
        bank.transform.localPosition = new Vector3(0.1f, 1.2f, 0);
        bank.AddComponent<tk2dSprite>();
        bank.AddComponent<tk2dSpriteAnimator>().Library = bank_animation;

        GameObject banker = new("banker");
        banker.transform.SetParent(shop_go.transform);
        banker.transform.localPosition = new Vector3(0, 0, 0);
        banker.AddComponent<tk2dSprite>();
        banker.AddComponent<tk2dSpriteAnimator>().Library = banker_animation;
    }
    public void adjust_bench_go(GameObject bench_go)
    {
        bench_go.LocateMyFSM("Bench Control").FsmVariables.FindFsmVector3("Adjust Vector").Value = new Vector3(-0.5f, 0.1f, 0);
        bench_go.transform.position = new Vector3(128.8f, 61.7f, 0.1f);
        bench_go.name = "RestBench (1)";
    }

    private int new_relive(int damage)
    {
        if (relive_num > 0 && inRogue && BossSequenceController.IsInSequence)
        {
            if (((PlayerData.instance.health + PlayerData.instance.healthBlue) <= damage) || (PlayerData.instance.equippedCharm_27 && (PlayerData.instance.healthBlue <= damage)))
            {
                relive_num--;
                if (role == giftname.Tuk)
                {
                    int r = UnityEngine.Random.Range(0, 2);
                    if (r == 0)
                    {
                        AddNailDamage();
                        ShowConvo("nail_upgrade_name".Localize());
                    }
                    else
                    {
                        GiveMask();
                        GiveMask();
                        ShowConvo("add_2_mask_name".Localize());
                    }
                }
                if (role == giftname.joni)
                {
                }
                else
                {
                    HeroController.instance.CharmUpdate();
                    PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
                }
                UpdateWeight();
                itemManager.DisplayStates();
                return 0;
            }
            // itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
        }
        return damage;
    }

    private void OnSavegameLoad(On.HeroController.orig_Awake orig, HeroController self)
    {
        orig(self);
        // PlayerData.instance.respawnMarkerName = "RestBench (1)";
        if (itemManager == null)
        {
            Log("ERR   itemmanager is null!");
            return;
        }
        if (itemManager.item_menu_go != null)
        {
            itemManager.item_menu_go.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_menu_go);
        }

        itemManager.item_menu_go = GameObject.Instantiate(menu_go);
        adjust_menu_go(itemManager.item_menu_go);
        GameObject.DontDestroyOnLoad(itemManager.item_menu_go);
        itemManager.item_menu_go.SetActive(false);

        if (itemManager.item_shop_go != null)
        {
            itemManager.item_shop_go.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_shop_go);
        }
        itemManager.item_shop_go = GameObject.Instantiate(shop_go);
        adjust_shop_go(itemManager.item_shop_go);
        GameObject.DontDestroyOnLoad(itemManager.item_shop_go);
        itemManager.item_shop_go.SetActive(false);

        if (itemManager.item_fly_go != null)
        {
            itemManager.item_fly_go.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_fly_go);
        }
        itemManager.item_fly_go = GameObject.Instantiate(fly_go);
        GameObject.DontDestroyOnLoad(itemManager.item_fly_go);
        itemManager.item_fly_go.SetActive(false);

        if (itemManager.item_restbench != null)
        {
            itemManager.item_restbench.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_restbench);
        }
        HeroController.instance.Respawn();
        itemManager.item_restbench = GameObject.Instantiate(bench_go);
        adjust_bench_go(itemManager.item_restbench);
        GameObject.DontDestroyOnLoad(itemManager.item_restbench);
        itemManager.item_shop_go.SetActive(false);
        Log("reload  shop and menu");

    }


    private void BeginScene(On.BossSequenceController.orig_SetupNewSequence orig, BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
    {
        orig(sequence, bindings, playerData);
    }

    private void EndScene(On.BossSequenceController.orig_FinishLastBossScene orig, BossSceneController self)
    {

        if (inRogue) inRogue = false;
        orig(self);

    }

    private void RewardInit()
    {
        Log("RewardInit");

        smallRewards.Clear();
        smallRewards.Add(giftname.add_2_mask, new gift()
        {
            level = 1,
            reward = (giftname) =>
                    {
                        GiveMask();
                        GiveMask();
                    },
            name = "add_2_mask_name".Localize(),
            desc = "add_2_mask_name".Localize(),
            weight = 1f,
        });
        smallRewards.Add(giftname.add_1_vessel, new gift()
        {
            level = 0,
            reward = (giftname) =>
                    {
                        GiveVessel();
                        Log("1 vessel");
                    },
            name = "add_1_vessel_name".Localize(),
            desc = "add_1_vessel_name".Localize(),
            weight = 1.1f,
        });
        smallRewards.Add(giftname.nail_upgrade, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                AddNailDamage();
            },
            name = "nail_upgrade_name".Localize(),
            desc = "+1骨钉",
            weight = 1f,
        });
        smallRewards.Add(giftname.add_3_notch, new gift()
        {
            level = 0,
            reward = (giftname) =>
            {
                PlayerData.instance.charmSlots += 3;
                if (PlayerData.instance.charmSlots > 11)
                    PlayerData.instance.charmSlots = 11;
            },
            name = "add_3_notch_name".Localize(),
            desc = "+3护符槽",
            weight = 1.1f,
        });
        smallRewards.Add(giftname.get_fireball, new gift()
        {
            level = 2,
            reward = (giftname) =>
            {
                PlayerData.instance.hasSpell = true;
                if (PlayerData.instance.fireballLevel < 2)
                    PlayerData.instance.fireballLevel++;
            },
            name = "get_fireball_name".Localize(),
            desc = "获得波法术",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_scream, new gift()
        {
            level = 2,
            reward = (giftname) =>
        {
            PlayerData.instance.hasSpell = true;
            if (PlayerData.instance.screamLevel < 2)
                PlayerData.instance.screamLevel++;
        },
            name = "get_scream_name".Localize(),
            desc = "获得吼法术",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_quake, new gift()
        {
            level = 3,
            reward = (giftname) =>
                    {
                        PlayerData.instance.hasSpell = true;
                        if (PlayerData.instance.quakeLevel < 2)
                            PlayerData.instance.quakeLevel++;
                    },
            name = "get_quake_name".Localize(),
            desc = "获得砸法术",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_dash, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            if (PlayerData.instance.hasDash)
            {
                PlayerData.instance.hasShadowDash = true;
                PlayerData.instance.canShadowDash = true;
            }
            else
            {
                PlayerData.instance.hasDash = true;
                PlayerData.instance.canDash = true;
            }
        },
            name = "get_dash_name".Localize(),
            desc = "获得冲刺",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_double_jump, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            PlayerData.instance.hasDoubleJump = true;
        },
            name = Language.Language.Get("INV_NAME_DOUBLEJUMP", "UI"),
            desc = "获得二段跳",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_zhua_lei_ding, new gift()
        {
            level = 1,
            reward = (giftname) =>
                    {
                        Log("飞舞大礼包");
                        PlayerData.instance.hasSuperDash = true;
                        PlayerData.instance.hasDreamNail = true;
                        PlayerData.instance.hasAcidArmour = true;
                        PlayerData.instance.hasWalljump = true;
                        PlayerData.instance.canWallJump = true;
                        PlayerData.instance.canSuperDash = true;
                    },
            name = Language.Language.Get("INV_NAME_WALLJUMP", "UI") + "&" + Language.Language.Get("INV_NAME_SUPERDASH", "UI") + "&" + Language.Language.Get("INV_NAME_ACIDARMOUR", "UI") + "&" + Language.Language.Get("INV_NAME_DREAMNAIL_A", "UI"),
            desc = "获得螳螂爪/超冲/酸泪/梦钉",
            weight = 1f,
        });
        smallRewards.Add(giftname.get_one_skill_up, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            List<gift> gifts = new();
            if (PlayerData.instance.fireballLevel == 1) gifts.Add(smallRewards[giftname.get_fireball]);
            if (PlayerData.instance.screamLevel == 1) gifts.Add(smallRewards[giftname.get_scream]);
            if (PlayerData.instance.quakeLevel == 1) gifts.Add(smallRewards[giftname.get_quake]);
            if (PlayerData.instance.nailDamage < 21) gifts.Add(smallRewards[giftname.nail_upgrade]);
            if (PlayerData.instance.hasDash && (!PlayerData.instance.hasShadowDash)) gifts.Add(smallRewards[giftname.get_dash]);
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_one_skill_up_name".Localize(),
            desc = "选择一已有法术/冲刺/骨钉升级",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_dash_slash, new gift()
        {
            level = 0,
            reward = (giftname) =>
            {
                PlayerData.instance.hasUpwardSlash = true;
                PlayerData.instance.hasNailArt = true;
            }
            ,
            name = Language.Language.Get("INV_NAME_ART_UPPER", "UI"),
            desc = "获得冲刺劈砍",
            weight = 1.1f,
        });
        smallRewards.Add(giftname.get_upward_slash, new gift()
        {
            level = 0,
            reward = (giftname) =>
                    {
                        PlayerData.instance.hasDashSlash = true;
                        PlayerData.instance.hasNailArt = true;
                    },
            name = Language.Language.Get("INV_NAME_ART_DASH", "UI"),
            desc = "获得强力劈砍",
            weight = 1.1f,
        });
        smallRewards.Add(giftname.get_cyclone, new gift()
        {
            level = 1,
            reward = (giftname) =>
                    {
                        PlayerData.instance.hasCyclone = true;
                        PlayerData.instance.hasNailArt = true;
                    },
            name = Language.Language.Get("INV_NAME_ART_CYCLONE", "UI"),
            desc = "获得旋风劈砍",
            weight = 1f,
        });
        smallRewards.Add(giftname.get_any_spell, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            List<gift> gifts = new() { smallRewards[giftname.get_fireball], smallRewards[giftname.get_scream], smallRewards[giftname.get_quake] };
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_any_spell_name".Localize(),
            desc = "获得任一法术",
            weight = 0f,
            active = false,
        });
        smallRewards.Add(giftname.get_any_shift, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            List<gift> gifts = new() { smallRewards[giftname.get_dash], smallRewards[giftname.get_zhua_lei_ding], smallRewards[giftname.get_double_jump] };
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_any_shift_name".Localize(),
            desc = "获得任一位移",
            weight = 0f,
            active = false,
        });
        smallRewards.Add(giftname.get_any_nail_art, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            List<gift> gifts = new() { smallRewards[giftname.get_dash_slash], smallRewards[giftname.get_upward_slash], smallRewards[giftname.get_cyclone] };
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_any_nail_art_name".Localize(),
            desc = "获得任一剑技",
            weight = 0f,
            active = false
        });
        smallRewards.Add(giftname.get_a_big_gift, new gift()
        {
            level = 4,
            reward = (giftname) =>
            {
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.one_big_gift });
                smallRewards[giftname.get_a_big_gift].weight = 0;
            },
            name = "get_a_big_gift_name".Localize(),
            desc = "随机一个大天赋",
            weight = 0.7f,
        });
        smallRewards.Add(giftname.get_500_geo, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                HeroController.instance.AddGeo(500);
                smallRewards[giftname.get_500_geo].weight = 0;
            },
            name = "get_500_geo_name".Localize(),
            desc = "获得500吉欧",
            weight = 1
        });
        smallRewards.Add(giftname.get_power, new gift()
        {
            level = 2,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_25 = true;
            },
            name = Language.Language.Get("CHARM_NAME_25_G", "UI"),
            desc = "获得坚固力量",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_saman, new gift()
        {
            level = 3,
            reward = (giftname) =>
                    {
                        PlayerData.instance.gotCharm_19 = true;
                    },
            name = Language.Language.Get("CHARM_NAME_19", "UI"),
            desc = "获得萨满之石",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_quick_and_wenti, new gift()
        {
            level = 2,
            reward = (giftname) =>
        {
            PlayerData.instance.gotCharm_14 = true;
            PlayerData.instance.gotCharm_32 = true;

        },
            name = Language.Language.Get("CHARM_NAME_14", "UI") + "&" + Language.Language.Get("CHARM_NAME_32", "UI"),
            desc = "获得稳定之体&快速劈砍",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_jiaoao, new gift()
        {
            level = 1,
            reward = (giftname) =>
                    {
                        PlayerData.instance.gotCharm_13 = true;
                    },
            name = Language.Language.Get("CHARM_NAME_13", "UI"),
            desc = "获得骄傲印记",
            weight = 1f,
        });
        smallRewards.Add(giftname.get_faniu_and_bushou, new gift()
        {
            level = 2,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_33 = true;
                PlayerData.instance.gotCharm_20 = true;
            },
            name = Language.Language.Get("CHARM_NAME_33", "UI") + "&" + Language.Language.Get("CHARM_NAME_20", "UI"),
            desc = "获得法术扭曲者&灵魂捕手",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_shihun, new gift()
        {
            level = 2,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_21 = true;
            }
            ,
            name = Language.Language.Get("CHARM_NAME_21", "UI"),
            desc = "获得噬魂者",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_kuaiju_shenju_wuen, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_7 = true;
                PlayerData.instance.gotCharm_34 = true;
                PlayerData.instance.gotCharm_28 = true;
            },
            name = Language.Language.Get("CHARM_NAME_7", "UI") + "&" + Language.Language.Get("CHARM_NAME_34", "UI") + "&" + Language.Language.Get("CHARM_NAME_28", "UI"),
            desc = "获得快速聚集&深度聚集&乌恩之形",
            weight = 1f,
            active = true,
            force_active = false
        });
        smallRewards.Add(giftname.get_badeer_yingke_wuyou, new gift()
        {
            level = 0,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_5 = true;
                PlayerData.instance.gotCharm_4 = true;
                PlayerData.instance.gotCharm_40 = true;
            },
            name = Language.Language.Get("CHARM_NAME_5", "UI") + "&" + Language.Language.Get("CHARM_NAME_4", "UI") + "&" + Language.Language.Get("CHARM_NAME_40_N", "UI"),
            desc = "获得巴德尔纸壳&坚硬外壳&无忧旋律",
            weight = 1.1f,
        });
        smallRewards.Add(giftname.get_zigong_bianzhizhe_chongge, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_22 = true;
                PlayerData.instance.gotCharm_39 = true;
                PlayerData.instance.gotCharm_3 = true;
            },
            name = Language.Language.Get("CHARM_NAME_22", "UI") + "&" + Language.Language.Get("CHARM_NAME_39", "UI") + "&" + Language.Language.Get("CHARM_NAME_3", "UI"),
            desc = "获得发光子宫&编织者之歌&幼虫之歌",
            weight = 1f,
        });
        smallRewards.Add(giftname.get_any_2_charms, new gift()
        {
            level = 4,
            reward = (giftname) =>
            {
                getAnyCharm += 2;
                smallRewards[giftname.get_any_2_charms].weight = 0;
            },
            name = "get_any_2_charms_name".Localize(),
            desc = "自选任意两个护符",
            weight = 0.7f,
        });
        smallRewards.Add(giftname.get_one_egg, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                relive_num++;
            },
            name = Language.Language.Get("INV_NAME_RANCIDEGG", "UI"),
            desc = "腐臭蛋+1",
            weight = 1f,
        });
        smallRewards.Add(giftname.get_xichong, new gift()
        {
            level = 3,
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_11 = true;
            },
            name = Language.Language.Get("CHARM_NAME_11", "UI"),
            desc = "获得吸虫之巢",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_birthright, new gift()
        {
            level = 3,
            reward = (giftname) =>
            {
                getBirthright = true;
                switch (role)
                {
                    case giftname.nail_master:
                        PlayerData.instance.gotCharm_26 = true;
                        ShowConvo("nail_master_get_birthright_prompt".Localize());
                        break;
                    case giftname.shaman:
                        if (PlayerData.instance.quakeLevel < 2)
                        {
                            PlayerData.instance.quakeLevel++;
                        }
                        ShowConvo("shaman_get_birthright_prompt".Localize());
                        break;
                    case giftname.hunter:
                        PlayerData.instance.charmSlots += Math.Min(3, 11 - PlayerData.instance.charmSlots);
                        ShowConvo("hunter_get_birthright_prompt".Localize());
                        break;
                    case giftname.moth:
                        getAnyCharm++;
                        ShowConvo("moth_get_birthright_prompt".Localize());
                        break;
                    case giftname.grey_prince:
                        PlayerData.instance.gotCharm_24 = true;
                        ShowConvo("grey_prince_get_birthright_prompt".Localize());
                        break;
                    case giftname.Tuk:
                        relive_num += 2;
                        ShowConvo("Tuk_get_birthright_prompt".Localize());
                        break;
                    case giftname.defender:
                        PlayerData.instance.hasAcidArmour = true;
                        ShowConvo("defender_get_birthright_prompt".Localize());
                        break;
                    case giftname.joni:
                        refresh_num++;
                        ShowConvo("joni_get_birthright_prompt".Localize());
                        break;
                    case giftname.test:
                        // PlayerData.instance.xunFlowerGiven = true;
                        ShowConvo("test_get_birthright_prompt".Localize());
                        break;
                    case giftname.mantis:
                        if (PlayerData.instance.hasDash)
                        {
                            PlayerData.instance.hasShadowDash = true;
                            PlayerData.instance.canShadowDash = true;
                            ShowConvo(Language.Language.Get("INV_NAME_SHADOWDASH", "UI"));
                        }
                        else
                        {
                            PlayerData.instance.hasDash = true;
                            PlayerData.instance.canDash = true;
                            ShowConvo(Language.Language.Get("INV_NAME_DASH", "UI"));
                        }
                        break;
                    case giftname.collector:
                        ShowConvo("collector_get_birthright_prompt".Localize());
                        break;
                    default:
                        break;

                }
            },
            name = "birthright",
            desc = "长子权",
            weight = 0,
            active = false,
            force_active = true
        });
        foreach (var item in smallRewards)
        {
            item.Value.giftname = item.Key;
            if (_set.smallgifts.ContainsKey(item.Key) && !item.Value.force_active)
            {
                item.Value.active = _set.smallgifts[item.Key].active;
            }
            if (item.Value.active)
            {
                if (!actSmallRewards.Contains(item.Value))
                {
                    actSmallRewards.Add(item.Value);
                }
            }
            else
            {
                if (actSmallRewards.Contains(item.Value))
                {
                    actSmallRewards.Remove(item.Value);
                }
            }
            switch (item.Value.level)
            {
                case 0:
                    item.Value.weight = 1.1f;
                    break;
                case 1:
                    item.Value.weight = 1f;
                    break;
                case 2:
                    item.Value.weight = 0.9f;
                    break;
                case 3:
                    item.Value.weight = 0.8f;
                    break;
                case 4:
                    item.Value.weight = 0.7f;
                    break;
                case 5:
                    item.Value.weight = 0;
                    break;
            }
        }


        bigRewards.Clear();

        bigRewards.Add(giftname.warrior, new gift()
        {
            level = 5,
            reward = (giftname) =>
            {
                GiveMask();
                GiveMask();
                AddNailDamage();

            },
            name = "warrior_name".Localize(),
            desc = "面具+2&骨钉+1",
            weight = 1f,
        });
        bigRewards.Add(giftname.mage, new gift()
        {
            level = 5,
            reward = (giftname) =>
            {
                GiveVessel();
                List<gift> gifts2 = new();
                if (PlayerData.instance.screamLevel < 2) gifts2.Add(smallRewards[giftname.get_scream]);
                if (PlayerData.instance.fireballLevel < 2) gifts2.Add(smallRewards[giftname.get_fireball]);
                if (PlayerData.instance.quakeLevel < 2) gifts2.Add(smallRewards[giftname.get_quake]);
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_gift, gifts = gifts2 });
            },
            name = "mage_name".Localize(),
            desc = "魂槽+1&法术+1",
            weight = 1f,
        });
        bigRewards.Add(giftname.ranger, new gift()
        {
            level = 5,
            reward = (giftname) =>
            {
                List<gift> gifts1 = new();
                if (!PlayerData.instance.hasCyclone) gifts1.Add(smallRewards[giftname.get_cyclone]);
                if (!PlayerData.instance.hasUpwardSlash) gifts1.Add(smallRewards[giftname.get_dash_slash]);
                if (!PlayerData.instance.hasDashSlash) gifts1.Add(smallRewards[giftname.get_upward_slash]);
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_small_gift, select = 1, gifts = gifts1 });

                List<gift> gifts2 = new();
                if (smallRewards[giftname.get_dash].weight != 0) gifts2.Add(smallRewards[giftname.get_dash]);
                if (smallRewards[giftname.get_zhua_lei_ding].weight != 0) gifts2.Add(smallRewards[giftname.get_zhua_lei_ding]);
                if (smallRewards[giftname.get_double_jump].weight != 0) gifts2.Add(smallRewards[giftname.get_double_jump]);
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_small_gift, select = 1, gifts = gifts2 });
            },
            name = "ranger_name".Localize(),
            desc = "剑技+1&位移+1",
            weight = 1f,
        });
        foreach (var item in bigRewards)
        {
            item.Value.giftname = item.Key;
            if (_set.biggifts.ContainsKey(item.Key) && !item.Value.force_active)
            {
                item.Value.active = _set.biggifts[item.Key].active;
            }
            if (item.Value.active)
            {
                if (!actBigRewards.Contains(item.Value))
                {
                    actBigRewards.Add(item.Value);
                }
            }
            else
            {
                if (actBigRewards.Contains(item.Value))
                {
                    actBigRewards.Remove(item.Value);
                }
            }
        }

        shopRewards.Clear();
        shopRewards.Add(giftname.test, new gift()
        {
            level = 0,
            reward = (giftname) =>
            {
                role = giftname.test;
                ShowDreamConvo("test_dream".Localize());
            },
            name = "test_name".Localize(),
            desc = "test_desc".Localize(),
            weight = 0,
            active = false,
            force_active = true
        });
        shopRewards.Add(giftname.nail_master, new gift()
        {
            //加2骨钉
            //全剑技
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.nail_master;
                AddNailDamage();
                AddNailDamage();
                PlayerData.instance.hasCyclone = true;
                PlayerData.instance.hasDashSlash = true;
                PlayerData.instance.hasUpwardSlash = true;
                PlayerData.instance.hasNailArt = true;
                ShowDreamConvo("nail_master_dream".Localize());
            },
            name = "nail_master_name".Localize(),
            desc = "nail_master_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("B_Great_Nailsage_Sly.png")
        });
        shopRewards.Add(giftname.shaman, new gift()
        {
            //萨满之石
            //获得黑波
            //1.25法术伤害倍率
            //0.8倍骨钉倍率
            //刷新次数+2
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.shaman;
                PlayerData.instance.gotCharm_19 = true;
                PlayerData.instance.fireballLevel = 2;
                refresh_num += 2;
                ShowDreamConvo("shaman_dream".Localize());
            },
            name = "shaman_name".Localize(),
            desc = "shaman_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Snail_Shaman.png")
        });
        shopRewards.Add(giftname.hunter, new gift()
        {
            //白冲
            //二段
            // 爪
            // 超冲
            // 快速劈砍
            // 稳定之体
            // +1护符槽
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.hunter;
                PlayerData.instance.hasDash = true;
                PlayerData.instance.canDash = true;
                PlayerData.instance.hasDoubleJump = true;
                PlayerData.instance.hasWalljump = true;
                PlayerData.instance.canWallJump = true;
                PlayerData.instance.canSuperDash = true;
                PlayerData.instance.hasSuperDash = true;
                PlayerData.instance.gotCharm_14 = true;
                PlayerData.instance.gotCharm_32 = true;
                PlayerData.instance.charmSlots += 1;
                if (PlayerData.instance.charmSlots > 11) PlayerData.instance.charmSlots = 11;
                ShowDreamConvo("hunter_dream".Localize());
            },
            name = "hunter_name".Localize(),
            desc = "hunter_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Hunter_Full_Body.png"),
            scale = new Vector2(0.2f, 0.2f)
        });
        shopRewards.Add(giftname.uunn, new gift()
        {
            //delete?
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.uunn;
                PlayerData.instance.charmSlots += 2;
                if (PlayerData.instance.charmSlots > 11) PlayerData.instance.charmSlots = 11;
                PlayerData.instance.gotCharm_7 = true;
                PlayerData.instance.gotCharm_28 = true;
            },
            name = "乌恩",
            desc = "获得乌恩之形&快速聚集\n+2护符槽\n全boss视为无伤",
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Unn.png"),
            scale = new Vector2(0.15f, 0.15f)

        });
        shopRewards.Add(giftname.joni, new gift()
        {
            //护符槽血魂全满
            //绑定乔尼的祝福
            //解锁全部护符
            //1.75倍骨钉倍率
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.joni;
                PlayerData.instance.charmSlots = 11;
                if (PlayerData.instance.maxHealthBase < 9)
                {
                    while (PlayerData.instance.maxHealthBase < 9)
                    {
                        GiveMask();
                    }
                }
                else
                {
                    while (PlayerData.instance.maxHealthBase > 9)
                    {
                        TakeAwayMask();
                    }
                }
                while (PlayerData.instance.MPReserveMax < 99)
                {
                    GiveVessel();
                }
                for (int i = 1; i <= 40; i++)
                {
                    PlayerData.instance.SetBool("gotCharm_" + i, true);
                }
                if (!PlayerData.instance.equippedCharms.Contains(27))
                {
                    PlayerData.instance.equippedCharms.Insert(1, 27);
                    PlayerData.instance.equippedCharm_27 = true;
                }
                ShowDreamConvo("joni_dream".Localize());
            },
            name = "joni_name".Localize(),
            desc = "joni_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Joni.png"),
            scale = new Vector2(0.7f, 0.7f)
        });
        shopRewards.Add(giftname.moth, new gift()
        {
            //梦之钉
            //1自选护符
            //+1骨钉
            //+1面具
            //+8护符槽
            //+1魂槽
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.moth;
                PlayerData.instance.hasDreamNail = true;
                getAnyCharm += 1;
                AddNailDamage();
                GiveMask();
                GiveVessel();
                PlayerData.instance.charmSlots = 11;
                ShowDreamConvo("moth_dream".Localize());
            },
            name = "moth_name".Localize(),
            desc = "moth_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Seer.png"),
            scale = new Vector2(0.6f, 0.6f)
        });
        shopRewards.Add(giftname.grey_prince, new gift()
        {
            //骨钉固定为-1钉
            //全白法+黑冲
            //护符不消耗护符槽
            //梦钉
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.grey_prince;
                PlayerData.instance.fireballLevel = 1;
                PlayerData.instance.screamLevel = 1;
                PlayerData.instance.quakeLevel = 1;
                PlayerData.instance.nailDamage = 5;
                PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
                PlayerData.instance.hasDash = true;
                PlayerData.instance.canDash = true;
                PlayerData.instance.hasShadowDash = true;
                PlayerData.instance.canShadowDash = true;
                PlayerData.instance.hasDreamNail = true;
                ShowDreamConvo("grey_prince_dream".Localize());
            },
            name = "grey_prince_name".Localize(),
            desc = "grey_prince_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Grey_Prince_Zote_Idle.png"),
            scale = new Vector2(0.45f, 0.45f)
        });
        shopRewards.Add(giftname.Tuk, new gift()
        {
            //5个腐臭蛋
            //500geo
            //每次使用腐臭蛋后+1骨钉或+2面具
            //初始只有三血
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.Tuk;
                while (PlayerData.instance.maxHealthBase > 3) TakeAwayMask();
                HeroController.instance.AddGeo(500);
                relive_num = 5;
                ShowDreamConvo("Tuk_dream".Localize());
            },
            name = "Tuk_name".Localize(),
            desc = "Tuk_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Tuk.png"),
            scale = new Vector2(0.45f, 0.45f)
        });
        shopRewards.Add(giftname.defender, new gift()
        {
            //绑定英勇气息
            //1.25倍骨钉倍率
            //+1骨钉
            //+白砸
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.defender;
                AddNailDamage();
                PlayerData.instance.quakeLevel = 1;
                // PlayerData.instance.gotCharm_25 = true;
                PlayerData.instance.gotCharm_10 = true;
                if (!PlayerData.instance.equippedCharms.Contains(10))
                {
                    PlayerData.instance.equippedCharms.Insert(1, 10);
                    PlayerData.instance.equippedCharm_10 = true;
                }


            },
            name = "奥格瑞姆",
            desc = "绑定防御者纹章\n+1护符槽&获得坚固力量&+1骨钉&获得白砸",
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("White_Defender_without_Essence.png"),
            scale = new Vector2(0.5f, 0.5f)
        });
        shopRewards.Add(giftname.mantis, new gift()
        {
            //获得螳螂爪
            //绑定骄傲印记
            //
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.mantis;
                PlayerData.instance.hasWalljump = true;
                PlayerData.instance.gotCharm_13 = true;
                if (!PlayerData.instance.equippedCharms.Contains(13))
                {
                    PlayerData.instance.equippedCharms.Insert(1, 13);
                    PlayerData.instance.equippedCharm_13 = true;
                    PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
                    // HeroController.instance.CharmUpdate();
                }
                ShowDreamConvo("mantis_dream".Localize());
            },
            name = "mantis_name".Localize(),
            desc = "mantis_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("mantis.png"),
            scale = new Vector2(0.55f, 0.55f)
        });
        shopRewards.Add(giftname.collector, new gift()
        {
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.collector;
                PlayerData.instance.gotCharm_40 = true;
                PlayerData.instance.grimmChildLevel = 4;
                PlayerData.instance.charmCost_40 = 2;
                PlayerData.instance.gotCharm_38 = true;
                PlayerData.instance.equippedCharm_38 = true;
                PlayerData.instance.equippedCharms.Add(38);
                ShowDreamConvo("collector_dream".Localize());
            },
            name = "collector_name".Localize(),
            desc = "collector_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("B_Collector.png"),
            scale = new Vector2(0.5f, 0.5f)
        });


        shopRewards.Add(giftname.shop_keeper_key, new gift()
        {
            price = 100,
            reward = (giftname) =>
            {
                itemManager.NormalShopItem();
            },
            name = "shop_keeper_key_name".Localize(),
            desc = "shop_keeper_key_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("keeper_key.png")
        });
        shopRewards.Add(giftname.shop_add_1_notch_1, new gift()
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name".Localize(),
            desc = "shop_add_1_notch_desc".Localize(),
            active = false,
            weight = 1,
            sprite = AssemblyUtils.GetSpriteFromResources("Charm_Notch.png")
        });
        shopRewards.Add(giftname.shop_nail_upgrade, new gift()
        {
            price = 500,
            reward = (giftname) =>
            {
                AddNailDamage();
            },
            name = "shop_nail_upgrade_name".Localize(),
            desc = "shop_nail_upgrade_desc".Localize(),
            active = false,
            weight = 1,
            sprite = AssemblyUtils.GetSpriteFromResources("pale_stone.png")
        });
        shopRewards.Add(giftname.shop_random_gift, new gift()
        {
            price = 400,
            reward = (giftname) =>
            {
                itemManager.RandomOneSmall();
            },
            name = "shop_random_gift_name".Localize(),
            desc = "shop_random_gift_desc".Localize(),
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("witches_eye.png")

        });
        shopRewards.Add(giftname.shop_any_charm_1, new gift()
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 2
        });
        shopRewards.Add(giftname.shop_any_charm_2, new gift()
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 1.5f,
        });
        shopRewards.Add(giftname.shop_any_charm_3, new gift()
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 0.8f
        });
        shopRewards.Add(giftname.shop_any_charm_4, new gift()
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 0.8f
        });
        shopRewards.Add(giftname.shop_add_1_notch_2, new gift()
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name".Localize(),
            desc = "shop_add_1_notch_desc".Localize(),
            weight = 1,
            getSprite = (giftname) =>
            {
                return shopRewards[giftname.shop_add_1_notch_1].sprite;
            }
        });
        shopRewards.Add(giftname.shop_add_1_notch_3, new gift()
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name".Localize(),
            desc = "shop_add_1_notch_desc".Localize(),
            weight = 1,
            getSprite = (giftname) =>
            {
                return shopRewards[giftname.shop_add_1_notch_1].sprite;
            }
        });
        shopRewards.Add(giftname.shop_add_1_notch_4, new gift()
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name".Localize(),
            desc = "shop_add_1_notch_desc".Localize(),
            weight = 1,
            getSprite = (giftname) =>
            {
                return shopRewards[giftname.shop_add_1_notch_1].sprite;
            }
        });
        shopRewards.Add(giftname.shop_egg, new gift()
        {
            price = 400,
            reward = (giftname) =>
            {
                relive_num++;
            },
            name = "shop_egg_name".Localize(),
            desc = "shop_egg_desc".Localize(),
            weight = 3,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Rancid Egg").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_super_dash, new gift()
        {
            price = 100,
            reward = (giftname) =>
            {
                PlayerData.instance.hasSuperDash = true;
                PlayerData.instance.canSuperDash = true;
            },
            name = Language.Language.Get("INV_NAME_SUPERDASH", "UI"),
            desc = Language.Language.Get("INV_DESC_SUPERDASH", "UI"),
            weight = 0.8f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Super Dash").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_wall_jump, new gift()
        {
            price = 200,
            reward = (giftname) =>
            {
                PlayerData.instance.hasWalljump = true;
                PlayerData.instance.canWallJump = true;
            },
            name = Language.Language.Get("INV_NAME_WALLJUMP", "UI"),
            desc = Language.Language.Get("INV_DESC_WALLJUMP", "UI"),
            weight = 0.8f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Mantis Claw").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_dream_nail, new gift()
        {
            price = 100,
            reward = (giftname) =>
            {
                PlayerData.instance.hasDreamNail = true;

            },
            name = Language.Language.Get("INV_NAME_DREAMNAIL_A", "UI"),
            desc = Language.Language.Get("INV_DESC_DREAMNAIL_A", "UI"),
            weight = 0.8f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Inv_Items").FindGameObjectInChildren("Dream Nail").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_add_1_mask, new gift()
        {
            price = 250,
            reward = (giftname) =>
            {
                GiveMask();
            },
            name = "shop_add_1_mask_name".Localize(),
            desc = "shop_add_1_mask_desc".Localize(),
            weight = 1.5f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Inv_Items").FindGameObjectInChildren("Heart Pieces").FindGameObjectInChildren("Pieces 4").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            },
            scale = new Vector2(0.25f, 0.25f)
        });
        shopRewards.Add(giftname.shop_add_1_vessel, new gift()
        {
            price = 200,
            reward = (giftname) =>
            {
                GiveVessel();
            },
            name = "shop_add_1_vessel_name".Localize(),
            desc = "shop_add_1_vessel_desc".Localize(),
            weight = 1.5f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Inv_Items").FindGameObjectInChildren("Soul Orb").FindGameObjectInChildren("Piece All").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_prettey_key, new gift()
        {
            price = 8,
            reward = (giftname) =>
            {
                List<string> dreams = new List<string>
                {
                    "pretty_key_Easter_egg_1".Localize(),
                    "pretty_key_Easter_egg_2".Localize(),
                    "pretty_key_Easter_egg_3".Localize(),
                    "pretty_key_Easter_egg_4".Localize(),
                    "pretty_key_Easter_egg_5".Localize(),
                    "pretty_key_Easter_egg_6".Localize(),
                    "pretty_key_Easter_egg_7".Localize(),
                };
                if (pretty_key_num == dreams.Count)
                {
                    ShowDreamConvo(dreams[pretty_key_num - 1]);
                    _set.owner = true;
                }
                else if (pretty_key_num < dreams.Count)
                {
                    ShowDreamConvo(dreams[pretty_key_num]);
                    pretty_key_num++;
                    if (pretty_key_num == dreams.Count)
                        _set.owner = true;
                }


            },
            name = Language.Language.Get("INV_NAME_WHITEKEY", "UI"),
            desc = "shop_pretty_key_desc".Localize(),
            weight = 0.1f,
            sprite = AssemblyUtils.GetSpriteFromResources("Elegant_Key.png"),
        });
        shopRewards.Add(giftname.shop_refresh, new gift()
        {
            price = 150,
            reward = (giftname) =>
            {
                refresh_num++;
            },
            name = "shop_refresh_name".Localize(),
            desc = "shop_refresh_desc".Localize(),
            weight = 2f,
            sprite = AssemblyUtils.GetSpriteFromResources("easy_key.png"),
        });
        shopRewards.Add(giftname.shop_acid_swim, new gift()
        {
            price = 50,
            reward = (giftname) =>
            {
                PlayerData.instance.hasAcidArmour = true;
            },
            name = Language.Language.Get("INV_NAME_ACIDARMOUR", "UI"),
            desc = Language.Language.Get("INV_DESC_ACIDARMOUR", "UI"),
            weight = 0.8f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Acid Armour").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_double_jump, new gift()
        {
            price = 600,
            reward = (giftname) =>
            {
                PlayerData.instance.hasDoubleJump = true;
            },
            name = Language.Language.Get("INV_NAME_DOUBLEJUMP", "UI"),
            desc = Language.Language.Get("INV_DESC_DOUBLEJUMP", "UI"),
            weight = 0.4f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Double Jump").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        shopRewards.Add(giftname.shop_dash, new gift()
        {
            price = 600,
            reward = (giftname) =>
            {
                if (!PlayerData.instance.hasDash)
                {
                    PlayerData.instance.hasDash = true;
                    PlayerData.instance.canDash = true;
                }
                else
                {
                    PlayerData.instance.hasShadowDash = true;
                    PlayerData.instance.canShadowDash = true;
                }
            },
            name = "shop_dash_name".Localize(),
            desc = "shop_dash_desc".Localize(),
            weight = 0.4f,
            getSprite = (giftname) =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").FindGameObjectInChildren("Inventory").Find("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Dash Cloak").GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        foreach (var item in shopRewards)
        {
            // if (_set.biggifts.ContainsKey(item.Key))
            // {
            //     item.Value.active = _set.biggifts[item.Key].active;
            item.Value.giftname = item.Key;
            // }
            if (item.Value.active)
            {
                if (!actShopRewards.Contains(item.Value))
                {
                    actShopRewards.Add(item.Value);
                }
            }
            else
            {
                if (actShopRewards.Contains(item.Value))
                {
                    actShopRewards.Remove(item.Value);
                }
            }
        }

        CharmRewardsInit();
    }

    private void CharmRewardsInit()
    {
        charmRewards.Clear();
        actCharmRewards.Clear();

        List<int> breakable = new List<int> { 23, 24, 25 };
        for (int i = 1; i <= 40; i++)
        {
            if (i == 36) continue;
            charmRewards.Add((giftname)i, new gift()
            {
                giftname = (giftname)i,
                name = Language.Language.Get("CHARM_NAME_" + i + (breakable.Contains(i) ? "_G" : "") + (i == 40 ? "_N" : ""), "UI"),
                desc = Language.Language.Get("CHARM_DESC_" + i + (breakable.Contains(i) ? "_G" : "") + (i == 40 ? "_N" : ""), "UI").Replace("<br>", "\n"),
                weight = charm_weight_and_price.ContainsKey((giftname)i) ? charm_weight_and_price[(giftname)i].Item1 : 0,
                price = charm_weight_and_price.ContainsKey((giftname)i) ? charm_weight_and_price[(giftname)i].Item2 : 999,
                sprite = null,
                reward = (giftname) =>
                {
                    PlayerData.instance.SetBool("gotCharm_" + (int)giftname, true);
                },
                getSprite = (giftname) =>
                {
                    List<int> breakable = new List<int> { 23, 24, 25 };
                    Log(giftname);
                    Log((int)giftname);
                    Log("i= " + i);
                    var charmIcon = HutongGames.PlayMaker.FsmVariables.GlobalVariables.FindFsmGameObject("Charm Icons").Value;
                    if (charmIcon == null) return null;
                    if (!breakable.Contains((int)giftname) && (int)giftname != 40)
                        return charmIcon.GetComponent<CharmIconList>().spriteList[(int)giftname];
                    if ((int)giftname == 23)
                    {
                        return charmIcon.GetComponent<CharmIconList>().unbreakableHeart;
                    }
                    if ((int)giftname == 24)
                    {
                        return charmIcon.GetComponent<CharmIconList>().unbreakableGreed;
                    }
                    if ((int)giftname == 25)
                    {
                        return charmIcon.GetComponent<CharmIconList>().unbreakableStrength;
                    }
                    if ((int)giftname == 40)
                    {
                        return charmIcon.GetComponent<CharmIconList>().nymmCharm;
                    }
                    return null;
                }
            });
            if (charmRewards[(giftname)i].active) actCharmRewards.Add(charmRewards[(giftname)i]);
        }
    }


    private void CharmsInit(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "UI Charms" && self.gameObject.name == "Charms")
        {
            charms = self.gameObject;
            charms.FindGameObjectInChildren("Equipped Charms").FindGameObjectInChildren("Notches").FindGameObjectInChildren("Text Notches").GetComponent<TMPro.TextMeshPro>().text = "get_any_charm_front".Localize() + getAnyCharm + "get_any_charm_behind".Localize();
            Log("Enable");
            // self.gameObject.AddComponent<Test>();
            self.ChangeTransition("Charm Collected?", "UNCOLLECTED", "Collected");
            if (self.GetState("Collected").Actions.Length == 10)
            {
                self.InsertCustomAction("Collected", (fsm) =>
                {
                    charmString = fsm.FsmVariables.GetFsmString("Build String").Value;
                    fsm.FsmVariables.GetFsmBool("Got Charm").Value = GameManager.instance.GetPlayerDataBool(fsm.FsmVariables.GetFsmString("Build String").Value);
                }, 1);
            }
            if (self.GetState("Deactivate UI").Actions.Length == 6)
            {
                self.RemoveAction("Deactivate UI", 2);
                self.InsertCustomAction("Deactivate UI", (fsm) =>
                {
                    if ((itemManager.scenename == "GG_Atrium_Roof" || itemManager.scenename == "GG_Engine") && inRogue)
                    {

                    }
                    else
                    {
                        if (!PlayerData.instance.atBench) fsm.SendEvent("NOT BENCH");
                    }
                }, 2);
                self.InsertCustomAction("Deactivate UI", (fsm) =>
                {
                    var val = fsm.FsmVariables.GetFsmBool("Got Charm").Value;
                    if (!val && getAnyCharm > 0)
                    {
                        GameManager.instance.SetPlayerDataBool(charmString, true);
                        fsm.FsmVariables.GetFsmBool("Got Charm").Value = true;
                        getAnyCharm--;
                        charms.FindGameObjectInChildren("Equipped Charms").FindGameObjectInChildren("Notches").FindGameObjectInChildren("Text Notches").GetComponent<TMPro.TextMeshPro>().text = "get_any_charm_front".Localize() + getAnyCharm + "get_any_charm_behind".Localize();
                    }
                }, 1);
            }
            if (self.GetState("Black Charm?").Actions.Length == 2)
            {
                self.InsertCustomAction("Black Charm?", (fsm) =>
                {
                    if (role == giftname.joni)
                    {
                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 27) fsm.SendEvent("CANCEL");
                    }
                    if (role == giftname.defender)
                    {
                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 10) fsm.SendEvent("CANCEL");
                    }
                    if (role == giftname.mantis)
                    {
                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 13) fsm.SendEvent("CANCEL");
                    }
                    if (role == giftname.collector)
                    {
                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 38) fsm.SendEvent("CANCEL");
                    }
                }, 0);
            }
            if (self.GetState("Black Charm? 2").Actions.Length == 2)
            {
                self.InsertCustomAction("Black Charm? 2", (fsm) =>
                                {
                                    if (role == giftname.joni)
                                    {
                                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 27) fsm.SendEvent("CANCEL");
                                    }
                                    if (role == giftname.defender)
                                    {
                                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 10) fsm.SendEvent("CANCEL");
                                    }
                                    if (role == giftname.mantis)
                                    {
                                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 13) fsm.SendEvent("CANCEL");
                                    }
                                    if (role == giftname.collector)
                                    {
                                        if (fsm.GetVariable<FsmInt>("Current Item Number").Value == 38) fsm.SendEvent("CANCEL");

                                    }
                                }, 0);
            }
        }
        orig(self);
    }

    public void Rogue_Reset()
    {
        if (PlayerData.instance.maxHealthBase > 5)
        {
            while (PlayerData.instance.maxHealthBase > 5)
            {
                TakeAwayMask();
            }
        }
        else
        {
            while (PlayerData.instance.maxHealthBase < 5)
            {
                GiveMask();
            }
        }
        while (PlayerData.instance.MPReserveMax > 0)
        {
            TakeAwayVessel();
        }
        RemoveAllCharms();
        RemoveAllSkills();
        ResetWeight();
        List<GameObject> childlist = new();
        GameObject itemlist = menu_go.FindGameObjectInChildren("Item List");
        itemlist.FindAllChildren(childlist);
        foreach (var child in childlist)
        {
            if (child == itemlist) continue;
            else GameObject.Destroy(child);
        }


        spa_count = 0;
        PlayerData.instance.geo = 0;
        itemManager.timer = 0;
        pretty_key_num = 0;
        itemManager.timeSpan = TimeSpan.FromSeconds(0);
        role = null;
        getBirthright = false;







    }
    public void Unload()
    {
        if (itemManager != null)
            GameObject.Destroy(itemManager.gameObject);
    }
    public void Rogue_Start()
    {
        if (itemManager != null)
        {
            itemManager.DestroyAllItems();
            itemManager.rewardsStack.Clear();
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_big_gift, select = 1, gifts = actBigRewards });
            itemManager.Next(true);
            getAnyCharm = 0;
            relive_num = 1;
            refresh_num = 3;
            itemManager.BeginDisplay();
            inRogue = true;
            HeroController.instance.geoCounter.gameObject.GetComponent<DeactivateIfPlayerdataTrue>().enabled = false;
            HeroController.instance.geoCounter.gameObject.SetActive(true);
            HeroController.instance.TakeGeo(PlayerData.instance.geo);
        }

    }

    public void Rogue_Over()
    {
        if (itemManager != null)
        {
            inRogue = false;
            getBirthright = false;
            HeroController.instance.geoCounter.gameObject.GetComponent<DeactivateIfPlayerdataTrue>().enabled = false;
            HeroController.instance.geoCounter.gameObject.SetActive(true);
            HeroController.instance.TakeGeo(PlayerData.instance.geo);
            HeroController.instance.geoCounter.gameObject.SetActive(false);
            HeroController.instance.geoCounter.gameObject.GetComponent<DeactivateIfPlayerdataTrue>().enabled = true;
            PlayerData.instance.nailDamage = 21;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            if (PlayerData.instance.maxHealthBase < 9)
            {
                while (PlayerData.instance.maxHealthBase < 9)
                {
                    GiveMask();
                }
            }
            else
            {
                while (PlayerData.instance.maxHealthBase > 9)
                {
                    TakeAwayMask();
                }
            }
            while (PlayerData.instance.MPReserveMax < 99)
            {
                GiveVessel();
            }

            GiveAllCharms();
            GiveAllSkills();
            role = null;

        }
    }


    public static void GiveMask()
    {
        if (PlayerData.instance.maxHealthBase < 9)
        {
            // HeroController.instance.MaxHealth();
            HeroController.instance.AddToMaxHealth(1);
            PlayMakerFSM.BroadcastEvent("MAX HP UP");
        }
        else
        {
        }
    }
    public static void GiveVessel()
    {
        if (PlayerData.instance.MPReserveMax < 99)
        {
            HeroController.instance.AddToMaxMPReserve(33);
            PlayMakerFSM.BroadcastEvent("NEW SOUL ORB");
        }
        else
        {
        }
    }
    public static void AddNailDamage()
    {
        if (PlayerData.instance.nailDamage < 21)
        {
            PlayerData.instance.nailDamage += 4;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
        }
    }
    public static void TakeAwayMask()
    {
        if (PlayerData.instance.maxHealthBase > 1)
        {
            PlayerData.instance.maxHealth -= 1;
            PlayerData.instance.maxHealthBase -= 1;
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        }
        else
        {
        }
    }
    //虚空之心36
    public static void TakeAwayVessel()
    {
        if (PlayerData.instance.MPReserveMax > 0)
        {
            PlayerData.instance.MPReserveMax -= 33;
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        }
        else
        {
        }
    }
    private static void UpdateCharmsEffects()
    {
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
    }

    public static void RemoveAllCharms()
    {
        for (int i = 1; i <= 40; i++)
        {
            PlayerData.instance.SetBoolInternal("gotCharm_" + i, false);
            PlayerData.instance.SetBoolInternal("equippedCharm_" + i, false);
        }


        PlayerData.instance.hasCharm = true;
        PlayerData.instance.charmsOwned = 1;
        PlayerData.instance.gotShadeCharm = false;
        PlayerData.instance.fragileGreed_unbreakable = true;
        PlayerData.instance.fragileStrength_unbreakable = true;
        PlayerData.instance.fragileHealth_unbreakable = true;
        PlayerData.instance.grimmChildLevel = 5;
        PlayerData.instance.charmCost_40 = 2;
        PlayerData.instance.charmSlots = 3;
        PlayerData.instance.equippedCharms.Clear();
        PlayerData.instance.equippedCharms.Add(36);
        PlayerData.instance.charmCost_36 = 0;
        PlayerData.instance.royalCharmState = 4;

        PlayerData.instance.SetBoolInternal("gotCharm_" + "36", true);
        PlayerData.instance.SetBoolInternal("equippedCharm_" + "36", true);
        HeroController.instance.CharmUpdate();

        UpdateCharmsEffects();
    }
    public static void RemoveAllSkills()
    {
        PlayerData.instance.screamLevel = 0;
        PlayerData.instance.fireballLevel = 0;
        PlayerData.instance.quakeLevel = 0;
        PlayerData.instance.nailDamage = 5;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

        PlayerData.instance.hasDash = false;
        PlayerData.instance.canDash = false;
        PlayerData.instance.hasShadowDash = false;
        PlayerData.instance.canShadowDash = false;
        PlayerData.instance.hasWalljump = false;
        PlayerData.instance.canWallJump = false;
        PlayerData.instance.hasDoubleJump = false;
        PlayerData.instance.hasSuperDash = false;
        PlayerData.instance.canSuperDash = false;
        PlayerData.instance.hasAcidArmour = false;

        PlayerData.instance.hasDreamNail = false;
        PlayerData.instance.dreamNailUpgraded = false;
        PlayerData.instance.hasDreamGate = false;

        PlayerData.instance.hasNailArt = false;
        PlayerData.instance.hasCyclone = false;
        PlayerData.instance.hasDashSlash = false;
        PlayerData.instance.hasUpwardSlash = false;
        PlayerData.instance.hasAllNailArts = false;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    public static void GiveAllSkills()
    {
        PlayerData.instance.screamLevel = 2;
        PlayerData.instance.fireballLevel = 2;
        PlayerData.instance.quakeLevel = 2;

        PlayerData.instance.hasDash = true;
        PlayerData.instance.canDash = true;
        PlayerData.instance.hasShadowDash = true;
        PlayerData.instance.canShadowDash = true;
        PlayerData.instance.hasWalljump = true;
        PlayerData.instance.canWallJump = true;
        PlayerData.instance.hasDoubleJump = true;
        PlayerData.instance.hasSuperDash = true;
        PlayerData.instance.canSuperDash = true;
        PlayerData.instance.hasAcidArmour = true;

        PlayerData.instance.hasDreamNail = true;
        PlayerData.instance.dreamNailUpgraded = true;
        PlayerData.instance.hasDreamGate = true;

        PlayerData.instance.hasNailArt = true;
        PlayerData.instance.hasCyclone = true;
        PlayerData.instance.hasDashSlash = true;
        PlayerData.instance.hasUpwardSlash = true;
        PlayerData.instance.hasAllNailArts = true;

    }

    public static void GiveAllCharms()
    {
        for (int i = 1; i <= 40; i++)
        {
            PlayerData.instance.SetBoolInternal("gotCharm_" + i, true);
        }

        PlayerData.instance.charmSlots = 10;
        PlayerData.instance.hasCharm = true;
        PlayerData.instance.charmsOwned = 40;
        PlayerData.instance.royalCharmState = 4;
        PlayerData.instance.gotShadeCharm = true;
        PlayerData.instance.charmCost_36 = 0;
        PlayerData.instance.fragileGreed_unbreakable = true;
        PlayerData.instance.fragileStrength_unbreakable = true;
        PlayerData.instance.fragileHealth_unbreakable = true;
        PlayerData.instance.grimmChildLevel = 5;
        PlayerData.instance.charmCost_40 = 3;
        PlayerData.instance.charmSlots = 11;

    }
    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        Satchel.BetterMenus.Menu menu = new("Rogue");
        var b = new Satchel.BetterMenus.KeyBind("rouge_refresh".Localize(), self_actions.refresh);
        var c = new Satchel.BetterMenus.KeyBind("rouge_end".Localize(), self_actions.over);
        var d = new Satchel.BetterMenus.KeyBind("rogue_begin".Localize(), self_actions.start);
        menu.AddElement(b);
        menu.AddElement(d);
        menu.AddElement(c);

        var text = new Satchel.BetterMenus.TextPanel("menu_intro".Localize(), width: 1500);
        Menu giftBase = new("menu_gift_base".Localize());
        int count = 0;
        List<List<Element>> list = new();
        List<Element> tempList = null;
        giftBase.AddElement(new TextPanel("menu_small_gift".Localize()));

        foreach (var gift in smallRewards)
        {
            gift g = gift.Value;
            if (g.desc == null) continue;
            var temp = Blueprints.ToggleButton(g.desc.Replace('\n', '&'), "",
            (act) =>
            {
                g.active = act;
                if (act)
                {
                    if (!actSmallRewards.Contains(g))
                    {
                        actSmallRewards.Add(g);
                    }
                }
                else
                {
                    if (actSmallRewards.Contains(g))
                    {
                        actSmallRewards.Remove(g);
                    }
                }
            },
            () =>
            {
                return g.active;
            });
            if (count == 2)
            {
                giftBase.AddElement(new MenuRow(tempList, ""));
                count = 0;
            }
            if (count == 0) tempList = new();
            tempList.Add(temp);
            count++;

        }
        //TODO 游戏中天赋库不出现的bug
        Log(count);
        Log(tempList.Count);
        if (count > 0)
        {
            giftBase.AddElement(new MenuRow(tempList, "1"));
        }
        giftBase.AddElement(new TextPanel("menu_big_gift".Localize()));
        tempList.Clear();
        count = 0;

        foreach (var gift in bigRewards)
        {
            gift g = gift.Value;
            if (g.desc == null) continue;
            var temp = Blueprints.ToggleButton(g.desc, "",
            (act) =>
            {
                g.active = act;
                if (act)
                {
                    if (!actBigRewards.Contains(g))
                    {
                        actBigRewards.Add(g);
                    }
                }
                else
                {
                    if (actBigRewards.Contains(g))
                    {
                        actBigRewards.Remove(g);
                    }
                }
            },
            () =>
            {
                return g.active;
            });
            if (count == 2)
            {
                giftBase.AddElement(new MenuRow(tempList, ""));
                count = 0;
            }
            if (count == 0) tempList = new();
            tempList.Add(temp);
            count++;

        }
        if (count > 0) giftBase.AddElement(new MenuRow(tempList, ""));

        var gs = giftBase.GetMenuScreen(giftBase.returnScreen);
        menu.AddElement(new MenuButton("menu_gift_base".Localize(), "", (but) =>
        {
            Utils.GoToMenuScreen(gs);
        }));


        Menu setting = new("Settings");
        setting.AddElement(new Satchel.BetterMenus.CustomSlider("font_size", (size) => { _set.item_font_size = size; }, () => _set.item_font_size, 0, 10));
        setting.AddElement(new Satchel.BetterMenus.CustomSlider("ui_transparency", (alpha) => { _set.UI_alpha = alpha; }, () => _set.UI_alpha, 0, 1));
        var ss = setting.GetMenuScreen(setting.returnScreen);
        menu.AddElement(new MenuButton("Settings", "", (but) =>
        {
            Utils.GoToMenuScreen(ss);
        }));
        menu.AddElement(text);
        var ms = menu.GetMenuScreen(modListMenu);
        giftBase.returnScreen = ms;
        setting.returnScreen = ms;
        return ms;

    }

    public void OnLoadGlobal(setting s)
    {
        Log("Onload");
        _set = s;
        if (_set.start == null) ;
        else if (_set.start is KeyBindingSource keyBindingSource1)
        {
            KeyBindingSource start = new KeyBindingSource((Key)_set.startname);
            self_actions.start.AddBinding(start);
        }
        else if (_set.start is MouseBindingSource mouseBindingSource)
        {
            MouseBindingSource start = new MouseBindingSource((Mouse)_set.startname);
            self_actions.start.AddBinding(start);
        }

        if (_set.over == null) ;
        else if (_set.over is KeyBindingSource keyBindingSource1)
        {
            KeyBindingSource over = new KeyBindingSource((Key)_set.overname);
            self_actions.over.AddBinding(over);
        }
        else if (_set.over is MouseBindingSource mouseBindingSource)
        {
            MouseBindingSource over = new MouseBindingSource((Mouse)_set.overname);
            self_actions.over.AddBinding(over);
        }

        if (_set.refresh == null) ;
        else if (_set.refresh is KeyBindingSource keyBindingSource1)
        {
            KeyBindingSource refresh = new KeyBindingSource((Key)_set.refreshname);
            self_actions.refresh.AddBinding(refresh);
        }
        else if (_set.refresh is MouseBindingSource mouseBindingSource)
        {
            MouseBindingSource refresh = new MouseBindingSource((Mouse)_set.refreshname);
            self_actions.refresh.AddBinding(refresh);
        }




    }

    public setting OnSaveGlobal()
    {
        _set.smallgifts = smallRewards;
        _set.biggifts = bigRewards;
        if (self_actions.refresh.Bindings.Count > 0)
        {
            _set.refresh = self_actions.refresh.Bindings[0];
            if (_set.refresh is KeyBindingSource keyBindingSource)
            {
                _set.refreshname = ReflectionHelper.GetField<KeyCombo, ulong>(keyBindingSource.Control, "includeData");
            }
            else if (_set.refresh is MouseBindingSource mouseBindingSource)
            {
                _set.refreshname = (ulong)mouseBindingSource.Control;
            }
        }
        if (self_actions.over.Bindings.Count > 0)
        {
            _set.over = self_actions.over.Bindings[0];
            if (_set.over is KeyBindingSource keyBindingSource)
            {
                _set.overname = ReflectionHelper.GetField<KeyCombo, ulong>(keyBindingSource.Control, "includeData");
            }
            else if (_set.over is MouseBindingSource mouseBindingSource)
            {
                _set.overname = (ulong)mouseBindingSource.Control;
            }
        }
        if (self_actions.start.Bindings.Count > 0)
        {
            _set.start = self_actions.start.Bindings[0];
            if (_set.start is KeyBindingSource keyBindingSource)
            {
                _set.startname = ReflectionHelper.GetField<KeyCombo, ulong>(keyBindingSource.Control, "includeData");
            }
            else if (_set.start is MouseBindingSource mouseBindingSource)
            {
                _set.startname = (ulong)mouseBindingSource.Control;
            }
        }
        return _set;
    }


    private void ShowConvo(string msg)
    {

        if (shiny_item == null) return;
        GameObject get_msg = shiny_item.LocateMyFSM("Shiny Control").GetAction<SpawnObjectFromGlobalPool>("Trink Flash", 5).gameObject.Value;
        GameObject new_msg = get_msg.Spawn(new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        new_msg.FindGameObjectInChildren("Icon").GetComponent<SpriteRenderer>().sprite = null;
        new_msg.FindGameObjectInChildren("Text").GetComponent<TMPro.TextMeshPro>().text = msg;
        return;
    }

    public void ShowDreamConvo(string msg)
    {
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HutongGames.PlayMaker.FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "rogue_" + msg;
        playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;

        playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
    }
    public void UpdateWeight()
    {
        if (role == giftname.grey_prince)
        {
            PlayerData.instance.nailDamage = 5;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
        }
        PlayerData playerData = PlayerData.instance;
        if (playerData.maxHealthBase >= 9) smallRewards[giftname.add_2_mask].weight = 0;
        if (playerData.MPReserveMax >= 99) smallRewards[giftname.add_1_vessel].weight = 0;
        if (playerData.nailDamage >= 21) smallRewards[giftname.nail_upgrade].weight = 0;
        if (playerData.charmSlots >= 11) smallRewards[giftname.add_3_notch].weight = 0;
        if (playerData.fireballLevel == 2) smallRewards[giftname.get_fireball].weight = 0;
        if (playerData.screamLevel == 2) smallRewards[giftname.get_scream].weight = 0;
        if (playerData.quakeLevel == 2) smallRewards[giftname.get_quake].weight = 0;
        if (playerData.hasDash && playerData.hasShadowDash) smallRewards[giftname.get_dash].weight = 0;
        if (playerData.hasDoubleJump) smallRewards[giftname.get_double_jump].weight = 0;
        if (playerData.hasWalljump && playerData.hasSuperDash && playerData.hasAcidArmour && playerData.hasDreamNail) smallRewards[giftname.get_zhua_lei_ding].weight = 0;
        if (playerData.hasUpwardSlash) smallRewards[giftname.get_dash_slash].weight = 0;
        if (playerData.hasDashSlash) smallRewards[giftname.get_upward_slash].weight = 0;
        if (playerData.hasCyclone) smallRewards[giftname.get_cyclone].weight = 0;
        if (playerData.gotCharm_25) smallRewards[giftname.get_power].weight = 0;
        if (playerData.gotCharm_19) smallRewards[giftname.get_saman].weight = 0;
        if (playerData.gotCharm_14 && playerData.gotCharm_32) smallRewards[giftname.get_quick_and_wenti].weight = 0;
        if (playerData.gotCharm_13) smallRewards[giftname.get_jiaoao].weight = 0;
        if (playerData.gotCharm_33 && playerData.gotCharm_20) smallRewards[giftname.get_faniu_and_bushou].weight = 0;
        if (playerData.gotCharm_21) smallRewards[giftname.get_shihun].weight = 0;
        if (playerData.gotCharm_5 && playerData.gotCharm_4 && playerData.gotCharm_40) smallRewards[giftname.get_badeer_yingke_wuyou].weight = 0;
        if (playerData.gotCharm_3 && playerData.gotCharm_22 && playerData.gotCharm_39) smallRewards[giftname.get_zigong_bianzhizhe_chongge].weight = 0;
        if (playerData.gotCharm_11) smallRewards[giftname.get_xichong].weight = 0;
        if (playerData.gotCharm_7 && playerData.gotCharm_28 && playerData.gotCharm_34) smallRewards[giftname.get_kuaiju_shenju_wuen].weight = 0;




        if (playerData.charmSlots >= 11)
        {
            shopRewards[giftname.shop_add_1_notch_1].weight = 0;
            shopRewards[giftname.shop_add_1_notch_2].weight = 0;
            shopRewards[giftname.shop_add_1_notch_3].weight = 0;
            shopRewards[giftname.shop_add_1_notch_4].weight = 0;
        }
        if (playerData.nailDamage >= 21)
        {
            shopRewards[giftname.shop_nail_upgrade].weight = 0;
        }
        if (playerData.hasSuperDash) shopRewards[giftname.shop_super_dash].weight = 0;
        if (playerData.hasWalljump) shopRewards[giftname.shop_wall_jump].weight = 0;
        if (playerData.hasDreamNail) shopRewards[giftname.shop_dream_nail].weight = 0;
        if (playerData.maxHealthBase >= 9) shopRewards[giftname.shop_add_1_mask].weight = 0;
        if (playerData.MPReserveMax >= 99) shopRewards[giftname.shop_add_1_vessel].weight = 0;
        if (playerData.hasAcidArmour) shopRewards[giftname.shop_acid_swim].weight = 0;
        if (playerData.hasDoubleJump) shopRewards[giftname.shop_double_jump].weight = 0;
        if (playerData.hasShadowDash) shopRewards[giftname.shop_dash].weight = 0;


        for (int i = 1; i <= 40; i++)
        {
            if (i == 36) continue;
            if (playerData.GetBool("gotCharm_" + i))
                charmRewards[(giftname)i].weight = 0;
        }
    }
    public void ResetWeight()
    {
        foreach (var item in smallRewards)
        {
            if (item.Value.level == 0) item.Value.weight = 1.1f;
            else if (item.Value.level == 1) item.Value.weight = 1f;
            else if (item.Value.level == 2) item.Value.weight = 0.9f;
            else if (item.Value.level == 3) item.Value.weight = 0.8f;
            else if (item.Value.level == 4) item.Value.weight = 0.7f;
        }
        shopRewards[giftname.shop_keeper_key].weight = 1;
        shopRewards[giftname.shop_add_1_notch_1].weight = 1;
        shopRewards[giftname.shop_add_1_notch_2].weight = 1;
        shopRewards[giftname.shop_add_1_notch_3].weight = 1;
        shopRewards[giftname.shop_add_1_notch_4].weight = 1;
        shopRewards[giftname.shop_nail_upgrade].weight = 1;
        shopRewards[giftname.shop_random_gift].weight = 1;
        shopRewards[giftname.shop_any_charm_1].weight = 2;
        shopRewards[giftname.shop_any_charm_2].weight = 1.5f;
        shopRewards[giftname.shop_any_charm_3].weight = 0.8f;
        shopRewards[giftname.shop_any_charm_4].weight = 0.8f;
        shopRewards[giftname.shop_egg].weight = 3f;
        shopRewards[giftname.shop_super_dash].weight = 0.8f;
        shopRewards[giftname.shop_wall_jump].weight = 0.8f;
        shopRewards[giftname.shop_dream_nail].weight = 0.8f;
        shopRewards[giftname.shop_add_1_mask].weight = 1.5f;
        shopRewards[giftname.shop_add_1_vessel].weight = 1.5f;
        shopRewards[giftname.shop_prettey_key].weight = 0.1f;
        shopRewards[giftname.shop_refresh].weight = 2f;
        shopRewards[giftname.shop_acid_swim].weight = 0.8f;
        shopRewards[giftname.shop_double_jump].weight = 0.4f;
        shopRewards[giftname.shop_dash].weight = 0.4f;

        for (int i = 1; i <= 40; i++)
        {
            if (i == 36) continue;
            charmRewards[(giftname)i].weight = charm_weight_and_price[(giftname)i].Item1;
        }
    }


}
