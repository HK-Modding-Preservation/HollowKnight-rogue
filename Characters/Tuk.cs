

namespace rogue.Characters;
internal class Tuk : Character
{
    public Tuk()
    {
        this.Selfname = CharacterRole.tuk;
        birthright_names = new(){
            "感染",
            "+2腐臭蛋",
            "使用腐臭蛋+200geo"
        };
    }
    System.Random random = new System.Random();

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.tuk;
        GiftHelper.AdjustMaskTo(3);
        HeroController.instance.AddGeo(500);
        GameInfo.revive_num = 5;
        Rogue.Instance.ShowDreamConvo("tuk_dream".Localize());
        ItemManager.Instance.after_revive_action += TukRevive;
    }

    private void TukRevive()
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
        if (got_birthright.Contains(3))
        {
            HeroController.instance.AddGeo(200);
        }

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
                GameInfo.revive_num += 2;
                break;
            case 2:
                break;

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
        ItemManager.Instance.after_revive_action -= TukRevive;
    }
}