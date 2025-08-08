using InControl;
using rogue.Characters;
using UnityEngine.EventSystems;

namespace rogue;

internal class DisplayManager : MonoBehaviour
{
    internal static DisplayManager instance = null;
    static float alpha = 0;
    internal static void Init()
    {
        if (instance != null) { Destroy(instance); }
        instance = Rogue.Instance.rogue_go.AddComponent<DisplayManager>();
    }

    static GameObject scorePanel = null;
    static GameObject inventory_display_holder = null;
    static GameObject rogue_display = null;
    static TMPro.TextMeshPro refresh_text_pro = null;

    static TMPro.TextMeshPro time_display_text_pro = null;

    static TMPro.TextMeshPro scoreText;
    internal static TMPro.TMP_FontAsset all_font = null;
    TimeSpan timeSpan = TimeSpan.Zero;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) { ShowScore(); }
        if (Input.GetKeyUp(KeyCode.Tab)) { HideScore(); }
        // if (Input.GetKeyUp(KeyCode.End))
        // {
        //     BossSceneController.Instance?.EndBossScene();
        //     ItemManager.Instance.StopCoroutine("EnemyWaves");
        // }
        // if (Input.GetKeyUp(KeyCode.Home)) { BossManager.bossleft = 0; }
        if (GameInfo.in_rogue && BossSequenceController.IsInSequence && (ProcessManager.scene_name != "GG_Spa") && (ProcessManager.scene_name! != "GG_Atrium_Roof" || ProcessManager.scene_name != "GG_Engine"))
        {
            if (!GameManager.instance.isPaused)
            {
                GameInfo.data.timer += Time.unscaledDeltaTime;
                timeSpan = TimeSpan.FromSeconds(GameInfo.data.timer);
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

    private string TimerText()
    {
        return string.Format(
            "\n{0}:{1:D2}:{2:D3}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds
            );
    }

    private static void Log(object msg)
    {

        Rogue.Instance.Log(msg);

    }

    static void GetFont()
    {
        if (all_font == null)
        {
            try
            {
                all_font = HutongGames.PlayMaker.FsmVariables.GlobalVariables.FindFsmGameObject("Inventory").Value.FindGameObjectInChildren("Inv").FindGameObjectInChildren("Equipment").FindGameObjectInChildren("Rancid Egg").FindGameObjectInChildren("Egg Amount").GetComponent<TMPro.TextMeshPro>().font;
            }
            catch (Exception ex)
            {
                Log("字体尚未找到");
            }
        }
    }
    internal static void BeginDisplay()
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
        GetFont();

        GameObject time_display = new("time_display");
        time_display.transform.SetParent(rogue_display.transform);
        time_display.layer = LayerMask.NameToLayer("UI");
        time_display.transform.localPosition = new Vector3(-5.1f, -0.6f, 0);
        time_display_text_pro = time_display.AddComponent<TMPro.TextMeshPro>();
        time_display_text_pro.fontSize = 6;
        time_display_text_pro.color = new Color(1, 1, 1, alpha);
        if (all_font != null) time_display_text_pro.font = all_font;


    }

    internal static void DisplayEquipped()
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
        _GameCameras = GameCameras.instance.gameObject;
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

    internal static void DisplayStates()
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
        else if (PlayerData.instance.fireballLevel >= 2)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Fireball").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 2", 0).sprite.Value;
        }
        render.color = new Color(1, 1, 1, alpha);
        if (PlayerData.instance.fireballLevel == 3) render.color = new Color(0, 0, 1, alpha);
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
        else if (PlayerData.instance.screamLevel >= 2)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Scream").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 2", 0).sprite.Value;
        }
        render.color = new Color(1, 1, 1, alpha);
        if (PlayerData.instance.screamLevel == 3) render.color = new Color(0, 0, 1, alpha);
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
        else if (PlayerData.instance.quakeLevel >= 2)
        {
            render.sprite = (Sprite)Inv_Items.FindGameObjectInChildren("Spell Quake").LocateMyFSM("Check Active").GetAction<SetSpriteRendererSprite>("Lv 2", 0).sprite.Value;
        }
        render.color = new Color(1, 1, 1, alpha);
        if (PlayerData.instance.quakeLevel == 3) render.color = new Color(0, 0, 1, alpha);
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







    static void InitScorePanel()
    {
        if (scorePanel == null)
        {
            scorePanel = new GameObject("ScorePanel");
            GameObject.DontDestroyOnLoad(scorePanel);
            scorePanel.transform.SetPosition2D(0f, 8f);
            scoreText = scorePanel.AddComponent<TMPro.TextMeshPro>();
            scorePanel.layer = LayerMask.NameToLayer("UI");
            scoreText.fontSize = 8;
            scoreText.color = Color.white;
            scoreText.alignment = TMPro.TextAlignmentOptions.Baseline;
        }
    }

    internal static void ShowScore()
    {
        InitScorePanel();
        if (GameInfo.data == null) return;
        scorePanel.SetActive(true);
        scoreText.text = $"Score: {GameInfo.data.score:F2}";
        if (Rogue.Instance._set.details)
        {
            scoreText.text += $"\nTime: {GameInfo.data.timer:F2} ";
            scoreText.text += $"\nAttack: {GameInfo.data.num_attack}";
            scoreText.text += $"\nNailArt: {GameInfo.data.num_nail_arts}";
            scoreText.text += $"\nSpell: {GameInfo.data.num_spells}";
            scoreText.text += $"\nInjured: {GameInfo.data.num_injured}";
            scoreText.text += $"\nHitted: {GameInfo.data.num_hitted}";
        }
    }
    internal static void HideScore()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }

    }




}