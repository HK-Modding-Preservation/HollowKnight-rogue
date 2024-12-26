using UnityEngine.UI;
using rogue.Characters;
using System.ComponentModel;
namespace rogue;

public class ItemManager : MonoBehaviour
{
    internal static ItemManager Instance { get; private set; }
    public ItemManager()
    {
        Instance = this;
    }
    public GameObject item;
    public GameObject knight;

    public List<GameObject> items = new();

    public GameObject rogue_display = null;

    public Text text = null;

    public TMPro.TextMeshPro refresh_text_pro = null;

    public TMPro.TextMeshPro time_display_text_pro = null;

    public string scenename;

    private float refreshgap = 1f;
    private float startgap = 3f;

    private float overgap = 3f;

    public TimeSpan timeSpan = TimeSpan.Zero;




    public enum Mode
    {
        one_big_gift,
        one_small_gift,
        select_big_gift,
        select_small_gift,
        fix_gift,
        fix_select_big_gift,
        fix_select_small_gift

    }

    public Mode nowMode;

    public int tempselect;

    Sprite sprite = null;

    public class OneReward
    {
        public Mode mode;
        public int give;

        public int select;

        public List<Gift> gifts;
    }

    internal Stack<OneReward> rewardsStack = new();

    public Dictionary<string, bool> now_scene_items = new Dictionary<string, bool>();

    public GameObject shop_item_temp = null;

    public int damaged_num = 0;

    public TMPro.TMP_FontAsset all_font = null;

    public float alpha = 0.6f;

    public GameObject inventory_display_holder = null;

    public GameObject item_menu_go = null;
    public GameObject item_shop_go = null;

    public GameObject item_restbench = null;

    public GameObject item_fly_go = null;
    public Action after_revive_action = null;

    internal List<Giftname> roleList = new List<Giftname>
        {
            Giftname.role_nail_master,
            Giftname.role_shaman,
            Giftname.role_hunter,
            // Giftname.uunn,
            Giftname.role_joni,
            Giftname.role_moth,
            Giftname.role_grey_prince,
            Giftname.role_tuk,
            // Giftname.defender,
            Giftname.role_mantis,
            Giftname.role_collector
        };
    List<string> nobossscene = new List<string> { "GG_Spa", "GG_Engine", "GG_Unn", "GG_Engine_Root", "GG_Wyrm", "GG_Atrium_Roof" };

    public string rouge_introduction = "rogue_introduction".Localize();

    public bool hatchscene = false;

    public delegate int OnChangeSceneAndAddGeo(int geo, int damage_num);
    public OnChangeSceneAndAddGeo after_scene_add_geo_num;
    Action<PlayMakerFSM> fsm_enable = null;
    public Func<OneReward, OneReward> before_spawn_item = null;

    System.Random item_random;





    public void Start()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        Modding.ModHooks.LanguageGetHook += OnLanguageGet;
        On.PlayerData.GetBool += OnGetBool;
        On.PlayerData.SetBool += OnSetBool;
        ModHooks.CharmUpdateHook += OnCharmUpdate;
        On.PlayMakerFSM.OnEnable += OnFSMEnable;
        ModHooks.TakeHealthHook += Revive;
        item_random = new();
    }
    public void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        ModHooks.LanguageGetHook -= OnLanguageGet;
        On.PlayerData.GetBool -= OnGetBool;
        On.PlayerData.SetBool -= OnSetBool;
        ModHooks.CharmUpdateHook -= OnCharmUpdate;
        On.PlayMakerFSM.OnEnable -= OnFSMEnable;
        ModHooks.TakeHealthHook -= Revive;

    }

    private int Revive(int damage)
    {
        if (GameInfo.revive_num > 0 && GameInfo.in_rogue && BossSequenceController.IsInSequence)
        {
            if (((PlayerData.instance.health + PlayerData.instance.healthBlue) <= damage) || (PlayerData.instance.equippedCharm_27 && (PlayerData.instance.healthBlue <= damage)))
            {
                GameInfo.revive_num--;
                after_revive_action?.Invoke();
                GiftFactory.UpdateWeight();
                DisplayStates();
                return 0;
            }
        }
        return damage;

    }

    private void OnCharmUpdate(PlayerData data, HeroController controller)
    {
        DisplayEquipped();

    }

    private void OnSetBool(On.PlayerData.orig_SetBool orig, PlayerData self, string boolName, bool value)
    {
        if (now_scene_items.ContainsKey(boolName))
        {
            now_scene_items[boolName] = value;
            return;
        }
        else orig(self, boolName, value);
    }

    private bool OnGetBool(On.PlayerData.orig_GetBool orig, PlayerData self, string boolName)
    {

        if (boolName == "blueRoomActivated") return true;
        else if (boolName == "blueRoomDoorUnlocked") return true;
        if (now_scene_items.ContainsKey(boolName))
        {
            return now_scene_items[boolName];
        }
        else
        {
            return orig(self, boolName);
        }
    }

    private string OnLanguageGet(string key, string sheetTitle, string orig)
    {
        if (key == null) return "key is null";
        if (sheetTitle == "Enemy Dreams" && key.StartsWith("rogue_"))
        {
            return key.Split('_')[1];
        }
        if (sheetTitle == "rouge" && key == "rouge_introduction") return rouge_introduction;
        if (sheetTitle == "Titles" && key == "rouge_introduction_MAIN") return "Gou_bro_MAIN".Localize();
        if (sheetTitle == "Titles" && key == "rouge_introduction_SUB") return "Gou_bro_SUB".Localize();
        if (sheetTitle == "Titles" && key == "rouge_introduction_SUPER") return "Gou_bro_SUPER".Localize();
        else return orig;
    }

    public void BeginDisplay()
    {
        if (rogue_display != null)
        {
            DestroyImmediate(rogue_display);
            refresh_text_pro = null;
            time_display_text_pro = null;
        }

        rogue_display = new("rogue_display");
        DontDestroyOnLoad(rogue_display);
        GameObject refresh = new("refresh");
        refresh.transform.SetParent(rogue_display.transform);
        refresh.layer = LayerMask.NameToLayer("UI");
        refresh.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        refresh.transform.localPosition = new Vector3(-14.6f, 2, 0);
        var render = refresh.AddComponent<SpriteRenderer>();
        render.sprite = AssemblyUtils.GetSpriteFromResources("刷新.png");
        render.color = new Color(1, 1, 1, alpha);
        GameObject refresh_num_text = new("refresh_num_text");
        refresh_num_text.transform.SetParent(refresh.transform);
        refresh_num_text.layer = LayerMask.NameToLayer("UI");
        refresh_num_text.transform.localScale = new Vector3(1, 1, 1);
        refresh_num_text.transform.localPosition = new Vector3(12.5f, -1.7f, 0);
        refresh_text_pro = refresh_num_text.AddComponent<TMPro.TextMeshPro>();
        refresh_text_pro.fontSize = 24;
        refresh_text_pro.color = new Color(1, 1, 1, alpha);
        try
        {
            if (all_font == null) all_font = HutongGames.PlayMaker.FsmVariables.GlobalVariables.FindFsmGameObject("Inventory").Value.FindGameObjectInChildren("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Rancid Egg").FindGameObjectInChildren("Egg Amount").GetComponent<TMPro.TextMeshPro>().font;
            refresh_text_pro.font = all_font;
        }
        catch (Exception ex)
        {
            Log("字体尚未找到");
        }

        GameObject time_display = new("time_display");
        time_display.transform.SetParent(rogue_display.transform);
        time_display.layer = LayerMask.NameToLayer("UI");
        time_display.transform.localPosition = new Vector3(-5.1f, -0.6f, 0);
        time_display_text_pro = time_display.AddComponent<TMPro.TextMeshPro>();
        time_display_text_pro.fontSize = 6;
        time_display_text_pro.color = new Color(1, 1, 1, alpha);
        if (all_font != null) time_display_text_pro.font = all_font;

    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        scenename = arg1.name;
        hatchscene = false;
        DisplayEquipped();
        DisplayStates();
        if (GameInfo.in_rogue && BossSequenceController.IsInSequence)
        {
            if (!nobossscene.Contains(arg0.name))
            {
                int geo = 50;
                if (PlayerData.instance.equippedCharm_24) geo += 20;
                if (after_scene_add_geo_num != null) geo = after_scene_add_geo_num(geo, damaged_num);
                HeroController.instance.AddGeo(geo);
            }
        }
        damaged_num = 0;
        if (arg1.name == "GG_Engine")
        {
            StartCoroutine(DelayShowDreamConvo(0.5f, "godseeker".Localize()));
            item_fly_go.SetActive(false);
        }
        else if (arg1.name == "GG_Atrium_Roof")
        {
            GameObject taijie = Instantiate(GameObject.Find("gg_plat_float_small"));
            taijie.transform.SetPosition2D(new Vector2(118, 64));
            taijie.SetActive(true);
            item_fly_go.SetActive(false);
            if (GameInfo.in_rogue)
            {
                StartShopItem();
                item_menu_go.SetActive(true);
                item_shop_go.transform.SetPosition2D(new Vector2(129.9f, 63));
                item_shop_go.LocateMyFSM("Shop Region").FsmVariables.GetFsmFloat("Move To X").Value = 132f;
                item_shop_go.SetActive(true);
                item_restbench.transform.SetPosition3D(128.8f, 61.7f, 0.1f);
                item_restbench.SetActive(true);
                var gou = item_shop_go.FindGameObjectInChildren("gou_Bro");
                gou.SetActive(true);
                gou.GetComponent<tk2dSpriteAnimator>().Play("Sleep");
                gou.GetComponent<tk2dSprite>().FlipX = true;
                var bank = item_shop_go.FindGameObjectInChildren("bank");
                bank.SetActive(false);
                var banker = item_shop_go.FindGameObjectInChildren("banker");
                banker.SetActive(false);
            }
        }
        else if (arg1.name == "GG_Spa")
        {
            item_restbench.SetActive(false);
            if (GameInfo.in_rogue)
            {
                // item_fly_go.SetActive(true);
                item_fly_go.transform.position = new Vector3(85, 20, 0);
                GameInfo.spa_count++;
                GiftFactory.UpdateWeight();
                DestroyAllItems();
                GiveReward(GameInfo.spa_count);
                if (GameInfo.spa_count % 2 == 0 || GameInfo.spa_count == 7)
                {
                    NormalShopItem();
                    item_menu_go.SetActive(true);
                    item_shop_go.transform.SetPosition2D(new Vector2(78, 16));
                    item_shop_go.LocateMyFSM("Shop Region").FsmVariables.GetFsmFloat("Move To X").Value = 78f;
                    item_shop_go.SetActive(true);
                    var gou = item_shop_go.FindGameObjectInChildren("gou_Bro");
                    gou.SetActive(false);
                    var bank = item_shop_go.FindGameObjectInChildren("bank");
                    bank.SetActive(true);
                    bank.GetComponent<tk2dSpriteAnimator>().Play("Stand Idle");
                    var banker = item_shop_go.FindGameObjectInChildren("banker");
                    banker.SetActive(true);
                    banker.GetComponent<tk2dSpriteAnimator>().Play("Idle");
                }
            }
        }
        else
        {
            item_menu_go.SetActive(false);
            item_shop_go.SetActive(false);
            item_restbench.SetActive(false);
            item_fly_go.SetActive(false);
        }
    }
    public void SpwanShopItem(List<Gift> gifts, bool select = false)
    {
        if (shop_item_temp == null)
        {
            shop_item_temp = Instantiate(item_menu_go.GetComponent<ShopMenuStock>().stock[0]);
            shop_item_temp.name = "shop_item_temp";
            var shopitem = shop_item_temp.GetComponent<ShopItemStats>();
            shopitem.specialType = 0;
            shopitem.charmsRequired = 0;
            shopitem.nameConvo = "物品名称";
            shopitem.descConvo = "物品描述";
            shopitem.priceConvo = "0";
            shop_item_temp.SetActive(false);
            DontDestroyOnLoad(shop_item_temp);

        }
        List<GameObject> shop_items = new();
        now_scene_items.Clear();
        int count = 0;
        string selectname = "rogue_select";
        List<Giftname> charms = new(){
                Giftname.shop_any_charm_1,
                Giftname.shop_any_charm_2,
                Giftname.shop_any_charm_3,
                Giftname.shop_any_charm_4
            };
        foreach (var gift in gifts)
        {
            GameObject good = Instantiate(shop_item_temp);
            var goodstats = good.GetComponent<ShopItemStats>();
            var render = good.FindGameObjectInChildren("Item Sprite").GetComponent<SpriteRenderer>();
            if (charms.Contains(gift.giftname))
            {
                var list = RandomList(GameInfo.act_gifts[GiftVariety.charm], 1, false);
                if (list.Count <= 0)
                {
                    Destroy(good);
                    continue;
                }
                Gift charmgift = list[0];
                good.name = charmgift.GetName();
                goodstats.nameConvo = charmgift.GetName();
                goodstats.descConvo = charmgift.GetDesc();
                goodstats.priceConvo = charmgift.price.ToString();
                goodstats.specialType = (int)charmgift.giftname + 18;
                goodstats.cost = charmgift.price;
                render.sprite = charmgift.GetSprite();
                var item = good.FindGameObjectInChildren("Item Sprite");
                if (charmgift.scale != Vector2.zero) item.transform.localScale = charmgift.scale;
                if (select) goodstats.playerDataBoolName = selectname;
                else
                    goodstats.playerDataBoolName = charmgift.giftname.ToString() + count;
                count++;

            }
            else
            {
                good.name = gift.GetName();
                goodstats.nameConvo = gift.GetName();
                goodstats.descConvo = gift.GetDesc();
                goodstats.priceConvo = gift.price.ToString();
                goodstats.specialType = (int)gift.giftname + 18;
                goodstats.cost = gift.price;
                render.sprite = gift.GetSprite();
                var item = good.FindGameObjectInChildren("Item Sprite");
                if (gift.scale != Vector2.zero) item.transform.localScale = gift.scale;
                if (select) goodstats.playerDataBoolName = selectname;
                else
                    goodstats.playerDataBoolName = gift.giftname.ToString() + count;
                count++;
            }

            //good.FindGameObjectInChildren("Item Sprite").GetComponent<SpriteRenderer>().sprite
            shop_items.Add(good);
            if (select)
            {
                if (!now_scene_items.ContainsKey(selectname))
                    now_scene_items.Add(selectname, false);
            }
            else
            {
                now_scene_items.Add(goodstats.playerDataBoolName, false);
            }
        }


        var itemlist = item_menu_go.FindGameObjectInChildren("Item List");
        var menustock = itemlist.GetComponent<ShopMenuStock>();
        var master_menu_stock = item_menu_go.GetComponent<ShopMenuStock>();

        master_menu_stock.stock = shop_items.ToArray();
        menustock.stock = shop_items.ToArray();
        Rogue.Instance.shop_items.Clear();
        foreach (var shop_item in shop_items)
        {
            Rogue.Instance.shop_items.Add(shop_item, shop_item);
        }
        ReflectionHelper.SetField(item_menu_go.GetComponent<ShopMenuStock>(), "spawnedStock", Rogue.Instance.shop_items);
        ReflectionHelper.SetField(menustock, "spawnedStock", Rogue.Instance.shop_items);
        // var menu = item_menu_go.FindGameObjectInChildren("Item List").GetComponent<ShopMenuStock>();
        // menu.itemCount++;
        // testinv.transform.SetParent(menu.gameObject.transform, worldPositionStays: false);
        // testinv.transform.localPosition = new Vector3(0f, (menu.itemCount - 1) * yDistance, 0f);
        // testinv.GetComponent<ShopItemStats>().itemNumber = menu.itemCount;
        // menu.stockInv[menu.itemCount] = testinv;
        // testinv.SetActive(value: true);

    }
    public void StartShopItem()
    {
        List<Gift> roles = new();
        foreach (var role in roleList)
        {
            roles.Add(GiftFactory.all_gifts[role]);
        }
        if (Rogue.Instance._set.owner) roles = roles.Append(GiftFactory.all_gifts[Giftname.role_test]).ToList();
        SpwanShopItem(roles, select: true);

    }
    public void NormalShopItem()
    {
        List<Gift> items = new List<Gift>();
        items.Add(GiftFactory.all_gifts[Giftname.shop_keeper_key]);
        if (GiftFactory.all_gifts[Giftname.shop_add_1_notch_1].weight > 0)
            items.Add(GiftFactory.all_gifts[Giftname.shop_add_1_notch_1]);
        if (GiftFactory.all_gifts[Giftname.shop_nail_upgrade].weight > 0)
            items.Add(GiftFactory.all_gifts[Giftname.shop_nail_upgrade]);
        items.Add(GiftFactory.all_gifts[Giftname.shop_random_gift]);
        var ranlist = RandomList(GameInfo.act_gifts[GiftVariety.shop], num: 9 - items.Count);
        SpwanShopItem(items.Concat(ranlist).ToList());


    }
    private IEnumerator DelayGenerater(float delay)
    {
        yield return new WaitForSeconds(delay);
        Next(true);
    }
    private IEnumerator DelayShowDreamConvo(float delay, string msg)
    {
        yield return new WaitForSeconds(delay);
        Rogue.Instance.ShowDreamConvo(msg);
    }
    public void DestroyAllItems()
    {
        foreach (GameObject item in items)
        {
            if (item != null) DestroyImmediate(item);
        }
        items.Clear();
    }

    public void GiveReward(int door)
    {
        rewardsStack.Push(new OneReward
        {
            mode = Mode.select_small_gift,
            select = 1,
            give = 3,
            gifts = null
        });
        if (door % 2 == 1)
        {
            rewardsStack.Push(new OneReward
            {
                mode = Mode.fix_select_big_gift,
                select = 1,
                gifts = GameInfo.act_gifts[GiftVariety.huge]
            });
        }
        StartCoroutine(DelayGenerater(1f));
    }


    public void Update()
    {
        refreshgap -= Time.deltaTime;
        overgap -= Time.deltaTime;
        startgap -= Time.deltaTime;
        if (HeroController.instance == null) return;
        if (Rogue.self_actions.refresh.IsPressed)
        {
            if (GameInfo.refresh_num > 0)
            {
                if (refreshgap < 0f)
                {
                    if (items.Count > 0)
                    {
                        refreshgap = 0.75f;
                        int t = items.Count;

                        switch (nowMode)
                        {
                            case Mode.select_small_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomSelectSmall(tempselect, t);
                                break;
                            case Mode.select_big_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomSelectBig(tempselect, t);
                                break;
                            case Mode.one_big_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomOneBig();
                                break;
                            case Mode.one_small_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomOneSmall();
                                break;
                            case Mode.fix_gift:
                                break;
                            case Mode.fix_select_big_gift:
                                break;
                            case Mode.fix_select_small_gift:
                                break;
                            default:
                                break;

                        }
                    }
                }
            }
        }
        if (Rogue.self_actions.start.IsPressed && startgap < 0)
        {
            startgap = 1f;
            Rogue.Instance.Rogue_Reset();
            Rogue.Instance.Rogue_Start();
            DisplayEquipped();
            DisplayStates();
            if (scenename == "GG_Atrium_Roof")
            {
                StartShopItem();
                item_menu_go.SetActive(true);
                item_shop_go.transform.SetPosition2D(new Vector2(129.9f, 63));
                item_shop_go.LocateMyFSM("Shop Region").FsmVariables.GetFsmFloat("Move To X").Value = 132f;
                item_shop_go.SetActive(true);
                item_restbench.transform.SetPosition3D(128.8f, 61.7f, 0.1f);
                item_restbench.SetActive(true);
                var gou = item_shop_go.FindGameObjectInChildren("gou_Bro");
                gou.SetActive(true);
                gou.GetComponent<tk2dSpriteAnimator>().Play("Sleep");
                gou.GetComponent<tk2dSprite>().FlipX = true;
                var bank = item_shop_go.FindGameObjectInChildren("bank");
                bank.SetActive(false);
                var banker = item_shop_go.FindGameObjectInChildren("banker");
                banker.SetActive(false);
            }
            ModHooks.AfterTakeDamageHook += CheckIfdamaged;
        }
        if (Rogue.self_actions.over.IsPressed && overgap < 0)
        {
            overgap = 1f;
            Rogue.Instance.Rogue_Over();
            ModHooks.AfterTakeDamageHook -= CheckIfdamaged;
        }
        if (GameInfo.in_rogue && BossSequenceController.IsInSequence && (scenename != "GG_Spa") && (scenename! != "GG_Atrium_Roof" || scenename != "GG_Engine"))
        {
            if (!GameManager.instance.isPaused)
            {
                GameInfo.timer += Time.unscaledDeltaTime;
                timeSpan = TimeSpan.FromSeconds(GameInfo.timer);
            }
        }

        if (refresh_text_pro != null)
        {
            refresh_text_pro.text = GameInfo.refresh_num.ToString();
        }
        if (time_display_text_pro != null)
        {
            time_display_text_pro.text = TimerText();
        }

    }

    private int CheckIfdamaged(int hazardType, int damageAmount)
    {
        if (damageAmount > 0)
            damaged_num++;
        return damageAmount;
    }


    private void OnFSMEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "FSM" && self.gameObject.name == "blue stuff")
        {
            self.GetAction<IntCompare>("Get Bindings", 1).integer2 = 0;
        }
        else if (self.FsmName == "Conversation Control" && self.gameObject.name == "Godseeker EngineRoom NPC")
        {

            if (self.GetState("End").Actions.Length == 2)
            {
                self.InsertCustomAction("End", () =>
                {
                    if (GameInfo.in_rogue && !GameInfo.get_birthright)
                    {
                        GiftFactory.all_gifts[Giftname.get_birthright].GetGift();
                        GiftFactory.UpdateWeight();
                        DisplayStates();
                    }
                }, 1);
            }
        }
        fsm_enable?.Invoke(self);
        orig(self);

    }

    public List<Gift> RandomList(List<Gift> gifts, int num = 1, bool canRepeat = false, System.Random random = null)
    {
        if (random == null)
        {
            random = this.item_random;
        }
        List<Gift> res = new();
        float whole = gifts.Sum((gift) =>
        {
            return gift.weight;
        });
        if (whole == 0) return res;
        int count = gifts.Count((gift) =>
        {
            return gift.weight > 0;
        });
        if (count < num)
        {
            res = gifts.Where((gift) =>
            {
                return gift.weight > 0;
            }).ToList();
            return res;
        }
        int index = 0;
        while (res.Count < num)
        {
            do
            {
                float now = 0;
                index = 0;
                while (now == 0) now = (float)random.NextDouble() * whole;
                while (now > 0)
                {
                    now -= gifts[index].weight;
                    index += 1;
                }
            }
            while (!canRepeat && res.Contains(gifts[index - 1]));
            res.Add(gifts[index - 1]);
        }
        return res;




    }
    public void RandomOneSmall()
    {
        items.Clear();
        nowMode = Mode.one_small_gift;
        int count = GameInfo.act_gifts[GiftVariety.item].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.item], 1);
        if (list.Count > 0)
            SpwanItem(list[0]);
    }
    public void RandomOneBig()
    {
        items.Clear();
        nowMode = Mode.one_big_gift;
        int count = GameInfo.act_gifts[GiftVariety.huge].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.huge], 1);
        if (list.Count > 0)
            SpwanItem(list[0]);
    }
    public void RandomSelectSmall(int select, int give)
    {
        items.Clear();
        nowMode = Mode.select_small_gift;
        tempselect = select;
        int count = GameInfo.act_gifts[GiftVariety.item].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.item], give);
        foreach (var item in list)
        {
            SpwanItem(item);
        }


    }
    public void RandomSelectBig(int select, int give)
    {
        items.Clear();
        nowMode = Mode.select_big_gift;
        tempselect = select;
        int count = GameInfo.act_gifts[GiftVariety.huge].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.huge], give);
        foreach (var item in list)
        {
            SpwanItem(item);
        }


    }

    public void FixGifts(List<Gift> gifts)
    {
        items.Clear();
        nowMode = Mode.fix_gift;
        foreach (var gift in gifts)
        {
            SpwanItem(gift);
        }
    }

    public void FixSelectBig(List<Gift> bigs, int select)
    {
        items.Clear();
        nowMode = Mode.fix_select_big_gift;
        tempselect = select;
        foreach (var gift in bigs)
        {
            SpwanItem(gift, true);
        }

    }
    public void FixSelectSmall(List<Gift> smalls, int select)
    {
        items.Clear();
        nowMode = Mode.fix_select_big_gift;
        tempselect = select;
        foreach (var gift in smalls)
        {
            SpwanItem(gift, true);
        }

    }
    private void SpwanItem(Gift gift, bool inSelect = false)
    {
        if (gift == null) return;
        SpwanItem(gift.level, gift.GetName(), gift.reward, gift.giftname);
        return;

    }


    private void SpwanItem(int level, string name, Action<Giftname> action, Giftname giftname)
    {
        Log(giftname.ToString() + " spawn");
        knight = GameObject.Find("Knight");
        item = Rogue.Instance.shiny_item;
        if (knight != null)
        {
            var newitem = Instantiate(item);
            if (scenename == "GG_Atrium_Roof")
            {
                newitem.transform.SetPosition2D(110, 63);
            }
            else if (scenename == "GG_Spa")
            {
                newitem.transform.SetPosition2D(85, 16);
            }
            else
            {
                newitem.transform.position = knight.transform.position;
            }

            bool flag = true;
            if (scenename == "GG_Spa")
            {
                var spa_pos = new List<Vector2> { new Vector2(83, 16), new Vector2(85, 16), new Vector2(87, 16) };

                foreach (var try_pos in spa_pos)
                {
                    newitem.transform.position = try_pos;
                    bool spaflag = true;
                    foreach (var item in items)
                    {
                        if (Math.Abs(item.transform.position.x - newitem.transform.position.x) < 0.5f)
                        {
                            spaflag = false;
                            break;
                        }
                    }
                    if (spaflag)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            while (flag)
            {
                var pos = newitem.transform.position;
                pos.x += Random.Range(-2f, 2f);
                newitem.transform.position = pos;
                flag = false;
                foreach (var item in items)
                {
                    if (Math.Abs(item.transform.position.x - newitem.transform.position.x) < 2f)
                    {
                        flag = true;
                        break;
                    }
                }

            }

            switch (level)
            {
                case 0:
                    newitem.GetComponent<SpriteRenderer>().color = Color.white;
                    break;
                case 1:
                    newitem.GetComponent<SpriteRenderer>().color = Color.blue;
                    break;
                case 2:
                    newitem.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0.5f);
                    break;
                case 3:
                    newitem.GetComponent<SpriteRenderer>().color = new Color(1f, 0.843f, 0);
                    break;
                case 4:
                    newitem.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                case 5:
                    newitem.GetComponent<SpriteRenderer>().color = Color.black;
                    break;
                default:
                    break;

            }
            var fsm = newitem.LocateMyFSM("Shiny Control");
            var reward = fsm.CopyState("Trink 1", "Reward");
            fsm.ChangeTransition("PD Bool?", "COLLECTED", "Fling?");
            fsm.ChangeTransition("Trink Flash", "FINISHED", "Reward");
            fsm.ChangeTransition("Init", "ACTIVATE", "PD Bool?");
            fsm.GetAction<BoolTest>("Init", 2).isTrue = null;
            reward.GetAction<SetSpriteRendererSprite>(2).sprite = sprite;
            reward.GetAction<SetTextMeshProText>(4).textString = name;
            reward.InsertCustomAction(() =>
            {
                action(giftname);
                GiftFactory.UpdateWeight();
                DisplayStates();
            }, 6);
            reward.RemoveAction(0);
            reward.RemoveAction(0);
            fsm.FsmVariables.GetFsmBool("Charm").Value = false;
            fsm.FsmVariables.GetFsmInt("Trinket Num").Value = 1;
            fsm.FsmVariables.GetFsmBool("Fling On Start").Value = false;
            var waitstate = fsm.CopyState("Fling Bubble", "wait state");
            waitstate.RemoveAction(0);
            waitstate.InsertAction(new Wait() { time = 0.2f, finishEvent = FsmEvent.Finished }, 0);
            waitstate.ChangeTransition("FINISHED", "Idle");
            fsm.ChangeTransition("Fling?", "FINISHED", "wait state");
            fsm.FsmVariables.GetFsmString("Tag").Value = "Boss";
            fsm.gameObject.FindGameObjectInChildren("Inspect Region").LocateMyFSM("inspect").FsmVariables.FindFsmBool("Inspectable").Value = false;
            fsm.InsertCustomAction("Idle", (fsm) =>
            {
                GameObject temp = fsm.gameObject;
                List<string> strings = new() { "Inspect Region", "Arrow Prompt(Clone)", "Labels", "Inspect" };
                foreach (string s in strings)
                {
                    temp = temp.FindGameObjectInChildren(s);
                    if (temp == null) { Log("null is " + s); return; }
                }
                temp.GetComponent<TMPro.TextMeshPro>().text = name;
                temp.GetComponent<TMPro.TextMeshPro>().fontSize = Rogue.Instance._set.item_font_size;
            }, 2);


            fsm.InsertCustomAction("Finish", () =>
            {
                Next();
            }, 8);
            fsm.GetAction<Wait>("Trink Pause", 0).time = 0.2f;


            newitem.SetActive(true);
            // newitem.AddComponent<Test>();
            newitem.LocateMyFSM("Shiny Control").SetState("Pause");
            items.Add(newitem);
        }

    }


    public void Next(bool start = false)
    {
        bool flag = false;
        if (!start)
        {
            switch (nowMode)
            {
                case Mode.select_small_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
                case Mode.select_big_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
                case Mode.one_big_gift:
                    flag = true;
                    break;
                case Mode.one_small_gift:
                    flag = true;
                    break;
                case Mode.fix_gift:
                    flag = true;
                    break;
                case Mode.fix_select_big_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
                case Mode.fix_select_small_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
            }
        }
        if (flag || start)
        {
            DestroyAllItems();
            if (rewardsStack.Count > 0)
            {
                OneReward reward;
                do
                {
                    reward = rewardsStack.Pop();
                }
                while (FixEmptyReward(reward) && rewardsStack.Count > 0);
                if (FixEmptyReward(reward)) return;
                if (before_spawn_item != null)
                {
                    reward = before_spawn_item(reward);
                }
                switch (reward.mode)
                {
                    case Mode.select_small_gift:
                        RandomSelectSmall(reward.select, reward.give);
                        break;
                    case Mode.select_big_gift:
                        RandomSelectBig(reward.select, reward.give);
                        break;
                    case Mode.one_big_gift:
                        RandomOneBig();
                        break;
                    case Mode.one_small_gift:
                        RandomOneSmall();
                        break;
                    case Mode.fix_gift:
                        FixGifts(reward.gifts);
                        break;
                    case Mode.fix_select_big_gift:
                        FixSelectBig(reward.gifts, reward.select);
                        break;
                    case Mode.fix_select_small_gift:
                        FixSelectSmall(reward.gifts, reward.select);
                        break;
                    default:
                        break;
                }
            }
        }

    }

    private bool FixEmptyReward(OneReward reward)
    {
        if ((reward.mode == Mode.fix_gift || reward.mode == Mode.fix_select_small_gift || reward.mode == Mode.fix_select_big_gift) && reward.gifts.Count <= 0) return true;
        return false;
    }

    private void Log(object msg)
    {

        Rogue.Instance.Log(msg);

    }
    private string TimerText()
    {
        //Modding.Logger.Log("now");
        return string.Format(
            "\n{0}:{1:D2}:{2:D3}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds
            );
    }


    private void DisplayEquipped()
    {
        var holder = GameObject.Find("charm_dispaly_holder");
        if (holder != null)
        {
            Log("Found existing holder.");
            DestroyImmediate(holder);
        }
        holder = new GameObject("charm_dispaly_holder");
        GameObject _GameCameras = null;
        if (HeroController.instance == null) return;
        foreach (var g in HeroController.instance.gameObject.scene.GetRootGameObjects())
        {
            if (g.name == "_GameCameras")
            {
                _GameCameras = g;
            }
        }
        var HudCamera = _GameCameras.transform.Find("HudCamera").gameObject;
        var Inventory = HudCamera.transform.Find("Inventory").gameObject;
        var Charms = Inventory.transform.Find("Charms").gameObject;
        var Collected_Charms = Charms.transform.Find("Collected Charms").gameObject;
        float x = 4;
        foreach (var num in PlayerData.instance.equippedCharms)
        {
            var c = Collected_Charms.transform.Find(num.ToString());
            var newC = new GameObject(c.name);
            newC.layer = LayerMask.NameToLayer("UI");
            newC.transform.SetParent(holder.transform);
            newC.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            var p = newC.transform.localPosition;
            newC.transform.localPosition = new Vector3(x, 6.3f, p.z);
            var render = newC.AddComponent<SpriteRenderer>();
            render.sprite = c.gameObject.FindGameObjectInChildren("Sprite").GetComponent<SpriteRenderer>().sprite;
            render.color = new Color(1, 1, 1, alpha);
            x += 1f;
        }
    }

    public void DisplayStates()
    {
        alpha = Rogue.Instance._set.UI_alpha;
        if (PlayerData.instance == null) return;
        PlayerData.instance.CalculateNotchesUsed();

        if (PlayerData.instance.charmSlots >= PlayerData.instance.charmSlotsFilled)
        {
            PlayerData.instance.overcharmed = false;
            GameManager.instance.RefreshOvercharm();
        }


        // inventory_display_holder = GameObject.Find("inventory_display_holder");
        if (inventory_display_holder != null)
        {
            Log("another inventory_display_holder");
            inventory_display_holder.SetActive(false);
            DestroyImmediate(inventory_display_holder);
        }
        inventory_display_holder = new GameObject("inventory_display_holder");

        if (GameInfo.role != CharacterRole.no_role)
        {
            GameObject role = new("role");
            role.layer = LayerMask.NameToLayer("UI");
            role.transform.SetParent(inventory_display_holder.transform);
            role.transform.localPosition = new Vector3(-14.3f, 7.6f, 0);
            var role_render = role.AddComponent<SpriteRenderer>();
            role_render.sprite = GiftFactory.all_gifts[(Giftname)GameInfo.role].GetSprite();
            if (GiftFactory.all_gifts[(Giftname)GameInfo.role].scale != Vector2.zero)
                role.transform.localScale = GiftFactory.all_gifts[(Giftname)GameInfo.role].scale;
            role_render.color = new Color(1, 1, 1, alpha);
            if ((Giftname)GameInfo.role == Giftname.role_test)
            {
                role.RemoveComponent<SpriteRenderer>();
                var test_text = role.AddComponent<TMPro.TextMeshPro>();
                test_text.color = new Color(1, 1, 1, alpha);
                test_text.text = "TEST";
                test_text.fontSize = 15;
                role.transform.position = new Vector3(-5.5f, 6.5f, 0);
            }
        }
        GameObject inventory = HutongGames.PlayMaker.FsmVariables.GlobalVariables.FindFsmGameObject("Inventory").Value;
        if (inventory == null) return;
        GameObject Inv = inventory.FindGameObjectInChildren("Inv");
        GameObject Inv_Items = Inv.FindGameObjectInChildren("Inv_Items");
        GameObject Equipment = Inv.FindGameObjectInChildren("Equipment");
        //x 4-14
        //y  -7
        float x = 4f;


        GameObject fireball = new GameObject("fireball");
        fireball.transform.SetParent(inventory_display_holder.transform);
        fireball.transform.localPosition = new Vector3(x, 7.5f, 0);
        fireball.layer = LayerMask.NameToLayer("UI");
        var render = fireball.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.fireballLevel == 1)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Fireball").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 1", 0).sprite.Value;
        }
        else if (PlayerData.instance.fireballLevel == 2)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Fireball").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 2", 0).sprite.Value;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject scream = new GameObject("scream");
        scream.transform.SetParent(inventory_display_holder.transform);
        scream.transform.localPosition = new Vector3(x, 7.5f, 0);
        scream.layer = LayerMask.NameToLayer("UI");
        render = scream.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.screamLevel == 1)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Scream").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 1", 0).sprite.Value;
        }
        else if (PlayerData.instance.screamLevel == 2)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Scream").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 2", 0).sprite.Value;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject quake = new GameObject("quake");
        quake.transform.SetParent(inventory_display_holder.transform);
        quake.transform.localPosition = new Vector3(x, 7.5f, 0);
        quake.layer = LayerMask.NameToLayer("UI");
        render = quake.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.quakeLevel == 1)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Quake").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 1", 0).sprite.Value;
        }
        else if (PlayerData.instance.quakeLevel == 2)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Quake").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 2", 0).sprite.Value;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject cyc_Slash = new GameObject("cyc_Slash");
        cyc_Slash.transform.SetParent(inventory_display_holder.transform);
        cyc_Slash.transform.localPosition = new Vector3(x, 7.5f, 0);
        cyc_Slash.layer = LayerMask.NameToLayer("UI");
        render = cyc_Slash.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasCyclone)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Art Cyclone").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject dash_Slash = new GameObject("dash_Slash");
        dash_Slash.transform.SetParent(inventory_display_holder.transform);
        dash_Slash.transform.localPosition = new Vector3(x, 7.5f, 0);
        dash_Slash.layer = LayerMask.NameToLayer("UI");
        render = dash_Slash.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasUpwardSlash)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Art Uppercut").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject upward_slash = new GameObject("upward_slash");
        upward_slash.transform.SetParent(inventory_display_holder.transform);
        upward_slash.transform.localPosition = new Vector3(x, 7.5f, 0);
        upward_slash.layer = LayerMask.NameToLayer("UI");
        render = upward_slash.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasDashSlash)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Art Dash").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject dash = new GameObject("dash");
        dash.transform.SetParent(inventory_display_holder.transform);
        dash.transform.localPosition = new Vector3(x, 7.5f, 0);
        dash.layer = LayerMask.NameToLayer("UI");
        render = dash.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasDash)
        {
            render.sprite = (Sprite)Equipment.FindGameObjectInChildren("Dash Cloak").GetComponent<SpriteRenderer>().sprite;
            if (PlayerData.instance.hasDash && !PlayerData.instance.hasShadowDash)
            {
                render.color = new Color(1, 1, 1, alpha);
            }
            else if (PlayerData.instance.hasDash && PlayerData.instance.hasShadowDash)
            {
                render.color = new Color(0, 0, 0, alpha);
            }
        }
        x += 1;

        GameObject walljump = new GameObject("walljump");
        walljump.transform.SetParent(inventory_display_holder.transform);
        walljump.transform.localPosition = new Vector3(x, 7.5f, 0);
        walljump.layer = LayerMask.NameToLayer("UI");
        render = walljump.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasWalljump)
        {
            render.sprite = (Sprite)Equipment.FindGameObjectInChildren("Mantis Claw").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject doublejump = new GameObject("doublejump");
        doublejump.transform.SetParent(inventory_display_holder.transform);
        doublejump.transform.localPosition = new Vector3(x, 7.5f, 0);
        doublejump.layer = LayerMask.NameToLayer("UI");
        render = doublejump.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasDoubleJump)
        {
            render.sprite = (Sprite)Equipment.FindGameObjectInChildren("Double Jump").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject superdash = new GameObject("superdash");
        superdash.transform.SetParent(inventory_display_holder.transform);
        superdash.transform.localPosition = new Vector3(x, 7.5f, 0);
        superdash.layer = LayerMask.NameToLayer("UI");
        render = superdash.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasSuperDash)
        {
            render.sprite = (Sprite)Equipment.FindGameObjectInChildren("Super Dash").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject acid_swim = new GameObject("acid_swim");
        acid_swim.transform.SetParent(inventory_display_holder.transform);
        acid_swim.transform.localPosition = new Vector3(x, 7.5f, 0);
        acid_swim.layer = LayerMask.NameToLayer("UI");
        render = acid_swim.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasAcidArmour)
        {
            render.sprite = (Sprite)Equipment.FindGameObjectInChildren("Acid Armour").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        x = 4f;
        GameObject dream_nail = new GameObject("dream_nail");
        dream_nail.transform.SetParent(inventory_display_holder.transform);
        dream_nail.transform.localPosition = new Vector3(14, 6f, 0);
        dream_nail.layer = LayerMask.NameToLayer("UI");
        render = dream_nail.AddComponent<SpriteRenderer>();
        if (PlayerData.instance.hasDreamNail)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Dream Nail").GetComponent<SpriteRenderer>().sprite;
        }
        render.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject nail = new GameObject("nail");
        nail.transform.SetParent(inventory_display_holder.transform);
        nail.transform.localPosition = new Vector3(15, 6.5f, 0);
        nail.layer = LayerMask.NameToLayer("UI");
        render = nail.AddComponent<SpriteRenderer>();
        GameObject level = new GameObject("level");
        level.transform.SetParent(nail.transform);
        level.transform.localPosition = new Vector3(9.5f, -0.5f, 0);
        level.layer = LayerMask.NameToLayer("UI");
        var text = level.AddComponent<TMPro.TextMeshPro>();
        text.fontSize = 6;
        if (all_font == null)
        {
            all_font = Equipment.FindGameObjectInChildren("Rancid Egg").FindGameObjectInChildren("Egg Amount").GetComponent<TMPro.TextMeshPro>().font;
        }
        text.font = all_font;
        if (PlayerData.instance.nailDamage <= 1)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Nail").GetComponent<InvNailSprite>().level1;
            text.text = "-1";
        }
        else if (PlayerData.instance.nailDamage <= 5)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Nail").GetComponent<InvNailSprite>().level1;
            text.text = "0";
        }
        else if (PlayerData.instance.nailDamage <= 9)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Nail").GetComponent<InvNailSprite>().level2;
            text.text = "1";
        }
        else if (PlayerData.instance.nailDamage <= 13)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Nail").GetComponent<InvNailSprite>().level3;
            text.text = "2";
        }
        else if (PlayerData.instance.nailDamage <= 17)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Nail").GetComponent<InvNailSprite>().level4;
            text.text = "3";
        }
        else
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Nail").GetComponent<InvNailSprite>().level5;
            text.text = "4";
        }
        render.color = new Color(1, 1, 1, alpha);
        text.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject egg = new GameObject("egg");
        egg.transform.SetParent(inventory_display_holder.transform);
        egg.transform.localPosition = new Vector3(12.5f, 6.3f, 0);
        egg.layer = LayerMask.NameToLayer("UI");
        render = egg.AddComponent<SpriteRenderer>();
        render.sprite = (Sprite)Equipment.FindGameObjectInChildren("Rancid Egg").GetComponent<SpriteRenderer>().sprite;
        GameObject egg_num = new GameObject("egg_num");
        egg_num.transform.SetParent(egg.transform);
        egg_num.transform.localPosition = new Vector3(10.2f, -2.6f, 0);
        egg_num.layer = LayerMask.NameToLayer("UI");
        text = egg_num.AddComponent<TMPro.TextMeshPro>();
        text.fontSize = 6;
        if (all_font == null)
        {
            all_font = Equipment.FindGameObjectInChildren("Rancid Egg").FindGameObjectInChildren("Egg Amount").GetComponent<TMPro.TextMeshPro>().font;
        }
        text.font = all_font;
        egg_num.GetComponent<TMPro.TextMeshPro>().text = GameInfo.revive_num.ToString();
        render.color = new Color(1, 1, 1, alpha);
        text.color = new Color(1, 1, 1, alpha);
        x += 1;

        GameObject charm_slot = new GameObject("charm_slot");
        charm_slot.transform.SetParent(inventory_display_holder.transform);
        charm_slot.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        charm_slot.transform.localPosition = new Vector3(12.5f, 5.1f, 0);
        charm_slot.layer = LayerMask.NameToLayer("UI");
        render = charm_slot.AddComponent<SpriteRenderer>();
        render.sprite = AssemblyUtils.GetSpriteFromResources("Charm_Notch.png");
        GameObject charm_slot_num = new GameObject("charm_slot_num");
        charm_slot_num.transform.SetParent(charm_slot.transform);
        charm_slot_num.transform.localPosition = new Vector3(14.5f, -3.7f, 0);
        charm_slot_num.layer = LayerMask.NameToLayer("UI");
        text = charm_slot_num.AddComponent<TMPro.TextMeshPro>();
        text.fontSize = 6;
        if (all_font == null)
        {
            all_font = Equipment.FindGameObjectInChildren("Rancid Egg").FindGameObjectInChildren("Egg Amount").GetComponent<TMPro.TextMeshPro>().font;
        }
        text.font = all_font;
        charm_slot_num.GetComponent<TMPro.TextMeshPro>().text = PlayerData.instance.charmSlots.ToString();
        render.color = new Color(1, 1, 1, alpha);
        text.color = new Color(1, 1, 1, alpha);


    }
}
