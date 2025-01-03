



namespace rogue.Characters;
internal class Uunn : Character
{
    public Uunn()
    {
        this.Selfname = CharacterRole.uunn;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.uunn;
        PlayerData.instance.charmSlots += 2;
        if (PlayerData.instance.charmSlots > 11) PlayerData.instance.charmSlots = 11;
        PlayerData.instance.gotCharm_7 = true;
        PlayerData.instance.gotCharm_28 = true;
        ItemManager.Instance.after_scene_add_geo_num += UunnReward;
        GetBirthright(0);
        GetBirthright(1);
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:

                On.PlayMakerFSM.OnEnable += BiggerSpore;
                break;
            case 1:
                ModHooks.GetPlayerIntHook += InfinityBlocker;
                HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects").
                                                    FindGameObjectInChildren("Blocker Shield").LocateMyFSM("Control")
                                                    .InsertCustomAction("Blocker Hit", DamageAll, 0);
                break;
        }
    }







    private void BiggerSpore(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.gameObject.name.Contains("Knight Spore Cloud") && self.FsmName == "Control")
        {
            self.RemoveAction("Recycle", 1);
            self.AddAction("Recycle", new DestroySelf());
            self.GetAction<SetScale>("Normal", 1).x = 2f;
            self.GetAction<SetScale>("Normal", 1).y = 2f;
            self.GetAction<SetScale>("Deep", 1).x = 2.7f;
            self.GetAction<SetScale>("Deep", 1).x = 2.7f;
            var d = self.gameObject.GetComponent<DamageEffectTicker>();
            d.damageInterval /= 5;
        }
        else if (self.gameObject.name.Contains("Knight Dung Cloud") && self.FsmName == "Control")
        {
            var d = self.gameObject.GetComponent<DamageEffectTicker>();
            if (d == null || d.extraDamageType != ExtraDamageTypes.Dung2) ;
            else
            {
                self.RemoveAction("Recycle", 1);
                self.AddAction("Recycle", new DestroySelf());
                self.GetAction<SetScale>("Normal", 1).x = 2f;
                self.GetAction<SetScale>("Normal", 1).y = 2f;
                self.GetAction<SetScale>("Deep", 1).x = 2.7f;
                self.GetAction<SetScale>("Deep", 1).x = 2.7f;
                d.damageInterval /= 5;
            }
        }
        orig(self);
    }

    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                On.PlayMakerFSM.OnEnable -= BiggerSpore;
                break;
            case 1:
                ModHooks.GetPlayerIntHook -= InfinityBlocker;
                var ctl = HeroController.instance.gameObject.FindGameObjectInChildren("Charm Effects").
                                                    FindGameObjectInChildren("Blocker Shield").LocateMyFSM("Control");
                if (ctl.GetState("Blocker Hit").Actions.Length > 7)
                    ctl.RemoveAction("Blocker Hit", 0);
                break;
        }
    }

    private int InfinityBlocker(string name, int orig)
    {
        if (name == "blockerHits") return 3;
        return orig;
    }
    void DamageAll()
    {
        foreach (var enemy in UnityEngine.Object.FindObjectsOfType<HealthManager>())
        {
            try
            {
                enemy.Hit(new HitInstance()
                {
                    Source = base.gameObject,
                    AttackType = AttackTypes.Generic,
                    CircleDirection = false,
                    DamageDealt = 5 * PlayerData.instance.nailDamage,
                    Direction = HeroController.instance.transform.localScale.x < 0 ? 180f : 0,
                    IgnoreInvulnerable = false,
                    MagnitudeMultiplier = 0,
                    MoveAngle = 0f,
                    MoveDirection = false,
                    Multiplier = 1f,
                    SpecialType = SpecialTypes.None,
                    IsExtraDamage = false
                });
            }
            catch (Exception ex)
            {

            }
        }
        GameManager.instance.FreezeMoment(0);
    }
    private int UunnReward(int geo, int damage_num)
    {
        if (damage_num == 0) geo += 50;
        return geo;
    }

    public override void EndCharacter()
    {
        ItemManager.Instance.after_scene_add_geo_num -= UunnReward;
    }
}