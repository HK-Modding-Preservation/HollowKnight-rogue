
namespace rogue;

internal static class BugFixManager
{
    internal static List<GameObject> recycle_list = new();
    internal static void Init()
    {
        On.ObjectPool.DestroyPooled_GameObject_int += OnDestroyPooled;
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