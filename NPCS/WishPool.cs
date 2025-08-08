using System.Diagnostics.Eventing.Reader;

namespace rogue.NPCs;

internal class WishPool : NPC
{
    const int wish_rate = 25;
    bool get_birthright = false;
    internal WishPool(Sprite down, Sprite up, GameObject well) : base(null, new(0, 0), KnightAction.LookUP, true)
    {
        go.GetComponent<SpriteRenderer>().sprite = down;
        well.transform.SetParent(go.transform);
        well.transform.localPosition = new(-0.2f, 5.253f, 0);
        GameObject up_go = new("up");
        up_go.transform.SetParent(go.transform);
        up_go.transform.localPosition = new(0f, 7.953f, 0);
        up_go.AddComponent<SpriteRenderer>().sprite = up;
        go.transform.localScale = new(0.5f, 0.5f, 1);

        name = "";
        name_sub = "";
        name_super = "";
        AddConversation("初遇", "这是一个许愿池<page>你可以在这里许下愿望");
        OnConvoEnd("初遇", () =>
        {
            RogueUIManager.SelectItem yes = new("许愿");
            yes.select_action = (select) =>
            {
                PlayerData.instance.TakeGeo(100);
                int t = UnityEngine.Random.Range(0, 101);
                if (t <= 30)
                {
                    Rogue.Instance.ShowDreamConvo("你获得了白王之愿");
                    if (get_birthright)
                    {
                        var gift = ItemManager.RandomList(GameInfo.act_gifts[GiftFactory.CustomVariety()], 1)[0];
                        gift.GetGift();
                    }
                    else
                    {
                        get_birthright = true;
                        GiftFactory.all_gifts[Giftname.get_birthright].GetGift();
                    }
                }
                else if (t > 30 && t <= 45)
                {
                    Rogue.Instance.ShowDreamConvo("你获得了再生之力");
                    GameInfo.revive_num += 1;
                }
                else if (t > 45 && t <= 60)
                {
                    Rogue.Instance.ShowDreamConvo("你获得了命运之骰");
                    GameInfo.refresh_num += 1;
                }
                else if (t > 60 && t <= 90)
                {
                    Rogue.Instance.ShowDreamConvo("你获得了意外之财");
                    PlayerData.instance.AddGeo(50);

                }
                else
                {
                    Rogue.Instance.ShowDreamConvo("什么也没有发生");
                }

            };
            yes.selectable = PlayerData.instance.geo >= 100;
            yes.not_select_info = "吉欧不足";
            RogueUIManager.SelectItem no = new("不许愿");
            no.select_action = (select) => ShowDialogue("不许愿");
            List<RogueUIManager.SelectItem> selectItems = new() { yes, no };

            RogueUIManager.StartSelection(0.3f, "投入100geo，让苍白之王解除你的负担", selectItems, 2);
        });
    }
    internal override void BeginConvo()
    {
        ShowDialogue("初遇");
    }
}