using System.Collections.Generic;
using InControl;

namespace rogue;

public class setting
{
    public List<bool> small_gift_active = new();
    public List<bool> big_gift_active = new();

    public Dictionary<Rogue.giftname, Rogue.gift> smallgifts = new();
    public Dictionary<Rogue.giftname, Rogue.gift> biggifts = new();


    public BindingSource start;

    public ulong startname;

    public BindingSource over;

    public ulong overname;

    public BindingSource refresh;

    public ulong refreshname;

    public bool owner;




}