using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Satchel;
using Satchel.BetterMenus;
using Satchel.Futils;
using Satchel.Futils.Serialiser;
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
        add_4_naildamage,
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
        get_any_weiyi,
        get_an_big_gift,
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
        get_one_die,
        get_xichong,
        get_birthright,
        warrior,
        fashi,
        youxia,
        nail_master,
        shaman,
        hunter,
        uunn,
        johny,
        guarder,
        moth,
        grey_prince,
        grimm,
        tuke,
        defender,
        mantis,
        collector,
        shop_keeper_key,
        shop_add_1_notch_1,
        shop_add_1_notch_2,
        shop_add_1_notch_3,
        shop_add_1_notch_4,
        shop_add_4_nail_damage,
        shop_random_gift,
        shop_any_charm_1,
        shop_any_charm_2,
        shop_any_charm_3,
        shop_any_charm_4,
        shop_relive,
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
        {"物品描述","这是测试的第一个物品"},
        {"物品名称","测试物品"},
        {"物品价格","114514"},
        {"",""},
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
    public GameObject charms;

    public GameObject shiny_item = null;

    public GameObject menu_go = null;

    public GameObject shop_go = null;
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

        public string name = "";

        public string desc = "";

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
            (bank_scene,banker)
        };
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        base.Initialize(preloadedObjects);
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

        // ModHooks.DoAttackHook += DoAttackHook;


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
                if (role == giftname.tuke)
                {
                    int r = UnityEngine.Random.Range(0, 2);
                    if (r == 0)
                    {
                        AddNailDamage();
                        ShowConvo("+1骨钉");
                    }
                    else
                    {
                        GiveMask();
                        GiveMask();
                        ShowConvo("+2面具");
                    }
                }
                if (role == giftname.johny)
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
        Log("begin!!!");
        orig(sequence, bindings, playerData);
    }

    private void EndScene(On.BossSequenceController.orig_FinishLastBossScene orig, BossSceneController self)
    {

        if (inRogue) inRogue = false;
        Log("Scene OVER");
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
                        Log("2 mask");
                    },
            desc = "+2面具",
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
            desc = "+1魂槽",
            weight = 1.1f,
        });
        smallRewards.Add(giftname.add_4_naildamage, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                AddNailDamage();
            },
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
            desc = "+3护符槽",
            weight = 1.1f,
        });
        smallRewards.Add(giftname.get_fireball, new gift()
        {
            level = 2,
            reward = (giftname) =>
            {
                Log("spell");
                PlayerData.instance.hasSpell = true;
                if (PlayerData.instance.fireballLevel < 2)
                    PlayerData.instance.fireballLevel++;
            },
            desc = "获得波法术",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_scream, new gift()
        {
            level = 2,
            reward = (giftname) =>
        {
            Log("scream");
            PlayerData.instance.hasSpell = true;
            if (PlayerData.instance.screamLevel < 2)
                PlayerData.instance.screamLevel++;
        },
            desc = "获得吼法术",
            weight = 0.9f,
        });
        smallRewards.Add(giftname.get_quake, new gift()
        {
            level = 3,
            reward = (giftname) =>
                    {
                        Log("quake");
                        PlayerData.instance.hasSpell = true;
                        if (PlayerData.instance.quakeLevel < 2)
                            PlayerData.instance.quakeLevel++;
                    },
            desc = "获得砸法术",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_dash, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            Log("chong");
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
            desc = "获得冲刺",
            weight = 0.8f,
        });
        smallRewards.Add(giftname.get_double_jump, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            Log("Double jump");
            PlayerData.instance.hasDoubleJump = true;
        },
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
            if (PlayerData.instance.nailDamage < 21) gifts.Add(smallRewards[giftname.add_4_naildamage]);
            if (PlayerData.instance.hasDash && (!PlayerData.instance.hasShadowDash)) gifts.Add(smallRewards[giftname.get_dash]);
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
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
            desc = "获得任一法术",
            weight = 0f,
            active = false,
        });
        smallRewards.Add(giftname.get_any_weiyi, new gift()
        {
            level = 3,
            reward = (giftname) =>
        {
            List<gift> gifts = new() { smallRewards[giftname.get_dash], smallRewards[giftname.get_zhua_lei_ding], smallRewards[giftname.get_double_jump] };
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
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
            desc = "获得任一剑技",
            weight = 0f,
            active = false
        });
        smallRewards.Add(giftname.get_an_big_gift, new gift()
        {
            level = 4,
            reward = (giftname) =>
            {
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.one_big_gift });
                smallRewards[giftname.get_an_big_gift].weight = 0;
            },
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
            desc = "自选任意两个护符",
            weight = 0.7f,
        });
        smallRewards.Add(giftname.get_one_die, new gift()
        {
            level = 1,
            reward = (giftname) =>
            {
                relive_num++;
            },
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
                        ShowConvo("获得0槽骨钉大师的荣耀");
                        break;
                    case giftname.shaman:
                        if (PlayerData.instance.quakeLevel < 2)
                        {
                            PlayerData.instance.quakeLevel++;
                        }
                        ShowConvo("获得砸法术");
                        break;
                    case giftname.hunter:
                        PlayerData.instance.charmSlots += Math.Min(3, 11 - PlayerData.instance.charmSlots);
                        ShowConvo("+3护符槽");
                        break;
                    case giftname.moth:
                        getAnyCharm++;
                        ShowConvo("自选护符+1");
                        break;
                    case giftname.grey_prince:
                        PlayerData.instance.gotCharm_24 = true;
                        ShowConvo("获得坚固贪婪");
                        break;
                    case giftname.tuke:
                        relive_num += 2;
                        ShowConvo("腐臭蛋+2");
                        break;
                    case giftname.defender:
                        PlayerData.instance.hasAcidArmour = true;
                        ShowConvo("获得伊斯玛的眼泪");
                        break;
                    case giftname.johny:
                        refresh_num++;
                        ShowConvo("刷新次数+1");
                        break;
                    case giftname.test:
                        // PlayerData.instance.xunFlowerGiven = true;
                        ShowConvo("能力变得更强");
                        break;
                    case giftname.mantis:
                        if (PlayerData.instance.hasDash)
                        {
                            PlayerData.instance.hasShadowDash = true;
                            PlayerData.instance.canShadowDash = true;
                            ShowConvo("获得暗影冲刺");
                        }
                        else
                        {
                            PlayerData.instance.hasDash = true;
                            PlayerData.instance.canDash = true;
                            ShowConvo("获得冲刺");
                        }
                        break;
                    case giftname.collector:
                        ShowConvo("召唤物变得更强");
                        break;
                    default:
                        break;

                }
            },
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
            desc = "面具+2&骨钉+1",
            weight = 1f,
        });
        bigRewards.Add(giftname.fashi, new gift()
        {
            level = 5,
            reward = (giftname) =>
            {
                GiveVessel();
                List<gift> gifts2 = new() { smallRewards[giftname.get_scream], smallRewards[giftname.get_fireball], smallRewards[giftname.get_quake] };
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_gift, gifts = gifts2 });
            },
            desc = "魂槽+1&法术+1",
            weight = 1f,
        });
        bigRewards.Add(giftname.youxia, new gift()
        {
            level = 5,
            reward = (giftname) =>
            {
                List<gift> gifts1 = new() { smallRewards[giftname.get_upward_slash], smallRewards[giftname.get_cyclone], smallRewards[giftname.get_dash_slash] };
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_small_gift, select = 1, gifts = gifts1 });
                List<gift> gifts2 = new() { smallRewards[giftname.get_dash], smallRewards[giftname.get_zhua_lei_ding], smallRewards[giftname.get_double_jump] };
                itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_small_gift, select = 1, gifts = gifts2 });
            },
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
                ShowDreamConvao("你准备好去点一份炒饭了");
            },
            name = "作者小号",
            desc = "·选择随机小天赋时可以额外选择一个\n\n\n——容器的能力真的是有极限的啊，在我短暂的虫生里学到，越是玩弄阴谋，就越会在意料之外的事态失足，要成为超越容器的存在啊\n——什么意思，你在说什么？\n——前辈，我不做容器啦！",
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
                ShowDreamConvao("你的骨钉闪闪发亮");
            },
            name = "骨钉大师",
            desc = "·+2级骨钉\n·全剑技\n\n\n骨钉大师虽然技艺高超，可从不显山露水，即使你说他骨钉挥得像棒槌，他也只是会笑笑。\n\n不过如果你想试试他的技艺，或许可以试试少给几块吉欧。",
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
                ShowDreamConvao("你的灵魂肆意膨胀");
            },
            name = "萨满",
            desc = "·获得萨满之石\n·获得暗影之魂\n·+2刷新天赋机会\n·1.25倍法术倍率\n·0.8倍骨钉倍率\n\n\n蜗牛萨满从来都是一个群体，而非对某一只虫的特定称呼。\n\n不过不知道为什么，最近直接叫他萨满的倒是越来越多了。",
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
                ShowDreamConvao("你的脚步轻快异常");
            },
            name = "猎人",
            desc = "·获得冲刺、二段跳、螳螂爪、水晶之心\n·获得快速劈砍、稳定之体\n\n\n潜伏，杀戮，抽丝剥茧\n\n猎人的世界里，不是猎杀就是被猎杀",
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
        shopRewards.Add(giftname.johny, new gift()
        {
            //护符槽血魂全满
            //绑定乔尼的祝福
            //解锁全部护符
            //1.75倍骨钉倍率
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.johny;
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
                ShowDreamConvao("你的心中充满慈悲");
            },
            name = "乔尼",
            desc = "·获得全部面具、魂槽、护符槽\n·获得全部护符，绑定乔尼的祝福\n·1.75倍骨钉倍率\n\n\n虽然是异教徒，但蓝色之子乔尼却很少与他人产生矛盾，仁慈的她会真诚地祝福每一位路过的漫游者。",
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
                ShowDreamConvao("你的决意无以复加");
            },
            name = "先知",
            desc = "·获得梦之钉\n·+1自选护符\n·+1骨钉、+1面具、+1魂槽\n·获得满护符槽\n\n\n照料墓碑，引导挥舞者，为了忏悔昔日的罪行，先知日复一日的做着，从无怨言。\n\n只是有时，她也会梦见那道被遗忘的光，",
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
                ShowDreamConvao("你的形象不断高大");
            },
            name = "灰色王子",
            desc = "·骨钉固定为0钉\n·获得复仇之魂、荒芜俯冲、嚎叫幽灵\n·获得暗影披风\n·获得梦之钉\n·所有护符不消耗护符槽\n\n\n现在是\n\n幻想时间！！！",
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Grey_Prince_Zote_Idle.png"),
            scale = new Vector2(0.45f, 0.45f)
        });
        shopRewards.Add(giftname.tuke, new gift()
        {
            //5个腐臭蛋
            //500geo
            //每次使用腐臭蛋后+1骨钉或+2面具
            //初始只有三血
            level = 5,
            reward = (giftname) =>
            {
                role = giftname.tuke;
                while (PlayerData.instance.maxHealthBase > 3) TakeAwayMask();
                HeroController.instance.AddGeo(500);
                relive_num = 5;
                ShowDreamConvao("你的荷包鼓鼓当当");
            },
            name = "图克",
            desc = "·初始只有三血\n·初始具有5个腐臭蛋、500吉欧\n·每次使用腐臭蛋后+1骨钉或+2面具\n\n\n皇家水道的寻荒者，脆弱但坚强\n\n食物、朋友，水流带来了所有想要的东西……图克只要把它们找出来就行了",
            active = false,
            weight = 0,
            sprite = AssemblyUtils.GetSpriteFromResources("Tuk.png"),
            scale = new Vector2(0.45f, 0.45f)
        });
        shopRewards.Add(giftname.defender, new gift()
        {
            //绑定英勇气息
            //1.25倍骨钉倍率todo
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
                ShowDreamConvao("你的锋芒无法隐藏");
            },
            name = "螳螂",
            desc = "·获得螳螂爪\n·绑定骄傲印记\n·1.25倍骨钉倍率\n\n\n他们依然身怀睿智和荣耀，当然也保存着致命的传统。实力是他们的底气，骄傲是他们的印记。\n\n寻求死亡的漫游者，祝你在爪下痛快地咽气。",
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
                ShowDreamConvao("你的爱意交织愉悦");
            },
            name = "收藏家",
            desc = "·0.7倍骨钉倍率\n·0.7倍法术倍率（吸虫法术不受影响）\n·获得格林之子\n·你的召唤物会因你而得到强化\n·除格林之子外，每个召唤护符都会使本体伤害倍率降低\n·召唤护符不消耗护符槽\n\n接近是爱，保护也是爱，伪装是爱，囚禁也是爱。\n\n没关系……不明白,也没关系……",
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
            name = "店主的钥匙",
            desc = "刷新一次商店\n\n\n斯莱的储物室的钥匙，可以凭此找店主更换一批货物，当然，如果你换到了并不需要的东西，最终解释权归店主所有。",
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
            name = "护符槽",
            desc = "+1护符槽\n\n\n",
            active = false,
            weight = 1,
            sprite = AssemblyUtils.GetSpriteFromResources("Charm_Notch.png")
        });
        shopRewards.Add(giftname.shop_add_4_nail_damage, new gift()
        {
            price = 500,
            reward = (giftname) =>
            {
                AddNailDamage();
            },
            name = "苍白矿石",
            desc = "+1级骨钉\n\n\n更锋利就更好吗？\n更纯粹就更好吗？\n曾有虫思考过这个问题，可惜它也没得到什么答案。",
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
            name = "随机一小天赋",
            desc = "\"生活就像一袋巧克力，你永远不知道下一颗是什么味道\"",
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
            name = "护符槽",
            desc = "+1护符槽",
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
            name = "护符槽",
            desc = "+1护符槽",
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
            name = "护符槽",
            desc = "+1护符槽",
            weight = 1,
            getSprite = (giftname) =>
            {
                return shopRewards[giftname.shop_add_1_notch_1].sprite;
            }
        });
        shopRewards.Add(giftname.shop_relive, new gift()
        {
            price = 400,
            reward = (giftname) =>
            {
                relive_num++;
            },
            name = "腐臭蛋",
            desc = "复活次数+1\n\n\n蛋神伟大，无需多言。",
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
            name = "水晶之心",
            desc = "获得超冲\n\n\n一个古老挖矿魔像的能量核心，以一块强力的水晶塑成。",
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
            name = "螳螂爪",
            desc = "获得螳螂爪\n\n\n用骨头雕刻的爪子。",
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
            name = "梦之钉",
            desc = "获得梦之钉\n\n\n如果你有这样的武器，你会想去读一读其他人的心吗？",
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
            name = "面具",
            desc = "+1面具\n\n\n",
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
            name = "魂槽",
            desc = "+1魂槽\n\n\n",
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
                    "哇哦，你发现了彩蛋",
                    "84 72 65 78 75 83",
                    "这是我们第三次见面了呢",
                    "70 79 82",
                    "还不死心吗，你的钱好像不够了呢",
                    "80 76 65 89 73 78 71",
                    "告诉你个秘密，后面已经没有东西了"
                };
                if (pretty_key_num == dreams.Count)
                {
                    ShowDreamConvao(dreams[pretty_key_num - 1]);
                    _set.owner = true;
                }
                else if (pretty_key_num < dreams.Count)
                {
                    ShowDreamConvao(dreams[pretty_key_num]);
                    pretty_key_num++;
                    if (pretty_key_num == dreams.Count)
                        _set.owner = true;
                }


            },
            name = "典雅钥匙",
            desc = "由闪亮的白色金属制成的华丽钥匙。刻有特殊的标记，在黑暗中发出8geo的光芒。",
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
            name = "简单钥匙",
            desc = "+1刷新次数\n\n\n仅仅是一把简单的钥匙，有时却能帮你打开通往未来之门。",
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
            name = "伊思玛的眼泪",
            desc = "获得伊思玛的眼泪\n\n\n一滴硬化的眼泪形成的果实。",
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
            name = "帝王之翼",
            desc = "获得帝王之翼\n\n\n在黑暗中闪烁的虚无翅膀。",
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
            name = "冲刺",
            desc = "获得或升级冲刺能力\n\n\n",
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
            charms.FindGameObjectInChildren("Equipped Charms").FindGameObjectInChildren("Notches").FindGameObjectInChildren("Text Notches").GetComponent<TMPro.TextMeshPro>().text = "可自选" + getAnyCharm + "个";
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
                        charms.FindGameObjectInChildren("Equipped Charms").FindGameObjectInChildren("Notches").FindGameObjectInChildren("Text Notches").GetComponent<TMPro.TextMeshPro>().text = "可自选" + getAnyCharm + "个";
                    }
                }, 1);
            }
            if (self.GetState("Black Charm?").Actions.Length == 2)
            {
                self.InsertCustomAction("Black Charm?", (fsm) =>
                {
                    if (role == giftname.johny)
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
                }, 0);
            }
            if (self.GetState("Black Charm? 2").Actions.Length == 2)
            {
                self.InsertCustomAction("Black Charm? 2", (fsm) =>
                                {
                                    if (role == giftname.johny)
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
        var h = new Satchel.BetterMenus.MenuButton("Reset", "重置至无能力状态", (but) =>
        {
            Rogue_Reset();
        },
        proceed: true);
        var test = new MenuButton("给予奖励", "", (but) =>
        {
            if (itemManager != null)
                itemManager.GiveReward(3);
        });
        var getcharm = new MenuButton("获取护符", "", (but) =>
        {
            for (int i = 1; i < 42; i++)
            {
                Log(i + "   " + Language.Language.Get("CHARM_NAME_" + i, "UI"));
            }
        });
        // menu.AddElement(h);
        // menu.AddElement(test);
        // menu.AddElement(getcharm);
        var b = new Satchel.BetterMenus.KeyBind("刷新", self_actions.refresh);
        var c = new Satchel.BetterMenus.KeyBind("结束", self_actions.over);
        var d = new Satchel.BetterMenus.KeyBind("开始", self_actions.start);
        menu.AddElement(b);
        menu.AddElement(d);
        menu.AddElement(c);
        var text = new Satchel.BetterMenus.TextPanel("本mod为空洞肉鸽mod(丝之肉鸽？)，为国庆期间举办的水晶杯制作，当前为V" + GetVersion() + "正式版。", width: 1500);

        Menu giftBase = new("天赋库");
        int count = 0;
        List<List<Element>> list = new();
        List<Element> tempList = null;
        giftBase.AddElement(new TextPanel("小天赋"));

        foreach (var gift in smallRewards)
        {
            gift g = gift.Value;
            if (g.desc == null) continue;
            var temp = Blueprints.ToggleButton(g.desc, "",
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
        giftBase.AddElement(new TextPanel("大天赋"));
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
        menu.AddElement(new MenuButton("天赋库", "", (but) =>
        {
            Utils.GoToMenuScreen(gs);
        }));
        menu.AddElement(text);
        Log("456");
        var ms = menu.GetMenuScreen(modListMenu);
        giftBase.returnScreen = ms;
        Log("789");
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

    public void ShowDreamConvao(string msg)
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
        if (playerData.nailDamage >= 21) smallRewards[giftname.add_4_naildamage].weight = 0;
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
            shopRewards[giftname.shop_add_4_nail_damage].weight = 0;
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
        shopRewards[giftname.shop_add_4_nail_damage].weight = 1;
        shopRewards[giftname.shop_random_gift].weight = 1;
        shopRewards[giftname.shop_any_charm_1].weight = 2;
        shopRewards[giftname.shop_any_charm_2].weight = 1.5f;
        shopRewards[giftname.shop_any_charm_3].weight = 0.8f;
        shopRewards[giftname.shop_any_charm_4].weight = 0.8f;
        shopRewards[giftname.shop_relive].weight = 3f;
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