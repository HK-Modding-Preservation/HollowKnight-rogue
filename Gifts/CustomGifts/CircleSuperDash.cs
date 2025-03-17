using GlobalEnums;



namespace rogue;
internal class CircleSuperDash : CustomGift
{
    internal CircleSuperDash() : base(Giftname.custom_circle_super_dash, 4, "witches_eye")
    {
        name = "超冲转向";
        desc = "超冲过程中，按下攻击键，超冲转向";
        weight = 0.5f;
        price = 200;
    }
    FsmFloat current_speed_x;
    FsmFloat current_speed_y;
    protected override void _GetGift()
    {
        var fsm = HeroController.instance.superDash;
        fsm.GetAction<FloatCompare>("Dashing", 8).Enabled = false;
        fsm.GetAction<FloatCompare>("Cancelable", 6).Enabled = false;
        current_speed_x = fsm.FsmVariables.FindFsmFloat("Current SD Speed");
        current_speed_y = new("Current SD Speed Y");
        fsm.GetAction<SetVelocity2d>("Dashing", 2).x = current_speed_x;
        fsm.GetAction<SetVelocity2d>("Dashing", 2).y = current_speed_y;
        fsm.GetAction<SetVelocity2d>("Cancelable", 1).x = current_speed_x;
        fsm.GetAction<SetVelocity2d>("Cancelable", 1).y = current_speed_y;
        fsm.InsertCustomAction("Dash Start", () => { current_speed_y.Value = 0; }, 29);
        fsm.InsertCustomAction("Inactive", () =>
        {
            if (current_speed_y.Value < 0)
                HeroController.instance.gameObject.transform.position = HeroController.instance.gameObject.transform.position + new Vector3(0, 1, 0);
            HeroController.instance.gameObject.transform.SetRotation2D(0);
        }, 2);
        ModHooks.HeroUpdateHook += CheckChangeSuperDash;
        On.HeroController.OnCollisionEnter2D += CheckHitWall;
        On.HeroController.EnterScene += EnterScene;

    }

    private IEnumerator EnterScene(On.HeroController.orig_EnterScene orig, HeroController self, TransitionPoint enterGate, float delayBeforeEnter)
    {
        self.gameObject.transform.SetRotation2D(0);
        yield return orig(self, enterGate, delayBeforeEnter);
    }





    private void CheckChangeSuperDash()
    {
        var fsm = HeroController.instance.superDash;
        if (fsm.ActiveStateName == "Dashing" || fsm.ActiveStateName == "Cancelable")
        {
            if (InputHandler.Instance.inputActions.attack.WasPressed)
            {
                Log("Change");
                float speed;
                if (current_speed_x.Value != 0)
                {
                    speed = current_speed_x.Value;
                    current_speed_y.Value = speed;
                    current_speed_x.Value = 0;
                    HeroController.instance.gameObject.transform.SetRotation2D(90);
                }
                else
                {
                    speed = current_speed_y.Value;
                    current_speed_x.Value = -speed;
                    current_speed_y.Value = 0;
                    HeroController.instance.gameObject.transform.SetRotation2D(0);
                    if (speed > 0) HeroController.instance.FaceLeft();
                    else HeroController.instance.FaceRight();
                }
            }
        }
    }

    protected override void _RemoveGift()
    {
        var fsm = HeroController.instance.superDash;
        On.HeroController.OnCollisionEnter2D -= CheckHitWall;
        fsm.GetAction<FloatCompare>("Cancelable", 6).Enabled = true;
        fsm.GetAction<FloatCompare>("Dashing", 8).Enabled = true;
        fsm.GetAction<SetVelocity2d>("Dashing", 2).x = fsm.FsmVariables.FindFsmFloat("Current SD Speed");
        fsm.GetAction<SetVelocity2d>("Dashing", 2).y = 0;
        fsm.GetAction<SetVelocity2d>("Cancelable", 1).x = fsm.FsmVariables.FindFsmFloat("Current SD Speed");
        fsm.GetAction<SetVelocity2d>("Cancelable", 1).y = 0;
        fsm.RemoveAction("Dash Start", 29);
        fsm.RemoveAction("Inactive", 2);
        ModHooks.HeroUpdateHook -= CheckChangeSuperDash;
        On.HeroController.OnCollisionEnter2D -= CheckHitWall;
        On.HeroController.EnterScene -= EnterScene;
    }

    private void CheckHitWall(On.HeroController.orig_OnCollisionEnter2D orig, HeroController self, Collision2D collision)
    {
        if (self.cState.superDashing)
        {
            if (CheckStillTouchingWall(CollisionSide.top) || CheckStillTouchingWall(CollisionSide.bottom))
                self.superDash.SendEvent("HIT WALL");

        }
        orig(self, collision);
    }
    private bool CheckStillTouchingWall(CollisionSide side, bool checkTop = false)
    {
        var col2d = HeroController.instance.gameObject.GetComponent<Collider2D>();
        Vector2 origin = new Vector2(col2d.bounds.min.x, col2d.bounds.max.y);
        Vector2 origin2 = new Vector2(col2d.bounds.center.x, col2d.bounds.max.y);
        Vector2 origin3 = new Vector2(col2d.bounds.max.x, col2d.bounds.max.y);
        Vector2 origin4 = new Vector2(col2d.bounds.min.x, col2d.bounds.min.y);
        Vector2 origin5 = new Vector2(col2d.bounds.center.x, col2d.bounds.min.y);
        Vector2 origin6 = new Vector2(col2d.bounds.max.x, col2d.bounds.min.y);
        float distance = 0.1f;
        RaycastHit2D raycastHit2D = default(RaycastHit2D);
        RaycastHit2D raycastHit2D2 = default(RaycastHit2D);
        RaycastHit2D raycastHit2D3 = default(RaycastHit2D);
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        switch (side)
        {
            case CollisionSide.top:
                if (checkTop)
                {
                    raycastHit2D = Physics2D.Raycast(origin, Vector2.up, distance, 256);
                }

                raycastHit2D2 = Physics2D.Raycast(origin2, Vector2.up, distance, 256);
                raycastHit2D3 = Physics2D.Raycast(origin3, Vector2.up, distance, 256);
                break;
            case CollisionSide.bottom:
                if (checkTop)
                {
                    raycastHit2D = Physics2D.Raycast(origin4, Vector2.down, distance, 256);
                }

                raycastHit2D2 = Physics2D.Raycast(origin5, Vector2.down, distance, 256);
                raycastHit2D3 = Physics2D.Raycast(origin6, Vector2.down, distance, 256);
                break;
            default:
                Debug.LogError("Invalid CollisionSide specified.");
                return false;
        }

        if (raycastHit2D2.collider != null)
        {
            flag2 = true;
            if (raycastHit2D2.collider.isTrigger)
            {
                flag2 = false;
            }

            if (raycastHit2D2.collider.GetComponent<SteepSlope>() != null)
            {
                flag2 = false;
            }

            if (raycastHit2D2.collider.GetComponent<NonSlider>() != null)
            {
                flag2 = false;
            }

            if (flag2)
            {
                return true;
            }
        }

        if (raycastHit2D3.collider != null)
        {
            flag3 = true;
            if (raycastHit2D3.collider.isTrigger)
            {
                flag3 = false;
            }

            if (raycastHit2D3.collider.GetComponent<SteepSlope>() != null)
            {
                flag3 = false;
            }

            if (raycastHit2D3.collider.GetComponent<NonSlider>() != null)
            {
                flag3 = false;
            }

            if (flag3)
            {
                return true;
            }
        }

        if (checkTop && raycastHit2D.collider != null)
        {
            flag = true;
            if (raycastHit2D.collider.isTrigger)
            {
                flag = false;
            }

            if (raycastHit2D.collider.GetComponent<SteepSlope>() != null)
            {
                flag = false;
            }

            if (raycastHit2D.collider.GetComponent<NonSlider>() != null)
            {
                flag = false;
            }

            if (flag)
            {
                return true;
            }
        }

        return false;
    }


}