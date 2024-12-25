

namespace rogue.Characters;
internal class Collector : Character
{
    readonly List<int> spawn_charms = new List<int>() { 22, 38, 39, 40 };
    public Collector()
    {
        this.Selfname = CharacterRole.collector;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.collector;
        PlayerData.instance.gotCharm_40 = true;
        PlayerData.instance.grimmChildLevel = 4;
        PlayerData.instance.charmCost_40 = 2;
        PlayerData.instance.gotCharm_38 = true;
        PlayerData.instance.equippedCharm_38 = true;
        PlayerData.instance.equippedCharms.Add(38);
        CharmHelper.SetCantUnequip(38);
        Rogue.Instance.ShowDreamConvo("collector_dream".Localize());
        On.PlayerData.GetInt += FreeSpawnCharm;
        On.KnightHatchling.OnEnable += OnHatchingDamage;
        On.KnightHatchling.DoChaseSimple += OnHatchingChaseSimple;
        On.KnightHatchling.DoChase += OnHatchingChase;
        On.PlayMakerFSM.OnEnable -= ChangeGrimm;
        On.PlayMakerFSM.OnEnable += ChangeGrimm;
        ModHooks.CharmUpdateHook += ChangeHatchOnCharmUpdate;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ChangeHatchOnSceneChanged;
    }
    public override void EndCharacter()
    {
        CharmHelper.SetCanEquip(38);
        On.PlayerData.GetInt -= FreeSpawnCharm;
        On.KnightHatchling.OnEnable -= OnHatchingDamage;
        On.KnightHatchling.DoChaseSimple -= OnHatchingChaseSimple;
        On.KnightHatchling.DoChase -= OnHatchingChase;
        // On.PlayMakerFSM.OnEnable -= ChangeGrimm;
        ModHooks.CharmUpdateHook -= ChangeHatchOnCharmUpdate;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ChangeHatchOnSceneChanged;
        HatchRollBack();
    }
    protected override void DamageMul(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        nail_mul = 1;
        spell_mul = 1;
        foreach (var charm in spawn_charms)
        {
            if (PlayerData.instance.equippedCharms.Contains(charm))
            {
                nail_mul *= 0.5f;
                spell_mul *= 0.5f;
            }
        }

        base.DamageMul(orig, self, hitInstance);
    }

    private void ChangeGrimm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "Control" && self.gameObject.name.Contains("Grimmchild"))
        {
            if (GameInfo.in_rogue && GameInfo.role == CharacterRole.collector)
            {
                int naillevel = (PlayerData.instance.nailDamage - 5) / 4;
                float damage = 10;
                damage += naillevel;
                if (PlayerData.instance.equippedCharm_25) damage *= 1.5f;
                if (PlayerData.instance.equippedCharm_6 && PlayerData.instance.health == 1) damage *= 1.75f;
                if (PlayerData.instance.equippedCharm_22) damage *= 0.7f;
                self.GetAction<SetIntValue>("Level 4", 0).intValue = (int)damage;


                self.GetAction<Wait>("Follow", 18).time = 0.25f;
                self.GetAction<SetFloatValue>("No Target", 0).floatValue = 0.3f;
                float attack_timer = 0.79f;
                float attack_speed = 1f;

                attack_speed += naillevel * 0.5f;
                if (PlayerData.instance.equippedCharm_32) attack_speed *= 1.5f;
                self.GetAction<RandomFloat>("Antic", 3).min = attack_timer / attack_speed;
                self.GetAction<RandomFloat>("Antic", 3).max = attack_timer / attack_speed;
                float anticfps = 16;
                float shootfps = 12;
                self.gameObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Shoot 4").fps = shootfps * attack_speed;
                self.gameObject.GetComponent<tk2dSpriteAnimator>().Library.GetClipByName("Antic 4").fps = anticfps * attack_speed;




                float speed = 30f;
                if (PlayerData.instance.equippedCharm_31) speed += 10f;
                if (PlayerData.instance.equippedCharm_37) speed += 10f;
                self.FsmVariables.FindFsmFloat("Flameball Speed").Value = speed;
                if (PlayerData.instance.equippedCharm_2) self.GetAction<FireAtTarget>("Shoot", 7).spread = 0;



                if (self.GetState("Shoot").Actions.Length == 10)
                {
                    self.InsertCustomAction("Shoot", (fsm) =>
                    {
                        if (PlayerData.instance.equippedCharm_22)
                        {
                            var ball = fsm.FsmVariables.FindFsmGameObject("Flameball").Value;
                            var rb2d = ball.GetComponent<Rigidbody2D>();
                            float speed = fsm.FsmVariables.FindFsmFloat("Flameball Speed").Value;
                            float num3 = Mathf.Acos(rb2d.velocity.x / speed) / ((float)Math.PI / 180f);
                            float[] spreads = new float[] { -35f, 35f };
                            foreach (var spread in spreads)
                            {
                                GameObject newball = Instantiate(ball);
                                newball.transform.position = ball.transform.position;
                                float newnum3 = num3 + spread;
                                var x = speed * Mathf.Cos(newnum3 * ((float)Math.PI / 180f));
                                var y = speed * Mathf.Sin(newnum3 * ((float)Math.PI / 180f));
                                Vector2 velocity = default(Vector2);
                                velocity.x = x;
                                velocity.y = y;
                                newball.GetComponent<Rigidbody2D>().velocity = velocity;
                            }

                        }
                    }, 8);
                }

                GameObject grimm = GameObject.Find("Grimmchild(Clone)");
                if (grimm != null)
                {
                    float scale = 1f;
                    if (GameInfo.get_birthright) scale += 0.5f;
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
                if (PlayerData.instance.equippedCharm_2) self.GetAction<FireAtTarget>("Shoot", 7).spread = 15f;

                if (self.GetState("Shoot").Actions.Length == 11)
                {
                    self.RemoveAction("Shoot", 8);
                }

                GameObject grimm = GameObject.Find("Grimmchild(Clone)");
                if (grimm != null)
                {
                    grimm.FindGameObjectInChildren("Enemy Range").transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
        else if (self.FsmName == "Control" && self.gameObject.name.Contains("Weaverling"))
        {
            if (GameInfo.in_rogue && GameInfo.role == CharacterRole.collector)
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
            if (GameInfo.in_rogue && GameInfo.role == CharacterRole.collector)
            {
                float damage = 3;
                damage += PlayerData.instance.nailDamage * 1.0f / 8;
                if (PlayerData.instance.equippedCharm_25) damage *= 1.5f;
                if (PlayerData.instance.equippedCharm_6 && PlayerData.instance.health == 1) damage *= 1.75f;
                self.FsmVariables.FindFsmInt("Damage").Value = (int)damage;

                int mp_get = GameInfo.get_birthright ? 5 : 3;
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
            if (GameInfo.in_rogue && GameInfo.role == CharacterRole.collector)
            {
                if (self.GetState("Init").Actions.Length == 10)
                {
                    self.InsertCustomAction("Init", (fsm) =>
                    {
                        float damage = PlayerData.instance.nailDamage;
                        if (GameInfo.get_birthright) damage += 5;
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
            if (GameInfo.in_rogue && GameInfo.role == CharacterRole.collector)
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
            if (GameInfo.in_rogue && GameInfo.role == CharacterRole.collector)
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

    }

    private void ChangeHatchOnSceneChanged(Scene arg0, Scene arg1)
    {
        HatchChange();
    }

    private void ChangeHatchOnCharmUpdate(PlayerData data, HeroController controller)
    {
        HatchChange();
    }

    private void HatchRollBack()
    {
        GameObject charmeffect = GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects");
        var fsm = charmeffect.LocateMyFSM("Hatchling Spawn");
        var spell_con = GameObject.Find("Knight").LocateMyFSM("Spell Control");
        fsm.FsmVariables.FindFsmInt("Soul Cost").Value = 8;
        fsm.FsmVariables.FindFsmInt("Hatchling Max").Value = 4;
        fsm.FsmVariables.FindFsmFloat("Hatch Time").Value = 4;
        fsm.ChangeTransition("Equipped", "FINISHED", "Can Hatch?");
        spell_con.ChangeTransition("Fireball Antic", "ANIM END", "Level Check");
    }
    private void HatchChange()
    {
        GameObject charmeffect = GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects");
        var fsm = charmeffect.LocateMyFSM("Hatchling Spawn");
        var spell_con = GameObject.Find("Knight").LocateMyFSM("Spell Control");
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
        int new_spelllevel = Math.Max(Math.Max(PlayerData.instance.fireballLevel, PlayerData.instance.quakeLevel), PlayerData.instance.screamLevel);
        hatch_speed += 0.9f * new_spelllevel;
        if (PlayerData.instance.equippedCharm_32) hatch_speed *= 1.5f;
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



    private int FreeSpawnCharm(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
    {
        if (intName == "charmCost_38") return 0;
        if (intName == "charmCost_39") return 0;
        if (intName == "charmCost_40") return 0;
        if (intName == "charmCost_22") return 0;
        return orig(self, intName);
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
    private void OnHatchingDamage(On.KnightHatchling.orig_OnEnable orig, KnightHatchling self)
    {
        //每次enable都会重新置伤害所以无影响
        orig(self);
        GameObject charmeffect = GameObject.Find("Knight").FindGameObjectInChildren("Charm Effects");
        var fsm = charmeffect.LocateMyFSM("Hatchling Spawn");
        var de = ReflectionHelper.GetField<KnightHatchling, KnightHatchling.TypeDetails>(self, "details");
        float damage = de.damage + 20 - 9;
        int spelllevel = Math.Max(Math.Max(PlayerData.instance.fireballLevel, PlayerData.instance.screamLevel), PlayerData.instance.quakeLevel);
        damage += 5 * spelllevel;
        if (PlayerData.instance.equippedCharm_19) damage += damage / 3;
        if (PlayerData.instance.equippedCharm_11) damage *= 0.7f;
        if (PlayerData.instance.equippedCharm_40) damage *= 0.7f;
        de.damage = (int)damage;
        if (GameInfo.get_birthright && ItemManager.Instance.scenename != "GG_Spa")
        {
            self.damageEnemies.attackType = AttackTypes.SharpShadow;
        }
        else
        {
            self.damageEnemies.attackType = AttackTypes.Generic;
        }

        ReflectionHelper.SetField(self, "details", de);

    }




}