
namespace rogue;
internal class CharmGift : Gift
{
    static readonly List<int> breakable = new() { 23, 24, 25 };
    static readonly Dictionary<Giftname, (float, int)> charm_weight_and_price = new Dictionary<Giftname, (float, int)>
        {
            {Giftname.charm_compass,(1,50)},
            {Giftname.charm_fengqun,(1,50)},
            {Giftname.charm_yingke,(1,200)},
            {Giftname.charm_bushou,(0.8f,350)},
            {Giftname.charm_saman,(0.4f,800)},
            {Giftname.charm_shihun,(0.8f,450)},
            {Giftname.charm_dash_master,(1f,150)},
            {Giftname.charm_feimaotui,(1,150)},
            {Giftname.charm_chongge,(8,300)},
            {Giftname.charm_tuibian,(1,200)},
            {Giftname.charm_xinzang,(1,300)},
            {Giftname.charm_tanlan,(5,200)},
            {Giftname.charm_power,(0.4f,800)},
            {Giftname.charm_faniu,(0.6f,500)},
            {Giftname.charm_wenti,(1,200)},
            {Giftname.charm_chenzhong,(1,200)},
            {Giftname.charm_kuaipi,(0.4f,600)},
            {Giftname.charm_xiuchang,(1,200)},
            {Giftname.charm_jiaoao,(0.8f,350)},
            {Giftname.charm_wangnu,(1,50)},
            {Giftname.charm_jingji,(1,100)},
            {Giftname.charm_baldur,(1f,150)},
            {Giftname.charm_xichong,(0.4f,600)},
            {Giftname.charm_defender,(1,100)},
            {Giftname.charm_zigong,(0.8f,200)},
            {Giftname.charm_quick_focus,(0.8f,300)},
            {Giftname.charm_deep_facus,(0.8f,300)},
            {Giftname.charm_blue_heart,(1,150)},
            {Giftname.charm_blue_hexin,(1,250)},
            {Giftname.charm_jiaoni,(0.8f,50)},
            {Giftname.charm_fengchao,(1,250)},
            {Giftname.charm_mogu,(1,150)},
            {Giftname.charm_fengli,(1,150)},
            {Giftname.charm_wuen,(0.8f,300)},
            {Giftname.charm_dingyao,(5,300)},
            {Giftname.charm_bian_zhi_zhe,(1,250)},
            {Giftname.charm_wumeng,(5,250)},
            {Giftname.charm_meng_zhi_dun,(1,200)},
            {Giftname.charm_wuyou,(0.8f,300)}
        };
    public CharmGift(Giftname giftname) : base(1)
    {
        this.giftname = giftname;
        charm_index = (int)giftname;
        weight = charm_weight_and_price.ContainsKey(giftname) ? charm_weight_and_price[giftname].Item1 : 0;
        price = charm_weight_and_price.ContainsKey(giftname) ? charm_weight_and_price[giftname].Item2 : 999;

    }

    int charm_index;
    internal override string GetName()
    {
        return Language.Language.Get("CHARM_NAME_" + charm_index + (breakable.Contains(charm_index) ? "_G" : "") + (charm_index == 40 ? "_N" : ""), "UI");
    }
    internal override string GetDesc()
    {
        return Language.Language.Get("CHARM_DESC_" + charm_index + (breakable.Contains(charm_index) ? "_G" : "") + (charm_index == 40 ? "_N" : ""), "UI").Replace("<br>", "\n");
    }
    internal override void GetGift()
    {
        PlayerData.instance.SetBool("gotCharm_" + (int)giftname, true);
    }
    internal override Sprite GetSprite()
    {
        var charmIcon = HutongGames.PlayMaker.FsmVariables.GlobalVariables.FindFsmGameObject("Charm Icons").Value;
        if (charmIcon == null) return null;
        if (!breakable.Contains((int)giftname) && (int)giftname != 40)
            return charmIcon.GetComponent<CharmIconList>().spriteList[(int)giftname];
        if ((int)giftname == 23)
        {
            return charmIcon.GetComponent<CharmIconList>().unbreakableHeart;
        }
        if ((int)giftname == 24)
        {
            return charmIcon.GetComponent<CharmIconList>().unbreakableGreed;
        }
        if ((int)giftname == 25)
        {
            return charmIcon.GetComponent<CharmIconList>().unbreakableStrength;
        }
        if ((int)giftname == 40)
        {
            return charmIcon.GetComponent<CharmIconList>().nymmCharm;
        }
        return null;
    }

}
