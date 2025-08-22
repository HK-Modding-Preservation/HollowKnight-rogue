
using System.Runtime.Serialization.Formatters.Binary;


namespace rogue.Characters;

internal class NailMaster : Character
{
    internal const string hk_scene = "GG_Hollow_Knight";
    internal const string hk_name = "Battle Scene/HK Prime";
    internal static GameObject hk_shot;
    const float default_charge_time = 1.25f;
    const float charm_charge_time = 0.75f;
    const float default_speed_charge_time = 0.75f;
    const float charm_speed_charge_time = 0.35f;
    public NailMaster()
    {
        this.Selfname = CharacterRole.nail_master;
        AddBirthRight("nail_master_birthright_0_name".Localize());
        AddBirthRight("nail_master_birthright_1_name".Localize());
        AddBirthRight("nail_master_birthright_2_name".Localize());
    }
    internal static void Init()
    {
        hk_shot = Instantiate(PreloadManager.getGO(hk_scene, hk_name).
                  LocateMyFSM("Control").
                  GetAction<FlingObjectsFromGlobalPoolTime>("SmallShot LowHigh", 2).
                  gameObject.Value);
        hk_shot.SetActive(false);
        hk_shot.RemoveComponent<DamageHero>();
        hk_shot.GetComponent<AudioSource>().enabled = false;
        var shot_de = NailMaster.hk_shot.AddComponent<DamageEnemies>();
        shot_de.attackType = AttackTypes.Nail;
        shot_de.damageDealt = 20;
        shot_de.ignoreInvuln = false;
        hk_shot.layer = LayerMask.NameToLayer("Attack");
        DontDestroyOnLoad(hk_shot);
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.nail_master;
        GiftHelper.AddNailDamage();
        GiftHelper.max_nail_damage += HigherNailLevel;
        PlayerData.instance.hasCyclone = true;
        PlayerData.instance.hasDashSlash = true;
        PlayerData.instance.hasUpwardSlash = true;
        PlayerData.instance.hasNailArt = true;
        PlayerData.instance.hasDash = true;
        PlayerData.instance.canDash = true;
        PlayerData.instance.gotCharm_26 = true;
        Rogue.Instance.ShowDreamConvo("nail_master_dream".Localize());

    }

    private bool HigherNailLevel(bool orig)
    {
        return PlayerData.instance.nailDamage >= 25;
    }

    AudioClip nail_charge_ready;
    int nail_charge_level;
    private int NailChargeLevel
    {
        get { return nail_charge_level; }
        set
        {
            if (value <= nail_charge_level) return;
            else
            {
                nail_charge_level = value;
            }
        }
    }
    GameObject accu_nail_art_effect = null;
    GameObject great_slash1;
    GameObject great_slash2;
    int shot_count = 0;





    public override void EndCharacter()
    {
        GiftHelper.max_nail_damage -= HigherNailLevel;
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = default_speed_charge_time;
                HeroController.instance.NAIL_CHARGE_TIME_CHARM = charm_speed_charge_time;
                if (PlayerData.instance.equippedCharm_26)
                {
                    ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "nailChargeTime", charm_speed_charge_time);
                }
                else
                {
                    ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "nailChargeTime", default_charge_time);
                }

                break;
            case 1:
                On.PlayMakerFSM.OnEnable += InverWhenNailArt;
                On.PlayMakerFSM.OnDisable += DisInverWhenNailArtEnd;
                break;
            case 2:
                if (accu_nail_art_effect != null)
                {
                    GameObject.DestroyImmediate(accu_nail_art_effect);
                }
                accu_nail_art_effect = new("accu_nail_art_effect");
                accu_nail_art_effect.transform.SetParent(HeroController.instance.gameObject.FindGameObjectInChildren("Attacks")?.transform, false);
                accu_nail_art_effect.AddComponent<AudioSource>();
                great_slash1 = Instantiate(HeroController.instance.gameObject.
                                                    FindGameObjectInChildren("Attacks").
                                                    FindGameObjectInChildren("Great Slash"));
                great_slash1.name = "great_slash1";
                great_slash2 = Instantiate(great_slash1);
                great_slash2.name = "great_slash2";
                great_slash1.transform.SetParent(accu_nail_art_effect.transform, false);
                great_slash2.transform.SetParent(accu_nail_art_effect.transform, false);
                nail_charge_ready = HeroController.instance.nailArtChargeComplete;
                On.HeroController.Update += CheckNailLevel;
                On.HeroController.FixedUpdate += CycloneEffect;
                On.PlayMakerFSM.OnEnable += SetNailArtLevel;
                On.PlayMakerFSM.OnDisable += ResetNailArtLevel;
                HeroController.instance.gameObject.LocateMyFSM("Nail Arts").InsertCustomAction("Inactive", () => { nail_charge_level = 0; }, 1);

                break;

        }
    }

    private void CycloneEffect(On.HeroController.orig_FixedUpdate orig, HeroController self)
    {
        orig(self);
        var nailArt_cyclone = ReflectionHelper.GetField<HeroController, bool>(self, "nailArt_cyclone");
        if (nailArt_cyclone)
        {
            if (nail_charge_level >= 3)
            {
                if (InputHandler.Instance.inputActions.up.IsPressed)
                {
                    var rig = self.gameObject.GetComponent<Rigidbody2D>();
                    var vel = rig.velocity;
                    vel.y = 7.5f;
                    rig.velocity = vel;
                }

            }
            shot_count++;
            if (shot_count > 5)
            {
                shot_count = 0;
                float angle;
                if (nail_charge_level >= 2)
                {
                    angle = UnityEngine.Random.Range(-5f, 5f);
                    ShotNail(angle, nail_charge_level);
                }
                if (nail_charge_level >= 3)
                {
                    angle = UnityEngine.Random.Range(-5f, 5f);
                    ShotNail(angle, nail_charge_level);
                }
            }
        }

    }
    void ShotNail(float angle, int level)
    {
        float mul = 1;
        if (level == 2) mul = 1.25f;
        if (level == 3) mul = 1.75f;
        var shot = Instantiate(hk_shot);
        shot.transform.position = base.transform.position + new Vector3(3, 0, 0);
        shot.GetComponent<Rigidbody2D>().gravityScale = 0;
        shot.GetComponent<DamageEnemies>().damageDealt = (int)mul * PlayerData.instance.nailDamage;
        if (level == 3) shot.GetComponent<tk2dSprite>().color = Color.black;
        shot.SetActive(true);
        shot.GetComponent<Rigidbody2D>().velocity = new Vector2(30 * Mathf.Cos(angle * Mathf.PI / 180), 30 * Mathf.Sin(angle * Mathf.PI / 180));
        var shot2 = Instantiate(hk_shot);
        shot2.transform.position = base.transform.position + new Vector3(-3, 0, 0);
        shot2.GetComponent<Rigidbody2D>().gravityScale = 0;
        shot2.transform.SetRotation2D(90f);
        shot2.GetComponent<DamageEnemies>().damageDealt = (int)mul * PlayerData.instance.nailDamage;
        if (level == 3) shot2.GetComponent<tk2dSprite>().color = Color.black;
        shot2.SetActive(true);
        shot2.GetComponent<Rigidbody2D>().velocity = new Vector2(-30 * Mathf.Cos(angle * Mathf.PI / 180), -30 * Mathf.Sin(angle * Mathf.PI / 180));
    }

    private void ResetNailArtLevel(On.PlayMakerFSM.orig_OnDisable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "nailart_damage")
        {
            if (self.gameObject.name == "Great Slash")
            {

            }
            else if (self.gameObject.name == "Dash Slash")
            {
                On.HealthManager.TakeDamage -= DrawNailSlash;
                On.PlayMakerFSM.OnEnable -= ChangeRotation;
            }
            else if (self.gameObject.name.Contains("Hit"))
            {
                self.gameObject.LocateMyFSM("nailart_damage").FsmVariables.FindFsmFloat("Multiplier").Value = 1.25f;
            }

        }
        orig(self);
    }

    private void SetNailArtLevel(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "nailart_damage")
        {
            if (self.gameObject.name == "Great Slash")
            {
                ItemManager.Instance.StartCoroutine(DelayGreatSlash(nail_charge_level));
            }
            else if (self.gameObject.name == "Dash Slash")
            {
                On.HealthManager.TakeDamage += DrawNailSlash;
                On.PlayMakerFSM.OnEnable += ChangeRotation;
            }
            else if (self.gameObject.name.Contains("Hit"))
            {
                if (nail_charge_level == 3)
                    self.gameObject.LocateMyFSM("nailart_damage").FsmVariables.FindFsmFloat("Multiplier").Value = 1.75f;
            }
        }
        orig(self);
    }

    private void ChangeRotation(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.gameObject.name.Contains("Sharp Shadow Impact") && self.FsmName == "Control")
        {
            self.GetAction<RandomFloat>("Activate", 1).min = -180f;
            self.GetAction<RandomFloat>("Activate", 1).max = 180f;
            self.InsertCustomAction("Recycle", (fsm) =>
            {
                fsm.GetAction<RandomFloat>("Activate", 1).min = -15f;
                fsm.GetAction<RandomFloat>("Activate", 1).max = 15f;
                fsm.RemoveAction("Recycle", 0);
            }, 0);

        }
        orig(self);
    }

    private void DrawNailSlash(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        orig(self, hitInstance);
        if (hitInstance.AttackType == AttackTypes.Nail)
        {
            ItemManager.Instance.StartCoroutine(DelayDashSlash(self, nail_charge_level));
        }
    }
    IEnumerator DelayDashSlash(HealthManager healthManager, int level)
    {
        int count = 0;
        if (level == 2) count = 3;
        if (level == 3) count = 5;
        yield return new WaitForSeconds(0.3f);
        while (count > 0)
        {
            if (healthManager != null && healthManager.gameObject != null)
            {
                yield return new WaitForSeconds(0.2f);
                healthManager.Hit(new HitInstance()
                {
                    Source = base.gameObject,
                    AttackType = AttackTypes.SharpShadow,
                    CircleDirection = false,
                    DamageDealt = PlayerData.instance.nailDamage,
                    Direction = HeroController.instance.transform.localScale.x < 0 ? 180f : 0,
                    IgnoreInvulnerable = true,
                    MagnitudeMultiplier = 0,
                    MoveAngle = 0f,
                    MoveDirection = false,
                    Multiplier = 1f,
                    SpecialType = SpecialTypes.None,
                    IsExtraDamage = false
                });
            }
            count--;

        }

    }
    IEnumerator DelayGreatSlash(int level)
    {
        if (level < 2) yield break;
        var gs = HeroController.instance.gameObject.FindGameObjectInChildren("Attacks").FindGameObjectInChildren("Great Slash");
        Color col = gs.GetComponent<tk2dSprite>().color;
        bool fury = gs.LocateMyFSM("nailart_damage").FsmVariables.FindFsmBool("Fury").Value;
        if (level >= 2)
        {
            yield return new WaitForSeconds(0.2f);
            great_slash1.GetComponent<tk2dSprite>().color = col;
            great_slash1.LocateMyFSM("nailart_damage").FsmVariables.FindFsmBool("Fury").Value = fury;
            great_slash1.LocateMyFSM("Control Collider").GetAction<SetScale>("Init", 2).y = -1.316208f;
            great_slash1.LocateMyFSM("Control Collider").GetAction<SetScale>("Deactivate", 3).y = -1.316208f;
            great_slash1.SetActive(true);
        }
        if (level >= 3)
        {
            yield return new WaitForSeconds(0.2f);
            great_slash2.GetComponent<tk2dSprite>().color = col;
            great_slash2.LocateMyFSM("nailart_damage").FsmVariables.FindFsmBool("Fury").Value = fury;
            great_slash2.SetActive(true);
        }

    }

    private void CheckNailLevel(On.HeroController.orig_Update orig, HeroController self)
    {
        orig(self);
        float nailChargeTime = ReflectionHelper.GetField<HeroController, float>(self, "nailChargeTime");
        float nailChargeTimer = ReflectionHelper.GetField<HeroController, float>(self, "nailChargeTimer");
        if (nailChargeTimer < nailChargeTime) NailChargeLevel = 0;
        if (nailChargeTimer > nailChargeTime) NailChargeLevel = 1;
        if (nailChargeTimer >= 2 * nailChargeTime)
        {
            if (nailChargeTimer - Time.deltaTime < 2 * nailChargeTime)
            {
                accu_nail_art_effect.GetComponent<AudioSource>().pitch = 1.5f;
                accu_nail_art_effect?.GetComponent<AudioSource>().PlayOneShot(nail_charge_ready, Setting.SoundScale());
                HeroController.instance.artChargedEffectAnim.PlayFromFrame(0);
            }
            NailChargeLevel = 2;
        }
        if (nailChargeTimer >= 3 * nailChargeTime)
        {
            if (nailChargeTimer - Time.deltaTime < 3 * nailChargeTime)
            {
                accu_nail_art_effect.GetComponent<AudioSource>().pitch = 2.0f;
                accu_nail_art_effect?.GetComponent<AudioSource>().PlayOneShot(nail_charge_ready, Setting.SoundScale());
                HeroController.instance.artChargedEffectAnim.PlayFromFrame(0);
            }
            NailChargeLevel = 3;
        }
    }

    private void DisInverWhenNailArtEnd(On.PlayMakerFSM.orig_OnDisable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "nailart_damage")
        {
            ("End No Damage").TestLog();

            HeroController.instance.SetDamageMode(0);
        }
        orig(self);
    }

    private void InverWhenNailArt(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "nailart_damage")
        {
            ("Begin No Damage").TestLog();
            (nail_charge_level).TestLog();
            HeroController.instance.SetDamageMode(1);
        }
        orig(self);
    }

    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = default_charge_time;
                HeroController.instance.NAIL_CHARGE_TIME_CHARM = charm_charge_time;
                break;
            case 1:
                On.PlayMakerFSM.OnEnable -= InverWhenNailArt;
                On.PlayMakerFSM.OnDisable -= DisInverWhenNailArtEnd;
                break;
            case 2:
                if (accu_nail_art_effect != null)
                {
                    GameObject.DestroyImmediate(accu_nail_art_effect);
                }
                On.HeroController.Update -= CheckNailLevel;
                On.HeroController.FixedUpdate -= CycloneEffect;
                On.PlayMakerFSM.OnEnable -= SetNailArtLevel;
                On.PlayMakerFSM.OnDisable -= ResetNailArtLevel;
                HeroController.instance.gameObject.LocateMyFSM("Nail Arts").RemoveAction("Inactive", 1);

                break;



        }
    }


}