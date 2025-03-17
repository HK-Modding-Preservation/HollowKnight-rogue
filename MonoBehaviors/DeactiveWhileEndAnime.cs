internal class DeactiveWhileEndAnime : MonoBehaviour
{
    public List<Behaviour> behaviours = new();
    public tk2dSpriteAnimator spriteAnimator;
    public Action<tk2dSpriteAnimator> enable_action = null;
    public float timer = 1;
    public bool need_timer = false;
    private float true_timer;


    private void OnEnable()
    {
        if (spriteAnimator == null)
        {
            spriteAnimator = GetComponent<tk2dSpriteAnimator>();
        }
        if (need_timer) true_timer = timer;
        enable_action?.Invoke(spriteAnimator);

    }

    private void Update()
    {
        true_timer -= Time.deltaTime;
        if (true_timer < 0 && need_timer) DoDeactive();

        if (!spriteAnimator.Playing)
        {
            DoDeactive();
        }
    }
    private void DoDeactive()
    {
        base.gameObject.SetActive(value: false);
        foreach (var monoBehaviour in behaviours)
        {
            monoBehaviour.enabled = false;
        }
    }

}