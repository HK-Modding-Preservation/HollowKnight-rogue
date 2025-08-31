




namespace rogue;

internal static class BugFixManager
{
    internal static List<GameObject> recycle_list = new();
    internal static List<string> animator_list = new()
    {
        "Dream Mage Lord",
        "Dream Mage Lord Phase2",
        "Mage Lord",
        "Mage Lord Phase2",
        "Giant Fly"
    };
    internal static void Init()
    {
        On.ObjectPool.DestroyPooled_GameObject_int += OnDestroyPooled;
        On.tk2dSpriteAnimator.OnEnable += OnSpriteAnimatorEnable;
        On.PlayMakerFSM.Awake += ChangeHealth;
        On.PlayMakerFSM.OnEnable += ChangeEnding;
    }

    private static void ChangeEnding(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);
        if (self.name == "Absolute Radiance" && self.FsmName == "Control")
        {
            self.GetAction<SetStaticVariable>("Ending Scene", 1).setValue.boolValue = false;//Modify this action to leave p5 as other doors
        }
    }

    private static void ChangeHealth(On.PlayMakerFSM.orig_Awake orig, PlayMakerFSM self)
    {
        orig(self);
        if (self.transform.parent != null && self.transform.parent.gameObject.name == "Health" && self.FsmName == "health_display")
        {
            if (self.GetState("Up Check") == null)
            {
                var state = self.AddState("Up Check");
                state.AddCustomAction((state) =>
                {
                    if (PlayerData.instance.health < state.Fsm.GetFsmInt("Health Number").Value)
                    {
                        self.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        self.gameObject.FindGameObjectInChildren("Idle").GetComponent<MeshRenderer>().enabled = false;
                        self.gameObject.GetComponent<tk2dSpriteAnimator>().Play("Health Empty");
                        state.Fsm.Event("EMPTY");
                        state.Fsm.GetFsmBool("Initialised").Value = true;
                    }
                });
                state.AddTransition("FINISHED", "Idle");
                state.AddTransition("EMPTY", "Empty");
                self.ChangeTransition("Max Up Flash", "FINISHED", "Up Check");
            }



        }
        if (self.gameObject.name == "Health" && self.FsmName == "Blue Health Control")
        {
            self.GetAction<SetPosition>("Add Blue Health 2", 10).y = 8.68f;
            self.InsertCustomAction("Add Blue Health 2", (fsm) =>
            {
                if (PlayerData.instance.equippedCharm_27)
                {
                    var health = GameCameras.instance.hudCanvas.gameObject.FindGameObjectInChildren("Health");
                    int count = 0;
                    foreach (Transform child in health.transform)
                    {
                        if (child.name.Contains("Blue") && child.localPosition.y < 8f && child.localPosition.y > 7f)
                        {
                            if (child.gameObject.LocateMyFSM("blue_health_display").ActiveStateName != "Health Reset")
                            {
                                count++;
                            }

                        }
                    }
                    count.TestLog();
                    self.FsmVariables.FindFsmInt("Max HP").Value = -count;
                }
                else
                {
                    self.FsmVariables.FindFsmInt("Max HP").Value = 0;
                }
            }, 2);
            self.GetAction<SetPosition>("Add Blue Health", 12).y = 8.68f;
            self.InsertCustomAction("Add Blue Health", (fsm) =>
            {
                if (PlayerData.instance.equippedCharm_27)
                {
                    var health = GameCameras.instance.hudCanvas.gameObject.FindGameObjectInChildren("Health");
                    int count = 0;
                    foreach (Transform child in health.transform)
                    {
                        if (child.name.Contains("Blue") && child.localPosition.y < 8f && child.localPosition.y > 7f)
                        {
                            if (child.gameObject.LocateMyFSM("blue_health_display").ActiveStateName != "Health Reset")
                            {
                                count++;
                            }
                        }
                    }
                    count.TestLog();
                    self.FsmVariables.FindFsmInt("Max HP").Value = -count;
                }
                else
                {
                    self.FsmVariables.FindFsmInt("Max HP").Value = 0;
                }
            }, 2);

        }
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
            (prefab.name + "still has" + ObjectPool.CountPooled(prefab)).TestLog();
            ObjectPool.DestroyPooled(prefab);
        }
    }
    internal static void GameLoadInit()
    {

    }
}