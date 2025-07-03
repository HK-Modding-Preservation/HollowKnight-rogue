
namespace rogue.NPCs;

internal class Leveler : NPC
{
    internal static Vector3 pos = new(124.9f, 63.0f, 0.1f);
    public Leveler(tk2dSpriteAnimation library) : base(library, new(0, -2))
    {
        name = " ";
        name_super = " ";
        name_sub = " ";
        go.LocateMyFSM("Conversation Control").GetAction<SendEventByName>("Box Up", 0).Enabled = false;


    }
    protected override void IdleAnimation()
    {
        return;
    }
    protected override void TalkAnimation()
    {
        return;
    }
    protected override void ShowTitle()
    {
        return;
    }
    internal override void BeginConvo()
    {
        if (GameInfo.in_rogue)
        {
            RogueUIManager.StartSelection(0.3f, "确认结束吗？", new List<RogueUIManager.SelectItem>
            {
                new RogueUIManager.SelectItem("是"){
                    select_action=(select)=>{

                        Rogue.Instance.Rogue_Over();
                        go.GetComponent<tk2dSprite>().color=Color.white;
                        go.GetComponent<tk2dSpriteAnimator>().Play("Lever Idle");
                        go.LocateMyFSM("Conversation Control").SendEvent("CONVO_FINISH");
                    }
                },
                new RogueUIManager.SelectItem("否"){
                    select_action=(select)=>{
                        go.LocateMyFSM("Conversation Control").SendEvent("CONVO_FINISH");
                    }
                }
            }, 2);
        }
        else
        {
            RogueUIManager.StartSelection(0.3f, "请选择要挑战的等级", new List<RogueUIManager.SelectItem>
        {
            new RogueUIManager.SelectItem("基础难度" ){select_action = (select) => {
                GameInfo.gameMode=GameInfo.GameMode.MODE0;
                // go.GetComponent<tk2dSpriteAnimator>().AnimationCompleted=(animator,clip)=>{animator.Play("Lever Activated");animator.AnimationCompleted=null;};
                go.GetComponent<tk2dSpriteAnimator>().Play("Lever Hit");
                go.LocateMyFSM("Conversation Control").SendEvent("CONVO_FINISH");
                ProcessManager.Instance.GameStart();

             } },
            new RogueUIManager.SelectItem("进阶难度"){select_action=(select)=>{
                GameInfo.gameMode=GameInfo.GameMode.MODE1;
                go.GetComponent<tk2dSprite>().color=Color.red;
                // go.GetComponent<tk2dSpriteAnimator>().AnimationCompleted=(animator,clip)=>{animator.Play("Lever Activated");animator.AnimationCompleted=null;};
                go.GetComponent<tk2dSpriteAnimator>().Play("Lever Hit");

                go.LocateMyFSM("Conversation Control").SendEvent("CONVO_FINISH");
                ProcessManager.Instance.GameStart();
                }
            },
            new RogueUIManager.SelectItem("取消"){select_action=(select)=>{
                go.LocateMyFSM("Conversation Control").SendEvent("CONVO_FINISH");
            }},
        }, 3);
        }
    }
}