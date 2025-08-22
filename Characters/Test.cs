
namespace rogue.Characters;

internal class Test : Character
{
    public Test()
    {
        this.Selfname = CharacterRole.test;
        AddBirthRight("贪婪");
    }
    bool get_birthright = false;
    public override void BeginCharacter()
    {
        var res = ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CheckStillTouchingWall", GlobalEnums.CollisionSide.left);
        GameInfo.role = CharacterRole.test;
        Rogue.Instance.ShowDreamConvo("test_dream".Localize());
        ItemManager.Instance.before_spawn_item += OneMoreThing;
    }

    private ItemManager.OneReward OneMoreThing(ItemManager.OneReward reward)
    {
        int whole = 0;

        if (reward.give_mode == ItemManager.OneReward.GiveMode.fix)
        {
            whole = reward.gifts.Count;
        }
        else if (reward.give_mode == ItemManager.OneReward.GiveMode.random)
        {
            whole = reward.give;
        }
        int add = get_birthright ? 2 : 1;
        reward.select = Mathf.Max(reward.select + add, whole);
        return reward;
    }

    public override void EndCharacter()
    {
        ItemManager.Instance.before_spawn_item -= OneMoreThing;
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                get_birthright = true;
                break;
        }
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                get_birthright = false;
                break;
        }
    }

}