
using IL;

namespace rogue;

internal class LoveKey : CustomGift
{
    //爱之钥，用于打开收藏家通路
    //具体实现方式，将GameInfo.Branch.collector设置为true
    //在GameInfo.Branch.collector为true时，三门上将替换为边境小怪房，收藏家会变为modboss形式
    internal LoveKey() : base(Giftname.custom_love_key, 4, "love_key")
    {
        giftname = Giftname.custom_love_key;
        price = 200;
        desc = "love_key_desc";
        force_active = false;
    }

    protected override void _GetGift()
    {
        GameInfo.Branch.collector = true;
        var oriscenes = ReflectionHelper.GetField<BossSequence, BossScene[]>(Rogue.Instance.bossSequence, "bossScenes");
        var replacy_scene = oriscenes.FirstOrDefault((bs) => { return bs.name == "Grimm Boss Scene"; });
        replacy_scene.sceneName = "GG_Collector_V";
    }

    protected override void _RemoveGift()
    {
        GameInfo.Branch.collector = false;
        var oriscenes = ReflectionHelper.GetField<BossSequence, BossScene[]>(Rogue.Instance.bossSequence, "bossScenes");
        var replacy_scene = oriscenes.FirstOrDefault((bs) => { return bs.name == "Grimm Boss Scene"; });
        replacy_scene.sceneName = "GG_Grimm";
    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_LOVEKEY", "UI");
    }

}