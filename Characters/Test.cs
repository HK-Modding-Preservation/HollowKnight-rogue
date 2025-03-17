
namespace rogue.Characters;
internal class Test : Character
{
    public Test()
    {
        this.Selfname = CharacterRole.test;
        birthright_names = new()
        {
            "贪婪"
        };
    }
    bool get_birthright = false;
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.test;
        Rogue.Instance.ShowDreamConvo("test_dream".Localize());
        ItemManager.Instance.before_spawn_item += OneMoreThing;
    }

    private ItemManager.OneReward OneMoreThing(ItemManager.OneReward reward)
    {
        if (reward.mode == ItemManager.Mode.select_small_gift)
        {
            if (reward.select < reward.give)
            {
                reward.select++;
            }
        }
        if (reward.mode == ItemManager.Mode.fix_select_small_gift && get_birthright)
        {
            reward.select++;
        }
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