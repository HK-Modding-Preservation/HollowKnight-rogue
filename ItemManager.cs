using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.AccessControl;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using IL.tk2dRuntime.TileMap;
using IL.TMPro;
using JetBrains.Annotations;
using Modding;
using Satchel;
using Satchel.BetterMenus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace rogue;

public class ItemManager : MonoBehaviour
{
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


    public float timer = 0f;


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

        public List<Rogue.gift> gifts;
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

    public List<Rogue.giftname> roleList = new List<Rogue.giftname>
        {
            Rogue.giftname.nail_master,
            Rogue.giftname.shaman,
            Rogue.giftname.hunter,
            // Rogue.giftname.uunn,
            Rogue.giftname.joni,
            Rogue.giftname.moth,
            Rogue.giftname.grey_prince,
            Rogue.giftname.Tuk,
            // Rogue.giftname.defender,
            Rogue.giftname.mantis,
            Rogue.giftname.collector
        };

    public string rouge_introduction = "rogue_introduction".Localize();

    public bool hatchscene = false;





    public void Start()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        Modding.ModHooks.LanguageGetHook += OnLanguageGet;
        On.PlayerData.GetBool += OnGetBool;
        On.PlayerData.SetBool += OnSetBool;
        On.PlayerData.GetInt += OnGetInt;
        ModHooks.CharmUpdateHook += OnCharmUpdate;
        On.HealthManager.TakeDamage += OnTakeDamage;
        On.SpellFluke.OnEnable += OnFlukeDamage;
        On.PlayMakerFSM.OnEnable += OnFSMEnable;
        On.KnightHatchling.OnEnable += OnHatchingDamage;
        On.KnightHatchling.DoChaseSimple += OnHatchingChaseSimple;
        On.KnightHatchling.DoChase += OnHatchingChase;



    }

    private void OnHatchingChase(On.KnightHatchling.orig_DoChase orig, KnightHatchling self, Transform target, float distance, float speedMax, float accelerationForce, float targetRadius, float deceleration, Vector2 offset)
    {
        if (PlayerData.instance.equippedCharm_1)
        {
            ReflectionHelper.CallMethod<KnightHatchling>(self, "DoChaseSimple", new object[] { HeroController.instance.gameObject.transform, speedMax, accelerationForce, offset.x, offset.y });
        }
        else
        {
            orig(self, target, distance, speedMax, accelerationForce, targetRadius, deceleration, offset);
        }
    }

    private void OnHatchingChaseSimple(On.KnightHatchling.orig_DoChaseSimple orig, KnightHatchling self, Transform target, float speedMax, float accelerationForce, float offsetX, float offsetY)
    {
        if (PlayerData.instance.equippedCharm_1)
        {
            orig(self, HeroController.instance.gameObject.transform, speedMax, accelerationForce, offsetX, offsetY);
        }
        else
        {
            orig(self, target, speedMax, accelerationForce, offsetX, offsetY);
        }
    }
    private void HatchChange()
    {
        GameObject charmeffect = GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects");
        var fsm = charmeffect.LocateMyFSM("Hatchling Spawn");
        var spell_con = GameObject.Find("Knight").LocateMyFSM("Spell Control");
        if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
        {
            int mp_cost = 8;
            if (PlayerData.instance.equippedCharm_33) mp_cost -= 4;
            fsm.FsmVariables.FindFsmInt("Soul Cost").Value = mp_cost;

            int hatch_max = 4;
            int spell_level = PlayerData.instance.fireballLevel + PlayerData.instance.quakeLevel + PlayerData.instance.screamLevel;
            if (spell_level >= 3) hatch_max *= 2;
            if (spell_level >= 6) hatch_max *= 2;
            if (PlayerData.instance.equippedCharm_11) hatch_max = 16;
            fsm.FsmVariables.FindFsmInt("Hatchling Max").Value = hatch_max;

            float hatch_time = 4;
            float hatch_speed = 1;
            hatch_speed += 0.3f * spell_level;
            if (PlayerData.instance.equippedCharm_32) hatch_speed *= 2;
            fsm.FsmVariables.FindFsmFloat("Hatch Time").Value = hatch_time / hatch_speed;

            var hatch = fsm.GetState("Hatch 2");
            if (hatch == null)
            {
                hatch = fsm.CopyState("Hatch", "Hatch 2");
                hatch.ChangeTransition("FINISHED", "Equipped");
                hatch.RemoveAction(0);
            }
            var check = fsm.GetState("Check Count 2");
            if (check == null)
            {
                check = fsm.CopyState("Check Count", "Check Count 2");
                check.ChangeTransition("CANCEL", "Equipped");
                check.ChangeTransition("FINISHED", "Hatch 2");

            }
            if (fsm.GetState("Equipped").Transitions.Count() == 1)
                fsm.AddTransition("Equipped", "SPAWN", "Check Count 2");
            if (PlayerData.instance.equippedCharm_17)
            {
                var hatch_cloud = spell_con.GetState("Hatch Cloud");
                if (hatch_cloud == null)
                {
                    hatch_cloud = spell_con.CopyState("Dung Cloud", "Hatch Cloud");
                    hatch_cloud.RemoveAction(0);
                    hatch_cloud.RemoveAction(0);
                    hatch_cloud.RemoveAction(0);
                    hatch_cloud.ChangeTransition("FINISHED", "Set HP Amount");
                    hatch_cloud.InsertCustomAction((state) =>
                    {
                        float num = 2;
                        if (PlayerData.instance.equippedCharm_33) num *= 1.5f;
                        if (PlayerData.instance.equippedCharm_34) num *= 2;
                        for (int i = 0; i < (int)num; i++)
                            GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects").LocateMyFSM("Hatchling Spawn").SendEvent("SPAWN");
                    }, 0);

                }
                if (spell_con.GetState("Spore Cloud").Transitions.Length == 2)
                {
                    spell_con.GetState("Spore Cloud").AddTransition("HATCH", "Hatch Cloud");
                }
                if (spell_con.GetState("Spore Cloud").Actions.Length == 6)
                {
                    spell_con.InsertCustomAction("Spore Cloud", (fsm) =>
                    {
                        if (PlayerData.instance.equippedCharm_22) fsm.SendEvent("HATCH");
                    }, 2);
                }
            }
            if (PlayerData.instance.equippedCharm_11)
            {
                fsm.ChangeTransition("Equipped", "FINISHED", "no");
                var state = spell_con.GetState("Fireball Recoil 2");
                if (state == null)
                {
                    spell_con.CopyState("Fireball Recoil", "Fireball Recoil 2");
                    state = spell_con.GetState("Fireball Recoil 2");
                    state.InsertCustomAction(() =>
                    {
                        int mp = 33;
                        int num = 4;

                        if (PlayerData.instance.equippedCharm_33) { mp = 24; num += 2; }
                        if (PlayerData.instance.fireballLevel == 2) { num += 2; }
                        HeroController.instance.TakeMP(mp);
                        for (int i = 0; i < num; i++)
                            GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects").LocateMyFSM("Hatchling Spawn").SendEvent("SPAWN");
                    }, 0);
                    // state.RemoveAction(1);
                    state.ChangeTransition("ANIM END", "Spell End");
                }
                spell_con.ChangeTransition("Fireball Antic", "ANIM END", "Fireball Recoil 2");
            }
            else
            {
                fsm.ChangeTransition("Equipped", "FINISHED", "Can Hatch?");
                spell_con.ChangeTransition("Fireball Antic", "ANIM END", "Level Check");
            }

        }
        else
        {
            fsm.FsmVariables.FindFsmInt("Soul Cost").Value = 8;
            fsm.FsmVariables.FindFsmInt("Hatchling Max").Value = 4;
            fsm.FsmVariables.FindFsmFloat("Hatch Time").Value = 4;
            fsm.ChangeTransition("Equipped", "FINISHED", "Can Hatch?");
            spell_con.ChangeTransition("Fireball Antic", "ANIM END", "Level Check");
        }
    }

    private void OnHatchingDamage(On.KnightHatchling.orig_OnEnable orig, KnightHatchling self)
    {
        orig(self);
        GameObject charmeffect = GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects");
        var fsm = charmeffect.LocateMyFSM("Hatchling Spawn");
        if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
        {
            var de = ReflectionHelper.GetField<KnightHatchling, KnightHatchling.TypeDetails>(self, "details");
            float damage = de.damage;
            if (Rogue.getBirthright) damage += 3;
            damage += PlayerData.instance.nailDamage * 1.0f / 4;
            if (PlayerData.instance.equippedCharm_25) damage *= 1.5f;
            if (PlayerData.instance.equippedCharm_11) damage *= 0.5f;
            de.damage = (int)damage;

            ReflectionHelper.SetField(self, "details", de);


            if (hatchscene) return;
            hatchscene = false;

        }
        else
        {


        }
    }

    private int OnGetInt(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
    {
        if (Rogue.Instance.inRogue)
        {
            if (Rogue.role == Rogue.giftname.grey_prince)
            {
                if (intName.StartsWith("charmCost_")) return 0;
            }
            if (Rogue.role == Rogue.giftname.nail_master)
            {
                if (intName == "charmCost_26") return 0;
            }
            if (Rogue.role == Rogue.giftname.mantis)
            {
                if (intName == "charmCost_13") return 0;
            }
            if (Rogue.role == Rogue.giftname.collector)
            {
                if (intName == "charmCost_38") return 0;
                if (intName == "charmCost_39") return 0;
                if (intName == "charmCost_40") return 0;
                if (intName == "charmCost_22") return 0;
            }

        }
        return orig(self, intName);
    }

    private void OnFlukeDamage(On.SpellFluke.orig_OnEnable orig, SpellFluke self)
    {
        orig(self);
        if (Rogue.Instance.inRogue)
        {
            if (Rogue.role == Rogue.giftname.shaman)
            {
                if (PlayerData.instance.equippedCharm_19)
                {
                    ReflectionHelper.SetField<SpellFluke, int>(self, "damage", 6);
                }
                else
                {
                    ReflectionHelper.SetField<SpellFluke, int>(self, "damage", 5);
                }
            }
        }
    }

    private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (!Rogue.Instance.inRogue)
        {
            orig(self, hitInstance);
            return;
        }
        switch (hitInstance.AttackType)
        {
            case AttackTypes.Nail:
                if (Rogue.role == Rogue.giftname.shaman)
                {
                    hitInstance.Multiplier *= 0.8f;
                }
                else if (Rogue.role == Rogue.giftname.joni)
                {
                    hitInstance.Multiplier *= 1.75f;
                }
                else if (Rogue.role == Rogue.giftname.defender)
                {
                    hitInstance.Multiplier *= 1.25f;
                }
                else if (Rogue.role == Rogue.giftname.mantis)
                {
                    hitInstance.Multiplier *= 1.25f;
                }
                else if (Rogue.role == Rogue.giftname.collector)
                {
                    float mul = 0.7f;
                    if (PlayerData.instance.equippedCharm_22) mul *= 0.7f;
                    if (PlayerData.instance.equippedCharm_38) mul *= 0.7f;
                    if (PlayerData.instance.equippedCharm_39) mul *= 0.7f;
                    hitInstance.Multiplier *= mul;
                }
                break;
            case AttackTypes.Generic:
                break;
            case AttackTypes.Spell:
                if (Rogue.role == Rogue.giftname.shaman)
                {
                    hitInstance.Multiplier *= 1.25f;
                }
                else if (Rogue.role == Rogue.giftname.collector)
                {
                    float mul = 0.7f;
                    if (PlayerData.instance.equippedCharm_22) mul *= 0.7f;
                    if (PlayerData.instance.equippedCharm_38) mul *= 0.7f;
                    if (PlayerData.instance.equippedCharm_39) mul *= 0.7f;
                    hitInstance.Multiplier *= mul;
                }
                break;
            case AttackTypes.Splatter:
                break;
            case AttackTypes.SharpShadow:
                break;
            case AttackTypes.NailBeam:
                break;
            case AttackTypes.RuinsWater:
                break;
            default: break;
        }
        orig(self, hitInstance);
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
        if (Rogue.Instance.name_and_desc.ContainsKey(key))
        {
            return Rogue.Instance.name_and_desc[key];
        }
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
        if (HeroController.instance != null)
        {
            HatchChange();
        }
        if (Rogue.Instance.inRogue && BossSequenceController.IsInSequence)
        {
            List<string> nobossscene = new List<string> { "GG_Spa", "GG_Engine", "GG_Unn", "GG_Engine_Root", "GG_Wyrm", "GG_Atrium_Roof" };
            if (!nobossscene.Contains(arg0.name))
            {
                int geo = 50;
                if (PlayerData.instance.equippedCharm_24) geo += 20;
                if (damaged_num == 0 || Rogue.role == Rogue.giftname.uunn) geo += 20;
                HeroController.instance.AddGeo(geo);
            }
        }
        damaged_num = 0;
        if (arg1.name == "GG_Engine")
        {
            StartCoroutine(DelayShowDreamConvo(0.5f, "godseeker".Localize()));
        }
        if (arg1.name == "GG_Atrium_Roof")
        {
            GameObject taijie = Instantiate(GameObject.Find("gg_plat_float_small"));
            taijie.transform.SetPosition2D(new Vector2(118, 64));
            taijie.SetActive(true);
            if (Rogue.Instance.inRogue)
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
            if (Rogue.Instance.inRogue)
            {
                Rogue.Instance.spa_count++;
                Rogue.Instance.UpdateWeight();
                DestroyAllItems();
                GiveReward(Rogue.Instance.spa_count);
                if (Rogue.Instance.spa_count % 2 == 0 || Rogue.Instance.spa_count == 7)
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
        }
    }
    public void SpwanShopItem(List<Rogue.gift> gifts, bool select = false)
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
        List<Rogue.giftname> charms = new(){
                Rogue.giftname.shop_any_charm_1,
                Rogue.giftname.shop_any_charm_2,
                Rogue.giftname.shop_any_charm_3,
                Rogue.giftname.shop_any_charm_4
            };
        foreach (var gift in gifts)
        {
            GameObject good = Instantiate(shop_item_temp);
            var goodstats = good.GetComponent<ShopItemStats>();
            var render = good.FindGameObjectInChildren("Item Sprite").GetComponent<SpriteRenderer>();
            if (charms.Contains(gift.giftname))
            {
                var list = RandomList(Rogue.Instance.actCharmRewards, 1, false);
                if (list.Count <= 0)
                {
                    Destroy(good);
                    continue;
                }
                Rogue.gift charmgift = list[0];
                good.name = charmgift.name;
                goodstats.nameConvo = charmgift.name;
                goodstats.descConvo = charmgift.desc;
                goodstats.priceConvo = charmgift.price.ToString();
                goodstats.specialType = (int)charmgift.giftname + 18;
                goodstats.cost = charmgift.price;
                if (charmgift.sprite != null) render.sprite = charmgift.sprite;
                else if (charmgift.getSprite != null) render.sprite = charmgift.getSprite(charmgift.giftname);
                else render.sprite = null;
                var item = good.FindGameObjectInChildren("Item Sprite");
                if (charmgift.scale != Vector2.zero) item.transform.localScale = charmgift.scale;
                if (select) goodstats.playerDataBoolName = selectname;
                else
                    goodstats.playerDataBoolName = charmgift.giftname.ToString() + count;
                count++;

            }
            else
            {
                good.name = gift.name;
                goodstats.nameConvo = gift.name;
                goodstats.descConvo = gift.desc;
                goodstats.priceConvo = gift.price.ToString();
                goodstats.specialType = (int)gift.giftname + 18;
                goodstats.cost = gift.price;
                if (gift.sprite != null) render.sprite = gift.sprite;
                else if (gift.getSprite != null) render.sprite = gift.getSprite(gift.giftname);
                else render.sprite = null;
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
        List<Rogue.gift> roles = new();
        foreach (var role in roleList)
        {
            roles.Add(Rogue.Instance.shopRewards[role]);
        }
        if (Rogue.Instance._set.owner) roles = roles.Append(Rogue.Instance.shopRewards[Rogue.giftname.test]).ToList();
        SpwanShopItem(roles, select: true);

    }
    public void NormalShopItem()
    {
        List<Rogue.gift> items = new List<Rogue.gift>();
        items.Add(Rogue.Instance.shopRewards[Rogue.giftname.shop_keeper_key]);
        if (Rogue.Instance.shopRewards[Rogue.giftname.shop_add_1_notch_1].weight > 0)
            items.Add(Rogue.Instance.shopRewards[Rogue.giftname.shop_add_1_notch_1]);
        if (Rogue.Instance.shopRewards[Rogue.giftname.shop_nail_upgrade].weight > 0)
            items.Add(Rogue.Instance.shopRewards[Rogue.giftname.shop_nail_upgrade]);
        items.Add(Rogue.Instance.shopRewards[Rogue.giftname.shop_random_gift]);
        var ranlist = RandomList(Rogue.Instance.actShopRewards, num: 9 - items.Count);
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
                gifts = Rogue.Instance.actBigRewards
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
            if (Rogue.Instance.refresh_num > 0)
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
                                Rogue.Instance.refresh_num--;
                                RandomSelectSmall(tempselect, t);
                                break;
                            case Mode.select_big_gift:
                                DestroyAllItems();
                                Rogue.Instance.refresh_num--;
                                RandomSelectBig(tempselect, t);
                                break;
                            case Mode.one_big_gift:
                                DestroyAllItems();
                                Rogue.Instance.refresh_num--;
                                RandomOneBig();
                                break;
                            case Mode.one_small_gift:
                                DestroyAllItems();
                                Rogue.Instance.refresh_num--;
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
            Rogue.Instance.Rogue_Over();
            ModHooks.AfterTakeDamageHook -= CheckIfdamaged;
        }
        if (Rogue.Instance.inRogue && BossSequenceController.IsInSequence && (scenename != "GG_Spa") && (scenename! != "GG_Atrium_Roof"))
        {
            if (!GameManager.instance.isPaused)
            {
                timer += Time.unscaledDeltaTime;
                timeSpan = TimeSpan.FromSeconds(timer);
            }
        }

        if (refresh_text_pro != null)
        {
            refresh_text_pro.text = Rogue.Instance.refresh_num.ToString();
        }
        if (time_display_text_pro != null)
        {
            time_display_text_pro.text = TimerText();
        }

    }

    private int CheckIfdamaged(int hazardType, int damageAmount)
    {
        damaged_num++;
        return damageAmount;
    }

    public void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        ModHooks.LanguageGetHook -= OnLanguageGet;
        On.PlayerData.GetBool -= OnGetBool;
        On.PlayerData.SetBool -= OnSetBool;
        On.PlayerData.GetInt -= OnGetInt;
        ModHooks.CharmUpdateHook -= OnCharmUpdate;
        On.HealthManager.TakeDamage -= OnTakeDamage;
        On.SpellFluke.OnEnable -= OnFlukeDamage;
        On.PlayMakerFSM.OnEnable -= OnFSMEnable;
        On.KnightHatchling.OnEnable -= OnHatchingDamage;

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
                    if (Rogue.Instance.inRogue && !Rogue.getBirthright)
                    {
                        Rogue.Instance.smallRewards[Rogue.giftname.get_birthright].reward(Rogue.giftname.get_birthright);
                        Rogue.Instance.UpdateWeight();
                        DisplayStates();

                    }
                }, 1);
            }
        }
        else if (self.FsmName == "Control" && self.gameObject.name.Contains("Grimmchild"))
        {
            if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
            {
                float damage = 3;
                if (Rogue.getBirthright) damage += 1;
                damage += PlayerData.instance.nailDamage * 1.0f / 3;
                if (PlayerData.instance.equippedCharm_25) damage *= 1.5f;
                if (PlayerData.instance.equippedCharm_6 && PlayerData.instance.health == 1) damage *= 1.75f;
                self.GetAction<SetIntValue>("Level 4", 0).intValue = (int)damage;


                self.GetAction<Wait>("Follow", 18).time = 0.25f;
                self.GetAction<SetFloatValue>("No Target", 0).floatValue = 0.3f;
                float attack_timer = 0.59f;
                float attack_speed = 1f;
                int spelllevel = PlayerData.instance.fireballLevel + PlayerData.instance.quakeLevel + PlayerData.instance.screamLevel;
                attack_speed += spelllevel * 0.3f;
                if (PlayerData.instance.equippedCharm_32) attack_speed *= 1.5f;
                self.GetAction<RandomFloat>("Antic", 3).min = attack_timer / attack_speed;
                self.GetAction<RandomFloat>("Antic", 3).max = attack_timer / attack_speed;
                float anticfps = 16;
                float shootfps = 12;
                self.gameObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Shoot 4").fps = shootfps * attack_speed;
                self.gameObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Antic 4").fps = anticfps * attack_speed;




                float speed = 50f;
                if (PlayerData.instance.equippedCharm_31) speed += 10f;
                if (PlayerData.instance.equippedCharm_37) speed += 10f;
                self.FsmVariables.FindFsmFloat("Flameball Speed").Value = speed;

                GameObject grimm = GameObject.Find("Grimmchild(Clone)");
                if (grimm != null)
                {
                    float scale = 1f;
                    if (PlayerData.instance.equippedCharm_13) scale += 1f;
                    if (PlayerData.instance.equippedCharm_18) scale += 0.5f;
                    grimm.FindGameObjectInChildren("Enemy Range").transform.localScale = new Vector3(scale, scale, scale);
                }



            }
            else
            {
                self.GetAction<Wait>("Follow", 18).time = 0.25f;
                self.GetAction<SetIntValue>("Level 4", 0).intValue = (int)11;
                self.GetAction<SetFloatValue>("No Target", 0).floatValue = 0.75f;
                self.GetAction<RandomFloat>("Antic", 3).min = 1.5f;
                self.GetAction<RandomFloat>("Antic", 3).max = 1.5f;
                self.FsmVariables.FindFsmFloat("Flameball Speed").Value = 30f;
                self.gameObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Shoot 4").fps = 12;
                self.gameObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Antic 4").fps = 16;

                GameObject grimm = GameObject.Find("Grimmchild(Clone)");
                if (grimm != null)
                {
                    grimm.FindGameObjectInChildren("Enemy Range").transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else if (self.FsmName == "Hatchling Spawn" && self.gameObject.name.Contains("Charm Effects"))
        {

        }
        else if (self.FsmName == "Control" && self.gameObject.name.Contains("Weaverling"))
        {
            if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
            {
                if (PlayerData.instance.equippedCharm_31)
                {
                    self.GetAction<SetFloatValue>("Sprintmaster", 0).floatValue = 1.5f;
                    self.GetAction<SetFloatValue>("Sprintmaster", 2).floatValue = 2.25f;
                }
                else
                {
                    self.GetAction<SetFloatValue>("Sprintmaster", 0).floatValue = 1f;
                    self.GetAction<SetFloatValue>("Sprintmaster", 2).floatValue = 1.5f;
                }
                if (PlayerData.instance.equippedCharm_1)
                {
                    self.GetAction<RandomFloat>("Init", 1).min = 1.5f;
                    self.GetAction<RandomFloat>("Init", 1).max = 1.5f;
                }
                else
                {
                    self.GetAction<RandomFloat>("Init", 1).min = 1f;
                    self.GetAction<RandomFloat>("Init", 1).max = 1f;
                }
            }
            else
            {
                self.GetAction<SetFloatValue>("Sprintmaster", 0).floatValue = 1f;
                self.GetAction<SetFloatValue>("Sprintmaster", 2).floatValue = 1.5f;
                self.GetAction<RandomFloat>("Init", 1).min = 1f;
                self.GetAction<RandomFloat>("Init", 1).max = 1f;
            }
        }
        else if (self.FsmName == "Attack" && self.gameObject.name.Contains("Enemy Damager") && self.gameObject.transform.parent.name.Contains("Weaverling"))
        {
            if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
            {
                float damage = 3;
                damage += PlayerData.instance.nailDamage * 1.0f / 8;
                if (PlayerData.instance.equippedCharm_25) damage *= 1.5f;
                if (PlayerData.instance.equippedCharm_6 && PlayerData.instance.health == 1) damage *= 1.75f;
                self.FsmVariables.FindFsmInt("Damage").Value = (int)damage;

                int mp_get = Rogue.getBirthright ? 5 : 3;
                int spelllevel = PlayerData.instance.fireballLevel + PlayerData.instance.quakeLevel + PlayerData.instance.screamLevel;
                mp_get += spelllevel / 2;
                if (PlayerData.instance.equippedCharm_20) mp_get += 1;
                if (PlayerData.instance.equippedCharm_35) mp_get += 2;
                if (PlayerData.instance.equippedCharm_21) mp_get += 2;
                self.GetAction<CallMethodProper>("Grubsong", 1).parameters[0].intValue = mp_get;



            }
            else
            {
                self.FsmVariables.FindFsmInt("Damage").Value = 3;
                self.GetAction<CallMethodProper>("Grubsong", 1).parameters[0].intValue = 3;
            }

        }
        else if (self.FsmName == "Shield Hit" && self.gameObject.name.Contains("Shield"))
        {
            if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
            {
                if (self.GetState("Init").Actions.Length == 10)
                {
                    self.InsertCustomAction("Init", (fsm) =>
                    {
                        float damage = PlayerData.instance.nailDamage;
                        if (Rogue.getBirthright) damage += 5;
                        if (PlayerData.instance.equippedCharm_25) damage *= 1.5f;
                        if (PlayerData.instance.equippedCharm_6 && PlayerData.instance.health == 1) damage *= 1.75f;
                        self.FsmVariables.FindFsmInt("Damage").Value = (int)damage;
                    }, 10);
                }
                float timer = 2f;
                int spelllevel = PlayerData.instance.fireballLevel + PlayerData.instance.quakeLevel + PlayerData.instance.screamLevel;
                timer -= spelllevel * 0.2f;
                if (PlayerData.instance.equippedCharm_4) timer -= 0.2f;
                self.GetAction<Wait>("Break", 3).time = timer;

                float scale = 1f;
                if (PlayerData.instance.equippedCharm_18) scale += 0.15f;
                if (PlayerData.instance.equippedCharm_13) scale += 0.25f;
                self.GetAction<SetScale>("Dreamwielder?", 0).x = -scale;
                self.GetAction<SetScale>("Dreamwielder?", 0).y = scale;
                self.GetAction<SetScale>("Dreamwielder?", 1).x = scale;
                self.GetAction<SetScale>("Dreamwielder?", 1).y = scale;
                self.GetAction<SetScale>("Dreamwielder?", 3).x = -scale * 1.15f;
                self.GetAction<SetScale>("Dreamwielder?", 3).y = scale * 1.15f;
                self.GetAction<SetScale>("Dreamwielder?", 4).x = scale * 1.15f;
                self.GetAction<SetScale>("Dreamwielder?", 4).y = scale * 1.15f;

            }
            else
            {
                if (self.GetState("Init").Actions.Length == 11)
                {
                    self.RemoveAction("Init", 10);
                }
                self.FsmVariables.FindFsmInt("Damage").Value = 10;
                self.GetAction<Wait>("Break", 3).time = 2;
                self.GetAction<SetScale>("Dreamwielder?", 0).x = -1;
                self.GetAction<SetScale>("Dreamwielder?", 0).y = 1;
                self.GetAction<SetScale>("Dreamwielder?", 1).x = 1;
                self.GetAction<SetScale>("Dreamwielder?", 1).y = 1;
                self.GetAction<SetScale>("Dreamwielder?", 3).x = -1.15f;
                self.GetAction<SetScale>("Dreamwielder?", 3).y = 1.15f;
                self.GetAction<SetScale>("Dreamwielder?", 4).x = 1.15f;
                self.GetAction<SetScale>("Dreamwielder?", 4).y = 1.15f;
            }

        }
        else if (self.FsmName == "Control" && self.gameObject.name.Contains("Orbit Shield"))
        {
            if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
            {
                float speed = 110f;
                if (PlayerData.instance.equippedCharm_32) speed = 250f;
                self.FsmVariables.FindFsmFloat("Speed").Value = speed;
            }
            else
            {
                self.FsmVariables.FindFsmFloat("Speed").Value = 110f;
            }

        }
        else if (self.FsmName == "Focus Speedup" && self.gameObject.name.Contains("Orbit Shield"))
        {
            if (Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.collector)
            {
                float speed = 110f;
                float focus_speed = 300f;
                if (PlayerData.instance.equippedCharm_32)
                {
                    speed = 250f;
                    focus_speed = 1000f;
                }
                self.GetAction<SetFsmFloat>("Idle", 0).setValue = speed;
                self.GetAction<SetFsmFloat>("Focus", 0).setValue = focus_speed;
            }
            else
            {
                self.GetAction<SetFsmFloat>("Idle", 0).setValue = 110f;
                self.GetAction<SetFsmFloat>("Focus", 0).setValue = 300f;
            }

        }
        orig(self);

    }

    public List<Rogue.gift> RandomList(List<Rogue.gift> gifts, int num = 1, bool canRepeat = false)
    {
        List<Rogue.gift> res = new();
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
                while (now == 0) now = UnityEngine.Random.Range(0, whole);
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
        int count = Rogue.Instance.actSmallRewards.Count;
        if (count <= 0) return;
        var list = RandomList(Rogue.Instance.actSmallRewards, 1);
        if (list.Count > 0)
            SpwanItem(list[0]);
    }
    public void RandomOneBig()
    {
        items.Clear();
        nowMode = Mode.one_big_gift;
        int count = Rogue.Instance.actBigRewards.Count;
        if (count <= 0) return;
        var list = RandomList(Rogue.Instance.actBigRewards, 1);
        if (list.Count > 0)
            SpwanItem(list[0]);
    }
    public void RandomSelectSmall(int select, int give)
    {
        items.Clear();
        nowMode = Mode.select_small_gift;
        tempselect = select;
        int count = Rogue.Instance.actSmallRewards.Count;
        if (count <= 0) return;
        var list = RandomList(Rogue.Instance.actSmallRewards, give);
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
        int count = Rogue.Instance.actBigRewards.Count;
        if (count <= 0) return;
        var list = RandomList(Rogue.Instance.actBigRewards, give);
        foreach (var item in list)
        {
            SpwanItem(item);
        }


    }

    public void FixGifts(List<Rogue.gift> gifts)
    {
        items.Clear();
        nowMode = Mode.fix_gift;
        foreach (var gift in gifts)
        {
            SpwanItem(gift);
        }
    }

    public void FixSelectBig(List<Rogue.gift> bigs, int select)
    {
        items.Clear();
        nowMode = Mode.fix_select_big_gift;
        tempselect = select;
        foreach (var gift in bigs)
        {
            SpwanItem(gift, true);
        }

    }
    public void FixSelectSmall(List<Rogue.gift> smalls, int select)
    {
        items.Clear();
        nowMode = Mode.fix_select_big_gift;
        tempselect = select;
        foreach (var gift in smalls)
        {
            SpwanItem(gift, true);
        }

    }
    private void SpwanItem(Rogue.gift gift, bool inSelect = false)
    {
        if (gift == null) return;
        SpwanItem(gift.level, gift.name, gift.reward, gift.giftname);
        return;

    }


    private void SpwanItem(int level, string name, Action<Rogue.giftname> action, Rogue.giftname giftname)
    {
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
                    Log("\n");
                    Log(newitem.transform.position.x);
                    foreach (var item in items)
                    {
                        Log(item.transform.position.x);
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
                Rogue.Instance.UpdateWeight();
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
                var reward = rewardsStack.Pop();
                switch (reward.mode)
                {
                    case Mode.select_small_gift:
                        if (reward.select < reward.give && Rogue.role == Rogue.giftname.test && Rogue.Instance.inRogue) reward.select++;
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
                        if (reward.select < reward.gifts.Count && Rogue.Instance.inRogue && Rogue.role == Rogue.giftname.test && Rogue.getBirthright) reward.select++;
                        FixSelectSmall(reward.gifts, reward.select);
                        break;
                    default:
                        break;
                }
            }
        }

    }

    private void Log(object msg)
    {
        Modding.Logger.Log(msg);
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
            Log("num is " + num);
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

        if (Rogue.role != null)
        {
            GameObject role = new("role");
            role.layer = LayerMask.NameToLayer("UI");
            role.transform.SetParent(inventory_display_holder.transform);
            role.transform.localPosition = new Vector3(-14.3f, 7.6f, 0);
            var role_render = role.AddComponent<SpriteRenderer>();
            if (Rogue.Instance.shopRewards[Rogue.role.Value].sprite != null)
            {
                role_render.sprite = Rogue.Instance.shopRewards[Rogue.role.Value].sprite;
            }
            else if (Rogue.Instance.shopRewards[Rogue.role.Value].getSprite != null)
            {
                role_render.sprite = Rogue.Instance.shopRewards[Rogue.role.Value].getSprite(Rogue.role.Value);
            }
            else
            {
                role_render.sprite = null;
            }
            if (Rogue.Instance.shopRewards[Rogue.role.Value].scale != Vector2.zero)
                role.transform.localScale = Rogue.Instance.shopRewards[Rogue.role.Value].scale;
            role_render.color = new Color(1, 1, 1, alpha);
            if (Rogue.role == Rogue.giftname.test)
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
        egg_num.GetComponent<TMPro.TextMeshPro>().text = Rogue.Instance.relive_num.ToString();
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
