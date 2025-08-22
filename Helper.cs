namespace rogue;

internal static class Helper
{
    internal static void TestLog(this object msg)
    {
        if (Rogue.Instance.Test)
        {
            Rogue.Instance.Log(msg);
        }

    }
}