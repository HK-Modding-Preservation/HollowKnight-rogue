using rogue.Characters;
namespace rogue;

internal static class Helper
{
    internal static void TestLog(this object msg)
    {
        if (Rogue.Instance == null)
        {
            if (msg != null) Modding.Logger.Log(msg.ToString());
            else Modding.Logger.Log("null");
            return;
        }
        if (Rogue.Instance._set.testlog)
        {
            Rogue.Instance.Log(msg);
        }


    }
    internal static void CharacterInit()
    {
        Shaman.Init();
        NailMaster.Init();
        Moth.Init();
        Tuk.Init();
    }
}