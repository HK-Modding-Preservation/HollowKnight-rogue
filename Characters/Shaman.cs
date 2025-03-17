
using System.Security.Cryptography;

namespace rogue.Characters;
internal class Shaman : Character
{
    internal static GameObject beam;
    internal static GameObject butterfly;
    Color spell_color = Color.blue;
    public Shaman()
    {
        this.Selfname = CharacterRole.shaman;
        nail_mul = 0.8f;
        spell_mul = 1.25f;
        birthright_names = new()
        {
            "砸",
            "吼",
            "萨满之力"
        };
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.shaman;
        PlayerData.instance.gotCharm_19 = true;
        GameInfo.refresh_num += 2;
        Rogue.Instance.ShowDreamConvo("shaman_dream".Localize());
        On.SpellFluke.OnEnable += OnFlukeDamage;
        AddFireball3();
        AddQuake3();
        AddScream3();
        GiftFactory.after_update_weight += CanUpdateSpellLevel;
        beam.transform.SetParent(HeroController.instance.transform);
        beam.transform.localPosition = new Vector3(0, -1.4f, 0);
        beam.transform.localScale = new Vector3(10, 5, 0);
        beam.transform.SetRotation2D(90f);
        beam.GetComponent<tk2dSprite>().color = new Color(0, 0, 1, 0.5f);
        beam.SetActive(true);
        PlayerData.instance.fireballLevel = 2;
        PlayerData.instance.screamLevel = 2;
        PlayerData.instance.quakeLevel = 2;


    }

    private void CanUpdateSpellLevel()
    {
        GiftFactory.all_gifts[Giftname.get_fireball].SetWeight(PlayerData.instance.fireballLevel != 3);
        GiftFactory.all_gifts[Giftname.get_scream].SetWeight(PlayerData.instance.screamLevel != 3);
        GiftFactory.all_gifts[Giftname.get_quake].SetWeight(PlayerData.instance.quakeLevel != 3);
    }

    public override void EndCharacter()
    {
        On.SpellFluke.OnEnable -= OnFlukeDamage;
        RemoveFireball3();
        RemoveQuake3();
        RemoveScream3();
        GiftFactory.after_update_weight -= CanUpdateSpellLevel;
        beam.transform.SetParent(null);
        GameObject.DontDestroyOnLoad(beam);
        beam.SetActive(false);
    }
    private void FireballLifeBloodify(PlayMakerFSM fbc)
    {
        fbc.RemoveAction("Recycle", 0);
        fbc.AddAction("Recycle", new DestroySelf());
        fbc.gameObject.GetComponent<tk2dSprite>().color = spell_color;
        fbc.GetAction<Wait>("Idle", 0).time = 4f;
        fbc.GetAction<SetScale>("Set Damage", 0).x = 2.5f;
        fbc.GetAction<SetScale>("Set Damage", 0).y = 2.5f;
        fbc.GetAction<SetFsmInt>("Set Damage", 3).setValue = 40;
        fbc.GetAction<SetFsmInt>("Set Damage", 5).setValue = 55;
        if (fbc.ActiveStateName != "Set Damage")
            fbc.SetState("Set Damage");
    }
    private void AddFireball3()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        if (fsm.GetState("Fireball 3") == null)
        {

            var fireball3 = fsm.CopyState("Fireball 2", "Fireball 3");
            fsm.AddTransition("Level Check", "LEVEL 3", "Fireball 3");
            fsm.InsertCustomAction("Level Check", (fsm) => { if (PlayerData.instance.fireballLevel == 3) fsm.SendEvent("LEVEL 3"); }, 0);
            fireball3.InsertCustomAction((state) =>
            {
                var top = state.Fsm.GetFsmGameObject("Fireball Top").Value;
                top.LocateMyFSM("Fireball Cast").RemoveAction("Recycle", 0);
                top.LocateMyFSM("Fireball Cast").AddAction("Recycle", new DestroySelf());
                top.LocateMyFSM("Fireball Cast").FsmVariables.GetFsmFloat("Fire Speed").Value = 15;
                top.LocateMyFSM("Fireball Cast").InsertCustomAction("Cast Right", (fsm) =>
                {
                    var fbc = fsm.FsmVariables.FindFsmGameObject("Fireball").Value.LocateMyFSM("Fireball Control");
                    FireballLifeBloodify(fbc);
                }, 5);
                top.LocateMyFSM("Fireball Cast").InsertCustomAction("Cast Left", (fsm) =>
                {
                    var fbc = fsm.FsmVariables.FindFsmGameObject("Fireball").Value.LocateMyFSM("Fireball Control");
                    FireballLifeBloodify(fbc);
                }, 5);


            }, 4);
        }
        else
        {
            fsm.InsertCustomAction("Level Check", (fsm) => { if (PlayerData.instance.fireballLevel == 3) fsm.SendEvent("LEVEL 3"); }, 0);
        }
    }
    private void RemoveFireball3()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");

        fsm.RemoveAction("Level Check", 0);

    }
    void AddQuake3()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        fsm.InsertCustomAction("Level Check 2", () =>
        {
            if (PlayerData.instance.quakeLevel == 3)
            {

                SetQuake3();
                Shaman.beam.LocateMyFSM("Control").SendEvent("ANTIC");
            }
            else
            {
                SetNoQuake3();
            }
        }, 0);
        fsm.InsertCustomAction("Q2 Land", () =>
        {
            if (PlayerData.instance.quakeLevel == 3)
            {
                butterfly.SetActive(false);
                butterfly.transform.position = HeroController.instance.transform.position;
                butterfly.SetActive(true);
                Shaman.beam.LocateMyFSM("Control").SendEvent("FIRE");
            }
        }, 15);
        fsm.InsertCustomAction("Quake Finish", () =>
        {
            SetNoQuake3();
            Shaman.beam.LocateMyFSM("Control").SendEvent("END");
        }, 3);
    }
    void RemoveQuake3()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        fsm.RemoveAction("Level Check 2", 0);
        fsm.RemoveAction("Quake Finish", 3);
    }
    void AddScream3()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        fsm.InsertCustomAction("Level Check 3", () =>
        {
            if (PlayerData.instance.screamLevel == 3)
            {
                SetScream3();
            }
            else
            {
                SetNoScream3();
            }
        }, 0);
        fsm.InsertCustomAction("Scream End 2", () =>
        {
            SetNoScream3();
        }, 0);
    }
    void RemoveScream3()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        fsm.RemoveAction("Level Check 3", 0);
        fsm.RemoveAction("Scream End 2", 0);
    }
    void SetScream3()
    {

        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        fsm.InsertAction("Scream End 2", new Wait() { time = 0.5f }, 2);
        fsm.GetAction<SendMessage>("Scream Antic2", 0).Enabled = false;
        fsm.GetAction<SetVelocity2d>("Scream Antic2", 5).Enabled = false;
        fsm.GetAction<SetVelocity2d>("Scream Burst 2", 10).Enabled = false;
        var spells = HeroController.instance.gameObject.FindGameObjectInChildren("Spells");
        spells.FindGameObjectInChildren("Scr Heads 2").GetComponent<tk2dSprite>().color = spell_color;
        List<GameObject> childs = new();
        spells.FindGameObjectInChildren("Scr Heads 2").FindAllChildren(childs);
        foreach (var child in childs)
        {
            child.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 25;
            child.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 35;
        }
    }
    void SetNoScream3()
    {

        var fsm = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
        var wait = fsm.GetAction<Wait>("Scream End 2", 2);
        if (wait != null)
        {
            fsm.RemoveAction("Scream End 2", 2);
        }
        fsm.GetAction<SendMessage>("Scream Antic2", 0).Enabled = true;
        fsm.GetAction<SetVelocity2d>("Scream Antic2", 5).Enabled = true;
        fsm.GetAction<SetVelocity2d>("Scream Burst 2", 10).Enabled = true;
        var spells = HeroController.instance.gameObject.FindGameObjectInChildren("Spells");
        spells.FindGameObjectInChildren("Scr Heads 2").GetComponent<tk2dSprite>().color = Color.white;
        List<GameObject> childs = new();
        spells.FindGameObjectInChildren("Scr Heads 2").FindAllChildren(childs);
        foreach (var child in childs)
        {
            child.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 20;
            child.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 30;
        }
    }
    public void SetQuake3()
    {
        Log("Set Quake3");
        butterfly.SetActive(false);
        butterfly.transform.position = HeroController.instance.transform.position;
        butterfly.SetActive(true);
        HeroController.instance.INVUL_TIME_QUAKE = 0.5f;
        List<string> animes = new()
        {
            "Q Pillar",
            "Q Slam 2",
            "Q Trail 2",
            "Q Mega"
        };
        List<string> damages = new(){
            "Q Slam 2",
            "Q Mega",
            "Q Fall Damage"
        };

        var spells = HeroController.instance.gameObject.FindGameObjectInChildren("Spells");
        spells.FindGameObjectInChildren("Q Fall Damage").LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 30;
        spells.FindGameObjectInChildren("Q Fall Damage").LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 30;
        foreach (var anime in animes)
        {

            var ani_go = spells.FindGameObjectInChildren(anime);
            ani_go.GetComponent<tk2dSprite>().color = spell_color;
            if (damages.Contains(anime))
            {
                var hit_L = ani_go.FindGameObjectInChildren("Hit L");
                var hit_R = ani_go.FindGameObjectInChildren("Hit R");
                if (anime == "Q Slam 2")
                {
                    hit_L.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 40;
                    hit_L.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 60;
                    hit_R.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 40;
                    hit_R.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 60;
                }
                else if (anime == "Q Mega")
                {
                    hit_L.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 20;
                    hit_R.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 20;
                }
            }
        }
    }
    public void SetNoQuake3()
    {
        Log("Set No Quake3");

        HeroController.instance.INVUL_TIME_QUAKE = 0.4f;
        List<string> animes = new()
        {
            "Q Pillar",
            "Q Slam 2",
            "Q Trail 2",
            "Q Mega"
        };
        List<string> damages = new(){
            "Q Slam 2",
            "Q Mega",
            "Q Fall Damage"
        };

        var spells = HeroController.instance.gameObject.FindGameObjectInChildren("Spells");
        spells.FindGameObjectInChildren("Q Fall Damage").LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 15;
        spells.FindGameObjectInChildren("Q Fall Damage").LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 23;
        foreach (var anime in animes)
        {
            var ani_go = spells.FindGameObjectInChildren(anime);
            ani_go.GetComponent<tk2dSprite>().color = Color.white;
            if (damages.Contains(anime))
            {
                var hit_L = ani_go.FindGameObjectInChildren("Hit L");
                var hit_R = ani_go.FindGameObjectInChildren("Hit R");
                if (anime == "Q Slam 2")
                {
                    hit_L.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 35;
                    hit_L.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 50;
                    hit_R.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 0).setValue = 30;
                    hit_R.LocateMyFSM("Set Damage").GetAction<SetFsmInt>("Set Damage", 2).setValue = 50;
                }
                else if (anime == "Q Mega")
                {
                    hit_L.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 15;
                    hit_R.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 15;
                }
            }
        }
    }
    private void OnFlukeDamage(On.SpellFluke.orig_OnEnable orig, SpellFluke self)
    {
        //每次enable都会重新置伤害所以无影响
        orig(self);
        if (PlayerData.instance.equippedCharm_19)
        {
            ReflectionHelper.SetField<SpellFluke, int>(self, "damage", 6);
        }
        else
        {
            ReflectionHelper.SetField<SpellFluke, int>(self, "damage", 5);
        }
    }

    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.quakeLevel += 1;
                if (PlayerData.instance.quakeLevel > 3) PlayerData.instance.quakeLevel = 3;
                break;
            case 1:
                PlayerData.instance.screamLevel += 1;
                if (PlayerData.instance.screamLevel > 3) PlayerData.instance.screamLevel = 3;
                break;
            case 2:
                base.gameObject.FindGameObjectInChildren("Spells").transform.localScale = new(1.2f, 1.2f, 1);
                break;
        }
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
            case 1:
            case 2:
                gameObject.FindGameObjectInChildren("Spells").transform.localScale = new(1f, 1f, 1);
                break;
        }
    }
}