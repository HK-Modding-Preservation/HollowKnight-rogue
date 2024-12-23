namespace rogue.Characters;
internal class Test : Character
{
    public Test()
    {
        this.Selfname = CharacterRole.test;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.test;
        Rogue.Instance.ShowDreamConvo("test_dream".Localize());
    }

    public override void EndCharacter()
    {
    }
}