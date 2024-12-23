namespace rogue.Characters;
internal class Shaman : Character
{
    public Shaman()
    {
        this.Selfname = CharacterRole.shaman;
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
}