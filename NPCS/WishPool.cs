using System.Diagnostics.Eventing.Reader;

namespace rogue.NPCs;

internal class WishPool : NPC
{
    const int wish_rate = 25;
    bool get_birthright = false;
    internal WishPool(Sprite down, Sprite up, GameObject well) : base(null, new(0, 1), KnightAction.LookUP, true)
    {
        go.GetComponent<SpriteRenderer>().sprite = down;
        well.transform.SetParent(go.transform);
        well.transform.localPosition = new(-0.2f, 5.253f, 0);
        well.SetActive(true);
        GameObject up_go = new("up");
        up_go.transform.SetParent(go.transform);
        up_go.transform.localPosition = new(0f, 7.953f, 0);
        up_go.AddComponent<SpriteRenderer>().sprite = up;
        go.transform.localScale = new(0.5f, 0.5f, 1);

        name = "";
        name_sub = "";
        name_super = "";
        AddConversation("初遇", "npc_wishpool_conv_1".Localize());
        OnConvoEnd("初遇", () =>
        {
            RogueUIManager.SelectItem yes = new(Lang.YES);
            yes.select_action = (select) =>
            {
                HeroController.instance.TakeGeo(100);
                int t = UnityEngine.Random.Range(0, 101);
                if (t <= 30)
                {
                    Rogue.Instance.ShowDreamConvo("npc_wishpool_res_1".Localize());
                    if (get_birthright)
                    {
                        ItemManager.Instance.rewardsStack.Push(new()
                        {
                            give_mode = ItemManager.OneReward.GiveMode.random,
                            gift_variety = GiftFactory.CustomVariety(),
                            give = 1,
                            select = 1,
                            gifts = null
                        });
                        ItemManager.Instance.Next(force: false);
                    }
                    else
                    {
                        get_birthright = true;
                        GiftFactory.all_gifts[Giftname.get_birthright].GetGift();
                    }
                }
                else if (t > 30 && t <= 45)
                {
                    Rogue.Instance.ShowDreamConvo("npc_wishpool_res_2".Localize());
                    GameInfo.revive_num += 1;
                }
                else if (t > 45 && t <= 60)
                {
                    Rogue.Instance.ShowDreamConvo("npc_wishpool_res_3".Localize());
                    GameInfo.refresh_num += 1;
                }
                else if (t > 60 && t <= 90)
                {
                    Rogue.Instance.ShowDreamConvo("npc_wishpool_res_4".Localize());
                    HeroController.instance.AddGeo(50);

                }
                else
                {
                    Rogue.Instance.ShowDreamConvo("nothing_happened".Localize());
                }
                DisplayManager.DisplayStates();
            };
            yes.selectable = PlayerData.instance.geo >= 100;
            yes.not_select_info = "geo_cant_select_info".Localize();
            RogueUIManager.SelectItem no = new(Lang.NO);
            no.select_action = null;
            List<RogueUIManager.SelectItem> selectItems = new() { yes, no };

            RogueUIManager.StartSelection(0.3f, "npc_wishpool_select_conv".Localize(), selectItems, 2);
        });
    }
    internal static Vector3 spa_pos = new Vector3(104f, 14.0491f, 0.1f);
    internal override string GetName(string pos)
    {
        return "";
    }
    internal override void BeginConvo()
    {
        ShowDialogue("初遇");
    }
}