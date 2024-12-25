namespace rogue.Characters;
internal class Shaman : Character
{
    public Shaman()
    {
        this.Selfname = CharacterRole.shaman;
        nail_mul = 0.8f;
        spell_mul = 1.25f;
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.shaman;
        PlayerData.instance.gotCharm_19 = true;
        PlayerData.instance.fireballLevel = 2;
        GameInfo.refresh_num += 2;
        Rogue.Instance.ShowDreamConvo("shaman_dream".Localize());
        On.SpellFluke.OnEnable += OnFlukeDamage;
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

    public override void EndCharacter()
    {
        On.SpellFluke.OnEnable -= OnFlukeDamage;
    }
    public override int GetBirthrightNum()
    {
        return 1;
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.quakeLevel += 1;
                if (PlayerData.instance.quakeLevel > 2) PlayerData.instance.quakeLevel = 2;
                break;
        }
    }
    public override void RemoveBirthright(int num)
    {
    }
}