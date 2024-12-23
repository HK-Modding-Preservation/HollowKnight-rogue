using System.Collections.Generic;
using InControl;

namespace rogue;

public class setting
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




}