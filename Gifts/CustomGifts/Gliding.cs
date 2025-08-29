
namespace rogue;

internal class Gliding : CustomGift
{
    GameObject butterfly;
    internal Gliding() : base(Giftname.custom_gliding, 1, "health_butterfly")
    {
        price = 200;
        name = "custom_gliding_name";
        desc = "custom_gliding_desc";
        butterfly = new("health_butterfly");
        butterfly.AddComponent<SpriteRenderer>().sprite = SpriteLoader.GetSprite("health_butterfly");
        butterfly.SetActive(false);
        GameObject.DontDestroyOnLoad(butterfly);


    }

    protected override void _GetGift()
    {
        On.HeroController.FixedUpdate += KnightGliding;
        butterfly.transform.position = HeroController.instance.transform.position;
        butterfly.transform.SetParent(HeroController.instance.transform);
        butterfly.transform.localScale = new Vector3(-1.3f, 1.3f, 1);
    }

    private void KnightGliding(On.HeroController.orig_FixedUpdate orig, HeroController self)
    {
        orig(self);
        var rig = self.gameObject.GetComponent<Rigidbody2D>();
        var vel = self.gameObject.GetComponent<Rigidbody2D>().velocity;
        bool flag = false;
        if (rig.gravityScale > Mathf.Epsilon)
        {
            if (!self.cState.jumping && !self.cState.onGround && !self.cState.wallSliding && InputHandler.Instance.inputActions.jump.IsPressed)
            {
                if (vel.y < self.WALLSLIDE_SPEED * 0.5f)
                {
                    flag = true;
                    rig.velocity = new Vector2(vel.x, self.WALLSLIDE_SPEED * 0.5f);
                    butterfly.SetActive(true);
                    if (self.fallTimer > self.BIG_FALL_TIME)
                    {
                        ReflectionHelper.SetProperty<HeroController, float>(self, "fallTimer", 0f);
                        self.cState.willHardLand = false;
                    }
                }
            }
        }
        if (!flag) butterfly.SetActive(false);
    }

    protected override void _RemoveGift()
    {
        On.HeroController.FixedUpdate -= KnightGliding;
        butterfly.transform.SetParent(null);
        butterfly.SetActive(false);
    }
}