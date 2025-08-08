

namespace rogue;

internal static class BugFixManager
{
    internal static List<GameObject> recycle_list = new();
    internal static List<string> animator_list = new()
    {
        "Dream Mage Lord",
        "Dream Mage Lord Phase2"
    };
    internal static void Init()
    {
        On.ObjectPool.DestroyPooled_GameObject_int += OnDestroyPooled;
        On.tk2dSpriteAnimator.OnEnable += OnSpriteAnimatorEnable;
    }

    private static void OnSpriteAnimatorEnable(On.tk2dSpriteAnimator.orig_OnEnable orig, tk2dSpriteAnimator self)
    {

        if (animator_list.Contains(self.gameObject.name))
        {
            self.playAutomatically = false;
        }
        orig(self);
    }

    private static void OnDestroyPooled(On.ObjectPool.orig_DestroyPooled_GameObject_int orig, GameObject prefab, int amountToRemove)
    {
        orig(prefab, amountToRemove);
        if (recycle_list.Contains(prefab))
        {
            Rogue.TestLog(prefab.name + "still has" + ObjectPool.CountPooled(prefab));
            ObjectPool.DestroyPooled(prefab);
        }
    }
    internal static void GameLoadInit()
    {

    }
}