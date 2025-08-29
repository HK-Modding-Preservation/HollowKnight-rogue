using System.Collections.Generic;
using InControl;

namespace rogue;

public class Setting
{
    public List<bool> small_gift_active = new();
    public List<bool> big_gift_active = new();

    public BindingSource start;

    public ulong startname;

    public BindingSource over;

    public ulong overname;

    public BindingSource refresh;

    public ulong refreshname;

    public bool owner;

    public float item_font_size = 6.67f;

    public float UI_alpha = 0.6f;

    public int reroll_num = 3;

    public bool details = false;
    public bool modboss_in_workshop = false;
    public bool testlog = false;

    internal static float SoundScale()
    {
        return GameManager.instance.gameSettings.soundVolume / 10f * GameManager.instance.gameSettings.masterVolume / 10f;
    }





}