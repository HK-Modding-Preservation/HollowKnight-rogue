



namespace rogue.Characters;

internal class Tuk : Character
{
    public Tuk()
    {
        this.Selfname = CharacterRole.tuk;
        AddBirthRight("tuk_birthright_0_name".Localize());
        AddBirthRight("tuk_birthright_1_name".Localize());
        AddBirthRight("tuk_birthright_2_name".Localize());
    }
    System.Random random = new System.Random();
    internal const string bomb_scene = "Fungus2_08";
    internal const string bomb_name = "Mushroom Turret";
    static GameObject bomb;
    internal static void Init()
    {
        bomb = (GameObject)Resources.InstanceIDToObject(30308);
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.tuk;
        GiftHelper.AdjustMaskTo(3);
        HeroController.instance.AddGeo(500);
        GameInfo.revive_num = 5;
        Rogue.Instance.ShowDreamConvo("tuk_dream".Localize());
        ModHooks.AfterTakeDamageHook += OnAfterTakeDamage;
    }

    private int OnAfterTakeDamage(int hazardType, int damageAmount)
    {
        if (damageAmount > 0)
        {
            GameObject b = GameObject.Instantiate(bomb);
            b.GetComponent<DamageHero>().enabled = false;
            b.SetActive(true);
            damageAmount.TestLog();
            int damage = (int)((GiftHelper.GetNailLevel() * 3 + 10) * damageAmount * (1 + (0.2 * GameInfo.revive_num)));
            damage.TestLog();
            b.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = damage;
            b.transform.position = HeroController.instance.transform.position;
        }
        return damageAmount;
    }

    protected override void AfterRevive()
    {
        if (random.Next(0, 2) == 0)
        {
            GiftHelper.AddNailDamage();
        }
        else
        {
            GiftHelper.GiveMask();
            GiftHelper.GiveMask();
        }
        if (birthrights[2].got > 0)
        {
            HeroController.instance.AddGeo(200);
        }
        base.AfterRevive();
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                ModHooks.TakeDamageHook += MoreSelfDamage;
                On.HealthManager.TakeDamage += MoreEnemyDamage;
                break;
            case 1:
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged += GetEgg;
                break;
            case 2:

                break;

        }
    }

    private void GetEgg(Scene arg0, Scene arg1)
    {
        if (arg1.name == "GG_Spa")
        {
            GameInfo.revive_num++;
            DisplayManager.DisplayStates();
        }
    }

    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                ModHooks.TakeDamageHook -= MoreSelfDamage;
                On.HealthManager.TakeDamage -= MoreEnemyDamage;
                break;
            case 1:
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= GetEgg;
                break;
            case 2:

                break;
        }
    }

    private void MoreEnemyDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        for (int i = 0; i < GameInfo.revive_num; i++)
        {
            hitInstance.Multiplier *= 1.1f;
        }
        orig(self, hitInstance);
    }

    private int MoreSelfDamage(ref int hazardType, int damage)
    {
        if (damage > 0) damage += GameInfo.revive_num;
        return damage;
    }

    public override void EndCharacter()
    {
        ModHooks.AfterTakeDamageHook -= OnAfterTakeDamage;
    }
}