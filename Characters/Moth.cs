

namespace rogue.Characters;

internal class Moth : Character
{
    internal const string dream_sword_scene = "GG_Ghost_Markoth_V";
    internal const string markoth_name = "Warrior/Ghost Warrior Markoth";
    static GameObject dream_sword;
    internal static void Init()
    {
        dream_sword = Instantiate(PreloadManager.getGO(dream_sword_scene, markoth_name).
        LocateMyFSM("Attacking").GetAction<SpawnObjectFromGlobalPool>("Nail", 0).
        gameObject.Value);
        dream_sword.SetActive(false);
        dream_sword.RemoveComponent<DamageHero>();
        var sword_de = dream_sword.AddComponent<DamageEnemies>();
        sword_de.attackType = AttackTypes.Nail;
        sword_de.damageDealt = 20;
        sword_de.ignoreInvuln = false;
        dream_sword.layer = LayerMask.NameToLayer("Attack");
        DontDestroyOnLoad(dream_sword);

    }
    public Moth()
    {
        this.Selfname = CharacterRole.moth;
        AddBirthRight("弑梦");
        AddBirthRight("剑制");

    }


    int dream_mul = 0;
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.moth;
        PlayerData.instance.hasDreamNail = true;
        PlayerData.instance.hasDreamGate = true;
        GameInfo.get_any_charm_num += 1;
        GiftHelper.AddNailDamage();
        GiftHelper.GiveMask();
        GiftHelper.GiveVessel();
        GiftHelper.AddCharmSlot(8);
        PlayerData.instance.gotCharm_30 = true;
        On.EnemyDreamnailReaction.RecieveDreamImpact += DoDamage;
        Rogue.Instance.ShowDreamConvo("moth_dream".Localize());


    }

    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                dream_mul = 8;
                break;
            case 1:
                var fsm = HeroController.instance.gameObject.LocateMyFSM("Dream Nail");
                fsm.InsertCustomAction("Can Set?", SwordShoot, 0);
                fsm.GetAction<Wait>("Set Charge", 0).time = 0.05f;

                break;
        }
        base.GetBirthright(num);
    }
    void SwordShoot()
    {
        StartCoroutine(ShotSwords());
        IEnumerator ShotSwords()
        {
            float angle = 180f;
            for (int i = 0; i < 5; i++)
            {
                float t_angle = angle - 45 * i;
                var sword = Instantiate(dream_sword);

                sword.transform.position = HeroController.instance.transform.position + new Vector3(2 * Mathf.Cos(t_angle * Mathf.PI / 180), 2 * Mathf.Sin(t_angle * Mathf.PI / 180));
                sword.transform.SetRotation2D(t_angle);
                sword.SetActive(true);
                sword.GetComponent<DamageEnemies>().damageDealt = 4 * PlayerData.instance.nailDamage;
                sword.LocateMyFSM("Control").RemoveAction("Recycle", 0);
                sword.LocateMyFSM("Control").GetAction<Wait>("Antic", 1).time = 0.1f;
                sword.LocateMyFSM("Control").AddAction("Recycle", new DestroySelf());
                sword.LocateMyFSM("Control").SetState("Antic");
                yield return new WaitForSeconds(0.05f);
            }
            yield break;
        }

    }


    private void DoDamage(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
    {

        FSMUtility.SendEventToGameObject(self.gameObject, "TAKE DAMAGE");
        HitTaker.Hit(self.gameObject, new HitInstance
        {
            Source = base.gameObject,
            AttackType = AttackTypes.Generic,
            CircleDirection = false,
            DamageDealt = dream_mul * PlayerData.instance.nailDamage,
            Direction = HeroController.instance.transform.localScale.x < 0 ? 180f : 0,
            IgnoreInvulnerable = false,
            MagnitudeMultiplier = 0,
            MoveAngle = 0f,
            MoveDirection = false,
            Multiplier = 1f,
            SpecialType = SpecialTypes.None,
            IsExtraDamage = false
        });
        orig(self);
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                dream_mul = 0;
                break;
            case 1:
                var fsm = HeroController.instance.gameObject.LocateMyFSM("Dream Nail");
                fsm.GetAction<Wait>("Set Charge", 0).time = 1f;
                if (fsm.GetState("Can Set?").Actions.Length > 17)
                {
                    fsm.RemoveAction("Can Set?", 0);
                }
                break;

        }
        base.RemoveBirthright(num);
    }
    public override void EndCharacter()
    {
        On.EnemyDreamnailReaction.RecieveDreamImpact -= DoDamage;

    }
}