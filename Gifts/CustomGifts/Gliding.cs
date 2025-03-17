
namespace rogue;
internal class Gliding : CustomGift
{
    GameObject butterfly;
    internal Gliding() : base(Giftname.custom_gliding, 4, "health_butterfly")
    {
        weight = 0.5f;
        price = 200;
        name = "滑翔";
        desc = "小骑士下落的时候可以滑翔";
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