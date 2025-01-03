using rogue;

internal class ItemGift : Gift
{
    internal ItemGift(Giftname giftname, string name, string desc, int level, Action<Giftname> action) : base(level)
    {
        this.giftname = giftname;
        this.name = name;
        this.desc = desc;
        this.weight = 1.1f - (0.1f * level);
        getSprite = null;
        showConvo = true;
        active = true;
        this.reward = action;
    }
}