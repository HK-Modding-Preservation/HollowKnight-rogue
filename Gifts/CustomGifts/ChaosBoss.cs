
namespace rogue;
internal class ChaosBoss : CustomGift
{
    public ChaosBoss() : base(Giftname.custom_chaos_boss, 4, "witches_eye")
    {
        name = "小丑把戏";
        desc = "今天感觉怎么样？";
        weight = 0.5f;
    }

    protected override void _GetGift()
    {
        //Boss Sequence Tier 5
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.name == "GG_Spa")
            ItemManager.Instance.StartCoroutine(ChangeBoss());
    }
    IEnumerator ChangeBoss()
    {
        yield return new WaitForSeconds(0.2f);
        Rogue.Instance.boss_scenes.ShuffleArray(BossSequenceController.BossIndex + 1, BossSequenceController.BossIndex + 6);

    }
    void TestShuffle()
    {
        Rogue.Instance.boss_scenes.RemoveAt<BossScene>(BossSequenceController.BossIndex + 1);
        Rogue.Instance.boss_scenes.ShuffleArray(BossSequenceController.BossIndex + 1, BossSequenceController.BossIndex + 6);
    }


    protected override void _RemoveGift()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }
}
