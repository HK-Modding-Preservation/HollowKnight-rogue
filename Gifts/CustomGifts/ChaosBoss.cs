
namespace rogue;

internal class ChaosBoss : CustomGift
{
    public ChaosBoss() : base(Giftname.custom_chaos_boss, 0, "witches_eye")
    {
        name = "custom_chaos_boss_name";
        desc = "custom_chaos_boss_desc";
    }

    protected override void _GetGift()
    {
        //Boss Sequence Tier 5
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.name == "GG_Spa" || arg1.name == "GG_Engine")
            if (BossSequenceController.BossIndex <= 45)
                ItemManager.Instance.StartCoroutine(ChangeBoss());
    }
    IEnumerator ChangeBoss()
    {
        yield return new WaitForSeconds(0.2f);
        Rogue.Instance.boss_scenes.ShuffleArray(BossSequenceController.BossIndex + 1, BossSequenceController.BossIndex + 5);

    }
    void TestShuffle()
    {
        Rogue.Instance.boss_scenes.RemoveAt<BossScene>(BossSequenceController.BossIndex + 1);
        Rogue.Instance.boss_scenes.ShuffleArray(BossSequenceController.BossIndex + 1, BossSequenceController.BossIndex + 6);
    }


    protected override void _RemoveGift()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
