using System.IO;
using System.Threading;
using Satchel;
using Satchel.Futils;
using UnityEngine.UIElements;

namespace rogue.ModBosses;
internal class Uradiance : MonoBehaviour
{

    PlayMakerFSM _con;
    PlayMakerFSM _cho;
    PlayMakerFSM _com;
    PlayMakerFSM _tele;
    PlayMakerFSM _phcon;
    HealthManager _hp;
    GameObject oribeam;
    GameObject fakebb;

    GameObject bb = null;
    List<GameObject> orbList = new List<GameObject>();
    List<GameObject> useorbs = new List<GameObject>();
    List<GameObject> a2beams = new List<GameObject>();
    GameObject knight;
    bool foundbeam = false;
    bool final = false;
    bool finalSet1 = false;
    bool finalSet2 = false;
    private AudioClip BeamAnticClip;
    private AudioClip BeamFireClip;
    private AudioClip tip;
    private string convoTitle;


    int initHp = 4550;
    int spikeWaves_HP = 3750;
    int a1Rage_HP = 2950;
    int stun_HP = 2500;
    int p5Acend_HP = 1500;
    int Scream_HP = 1000;
    int death_HP = 550;


    float gap = 0.7f;
    float gapnow = 0f;
    float height = 80;
    float bbgap = 0.2f;
    float bbgapnow = 0f;
    float finalgap = 5f;
    float finalgapnow = 0f;

    GameObject orb;
    GameObject sword;
    GameObject beam;
    GameObject glow;


    float hour = (90f - System.DateTime.Now.Hour * 30f) % 360f;
    float minute = (90f - System.DateTime.Now.Minute * 6f) % 360f;
    float second = (90f - System.DateTime.Now.Second * 6f) % 360f;
    GameObject timeClock = new GameObject();
    GameObject Clock;
    GameObject hourHand;
    GameObject minuteHand;
    GameObject secondHand;
    AudioSource clock;
    float zoomscale;
    bool zoom = false;
    List<GameObject> beams = new();
    List<GameObject> spike_beams = new();
    System.Random general_random = new System.Random();
    GameObject slash_effect = null;
    static GameObject slash_go;
    GameObject slash_inst = null;


    private void Awake()
    {
        _cho = gameObject.LocateMyFSM("Attack Choices");
        _com = gameObject.LocateMyFSM("Attack Commands");
        _con = gameObject.LocateMyFSM("Control");
        _tele = gameObject.LocateMyFSM("Teleport");
        _hp = gameObject.GetComponent<HealthManager>();
        _phcon = gameObject.LocateMyFSM("Phase Control");

        ModifyRadiance();
    }

    private void Start()
    {
        knight = GameObject.Find("Knight");

        //获得激光的两个声音
        BeamAnticClip = _com.GetAction<AudioPlayerOneShotSingle>("Aim", 1).audioClip.Value as AudioClip;
        BeamFireClip = _com.GetAction<AudioPlayerOneShotSingle>("Aim", 3).audioClip.Value as AudioClip;
        orb = _com.GetAction<SpawnObjectFromGlobalPool>("Spawn Fireball", 1).gameObject.Value;
        sword = _com.GetAction<SpawnObjectFromGlobalPool>("CW Spawn", 0).gameObject.Value;
        glow = _com.FsmVariables.FindFsmGameObject("Eye Beam Glow").Value;
        beam = GameObject.Instantiate(gameObject.transform.parent.Find("Radiant Beam").gameObject);
        beam.SetActive(false);
        for (int i = 0; i < 6; i++)
        {
            var nb = GameObject.Instantiate(beam);
            nb.SetActive(false);
            beams.Add(nb);
        }



        ModifyOrbs();
        ModifyNailFan();
        ModifyEyeBeams();
        ModifyNailWall();
        ModifySwordRain();
        ModifySpike();


        ModifyRage();

        // ModifyBeamSweeper();




        // ModifyAcend();
        // ModifyFinal();
        // ModifyScene();

        _con.InsertCustomAction("Tendrils1", () => { ModifyPlatsPhase(); }, 0);



        for (int i = 0; i < 73; i++)
        {
            useorbs.Add(Instantiate(_com.GetAction<SpawnObjectFromGlobalPool>("Spawn Fireball", 1).gameObject.Value));
            useorbs[i].LocateMyFSM("Orb Control").RemoveTransition("Stop Particles", "FINISHED");
            useorbs[i].LocateMyFSM("Orb Control").GetAction<AudioPlaySimple>("Init", 0).volume = 0.1f;
        }
        useorbs[0].LocateMyFSM("Orb Control").GetAction<SetDamageHeroAmount>("Init", 5).damageDealt = 0;
        useorbs[0].LocateMyFSM("Orb Control").GetAction<SetDamageHeroAmount>("Dissipate", 3).damageDealt = 0;


        foreach (var orb in useorbs)
        {
            orb.LocateMyFSM("Orb Control").InsertCustomAction("Stop Particles", () =>
            {
                orb.SetActive(false);
            }, 3);
            orb.SetActive(false);
        }

    }



    private IEnumerator BeamFire(GameObject beam, float antic, float fire, float end, bool clip = true)
    {
        AudioSource aus;
        aus = beam.GetAddComponent<AudioSource>();
        beam.SetActive(true);
        PlayMakerFSM beamFSM = beam.LocateMyFSM("Control");
        if (clip)
            aus.PlayOneShot(BeamAnticClip, 0.5f * GameManager.instance.gameSettings.soundVolume / 10f);
        beamFSM.SendEvent("ANTIC");
        yield return new WaitForSeconds(antic);
        if (clip)
            aus.PlayOneShot(BeamFireClip, 0.5f * GameManager.instance.gameSettings.soundVolume / 10f);
        beamFSM.SendEvent("FIRE");
        yield return new WaitForSeconds(fire);
        beamFSM.SendEvent("END");
        yield return new WaitForSeconds(end);

        yield break;
    }

    private IEnumerator beginbeams(GameObject orb)
    {
        GameObject beam = Instantiate(oribeam);
        GameObject beam2 = Instantiate(oribeam);
        beam.transform.SetRotation2D(0);
        beam2.transform.SetRotation2D(0);
        beam.transform.position = orb.transform.position;
        beam2.transform.position = orb.transform.position;
        //beam.transform.position += new Vector3(0, -20f, 0);
        float f = UnityEngine.Random.Range(0f, 180f);
        if (_cho.GetVariable<FsmInt>("Arena").Value != 1)
        {
            float num = knight.transform.position.y - orb.transform.position.y;
            float num2 = knight.transform.position.x - orb.transform.position.x;
            float num3;
            for (num3 = Mathf.Atan2(num, num2) * (180f / (float)Math.PI); num3 < 0f; num3 += 360f)
            {
            }
            f = num3;
        }
        beam.transform.SetRotation2D(f);
        beam2.transform.SetRotation2D(180f + f);
        //beams[i].transform.Translate((float)(-20f * Math.Cos(f)), (float)(-20f * Math.Sin(f)), 0,Space.Self);
        beam.SetActive(true);
        beam2.SetActive(true);
        if (_cho.GetVariable<FsmInt>("Arena").Value != 1)
        {
            a2beams.Add(beam);
            a2beams.Add(beam2);
        }
        else
        {
            StartCoroutine(BeamFire(beam, 1.0f, 0.3f, 0f));
            StartCoroutine(BeamFire(beam2, 1.0f, 0.3f, 0f));
        }

        yield break;
        //StartCoroutine(beginbeam(beams[i],knight.transform.position,transform));

    }

    private IEnumerator beginsweeper(bool left, float floor)
    {

        float ll, lr, rl, rr;
        float le, ri;
        if (floor < 30f)
        {
            ll = 43f; lr = 45f;
            rl = 78f; rr = 80f;
        }
        else
        {
            ll = 32f; lr = 34f;
            rl = 83f; rr = 85f;
        }
        le = UnityEngine.Random.Range(ll, lr);
        ri = UnityEngine.Random.Range(rl, rr);
        if (left)
        {
            for (float i = le; i <= ri; i += 2.5f)
            {
                GameObject beam = Instantiate(oribeam);
                beam.transform.position = new Vector3(i, floor);
                beam.transform.SetRotation2D(90f);
                StartCoroutine(BeamFire(beam, 1.0f, 0.5f, 0f));
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            for (float i = ri; i >= le; i -= 2.5f)
            {
                GameObject beam = Instantiate(oribeam);
                beam.transform.position = new Vector3(i, floor - 5f);
                beam.transform.SetRotation2D(90f);
                StartCoroutine(BeamFire(beam, 1.0f, 0.5f, 0f));
                yield return new WaitForSeconds(0.05f);
            }
        }
        yield break;

    }

    private IEnumerator SendEventToNail(GameObject nail, float delay, string s)
    {
        yield return new WaitForSeconds(delay);
        nail.LocateMyFSM("Control").SendEvent(s);
        yield break;
    }
    private void SendEventToBB(GameObject go, string eve)
    {
        if (go != null)
        {
            List<GameObject> list = new List<GameObject>();
            go.FindAllChildren(list);
            foreach (GameObject go2 in list)
            {
                PlayMakerFSM bbcon;
                bbcon = go2.LocateMyFSM("Control");

                bbcon.SendEvent(eve);
            }
            list.Clear();
        }
    }


    private IEnumerator BBrorate(GameObject eb, float v, float time, bool random)
    {
        float timer = 0f;
        if (UnityEngine.Random.Range(0, 2) == 0 || random)
        {
            while (timer <= time)
            {
                timer += Time.deltaTime;
                eb.transform.Rotate(Vector3.back, -v * Time.deltaTime, Space.Self);
                yield return null;
            }
        }
        else
        {
            while (timer <= time)
            {
                timer += Time.deltaTime;
                eb.transform.Rotate(Vector3.back, v * Time.deltaTime, Space.Self);
                yield return null;
            }
        }
        yield break;
    }



    private void ModifyScene()
    {
        Scene scene = SceneUtils.getCurrentScene();
        string[] strings = { "GG_scenery_0004_17 (80)", "GG_scenery_0004_17 (86)", "GG_scenery_0004_17 (87)", "GG_scenery_0004_17 (89)", "GG_scenery_0004_17 (90)" };
        foreach (var go in scene.GetAllGameObjects())
        {
            if (go.name.IsAny(strings))
            {
                go.SetActive(false);
            }
        }
    }


    private void ModifyBeamSweeper()
    {
        _cho.GetAction<SendRandomEventV3>("A1 Choice", 1).weights[6] = 0.5f;
        _cho.GetAction<SendRandomEventV3>("A1 Choice", 1).weights[7] = 0.5f;
        _cho.RemoveAction("Beam Sweep L", 1);
        _cho.InsertCustomAction("Beam Sweep L", () =>
        {
            StartCoroutine(beginsweeper(true, 20f));
        }, 1);
        _cho.RemoveAction("Beam Sweep R", 1);
        _cho.InsertCustomAction("Beam Sweep R", () =>
        {
            StartCoroutine(beginsweeper(false, 20f));
        }, 1);
        _cho.RemoveAction("Beam Sweep L 2", 1);
        _cho.InsertCustomAction("Beam Sweep L 2", () =>
        {
            StartCoroutine(beginsweeper(true, 30f));
        }, 1);
        _cho.RemoveAction("Beam Sweep R 2", 1);
        _cho.InsertCustomAction("Beam Sweep R 2", () =>
        {
            StartCoroutine(beginsweeper(false, 30f));
        }, 1);

    }

    private void ModifyRage()
    {
        _con.RemoveAction("Rage Comb", 1);
        _con.RemoveAction("Rage Comb", 1);
        IEnumerator FiveOrb()
        {
            List<GameObject> orbs = new();
            for (int i = 0; i < 5; i++)
            {
                var norb = orb.Spawn();
                norb.transform.position = new Vector3(42 + 7 * i, 35);
                norb.SetActive(true);
                orbs.Add(norb);
            }
            float timer = 0;
            while (timer < 2f)
            {
                timer += Time.deltaTime;
                foreach (var orb in orbs)
                {
                    orb.transform.Translate(0, -3 * Time.deltaTime, 0, Space.World);
                }
                yield return null;
            }
            foreach (var orb in orbs)
            {
                orb.LocateMyFSM("Orb Control").SendEvent("FIRE");
            }
        }
        _con.GetAction<Wait>("Rage Comb", 0).time = 1.7f;
        _con.AddCustomAction("Rage Comb", () =>
        {
            StartCoroutine(FiveOrb());
        });
        // _con.ChangeTransition("Rage1 Start", "FINISHED", "Rage Eyes");
        // _com.GetAction<SendEventByName>("EB 4", 3).delay = 0.7f;
        // _com.GetAction<SendEventByName>("EB 4", 4).delay = 1.0f;
        // _com.GetAction<SendEventByName>("EB 5", 4).delay = 0.7f;
        // _com.GetAction<SendEventByName>("EB 5", 5).delay = 1.0f;
        // _com.GetAction<SendEventByName>("EB 6", 4).delay = 0.7f;
        // _com.GetAction<SendEventByName>("EB 6", 5).delay = 1.0f;
        // _con.InsertCustomAction("Climb Plats1", () =>
        // {
        //     _com.ChangeTransition("EB 4", "FINISHED", "EB Glow End 2");
        //     _com.ChangeTransition("EB 5", "FINISHED", "EB Glow End 2");
        //     _com.ChangeTransition("EB 6", "FINISHED", "EB Glow End 2");
        //     _com.ChangeTransition("EB Glow End 2", "FINISHED", "Comb Top");
        //     _com.GetVariable<FsmGameObject>("Eye Beam Glow").Value.SetActive(false);
        // }, 0);
        // _con.RemoveAction("Stun1 Start", 12);
        // _con.RemoveAction("Stun1 Out", 8);
    }
    Vector2 Angle2Vector2(float angle, float length)
    {
        return length * new Vector2(Mathf.Cos(angle * Mathf.PI / 180f), Mathf.Sin(angle * Mathf.PI / 180f));
    }
    private void ModifyOrbs()
    {
        IEnumerator Orbs2(PlayMakerFSM fsm)
        {
            var shot_charge = _com.FsmVariables.FindFsmGameObject("Shot Charge").Value;
            var shot_charge_2 = _com.FsmVariables.FindFsmGameObject("Shot Charge 2").Value;
            float angle;
            float r = 5f;
            bool flag = false;
            do
            {
                flag = false;
                angle = UnityEngine.Random.Range(-360f, 0f);
                var tp = Angle2Vector2(angle, r);
                Vector2 self_pos = transform.position;
                if (Vector2.Distance(self_pos + tp, knight.transform.position) < 5f)
                {
                    flag = true;
                }
            }
            while (flag);
            IEnumerator SingleOrb(Vector3 pos, float delay, float stay)
            {
                var new_shot = GameObject.Instantiate(shot_charge);
                var new_shot_2 = GameObject.Instantiate(shot_charge_2);
                new_shot.transform.position = pos;
                new_shot_2.transform.position = pos;
                new_shot.GetComponent<ParticleSystem>().enableEmission = true;
                new_shot_2.GetComponent<ParticleSystem>().enableEmission = true;
                yield return new WaitForSeconds(0.75f);
                new_shot.GetComponent<ParticleSystem>().enableEmission = false;
                new_shot_2.GetComponent<ParticleSystem>().enableEmission = false;
                Destroy(new_shot);
                Destroy(new_shot_2);
                var new_orb = orb.Spawn(pos);
                new_orb.transform.position = pos;
                yield return new WaitForSeconds(3f);
                new_orb.LocateMyFSM("Orb Control").SendEvent("FIRE");
            }
            Coroutine coroutine = null;
            for (int i = 0; i < 6; i++, angle += 60f)
            {
                var tp = Angle2Vector2(angle, r);
                Vector3 pos = base.transform.position + new Vector3(tp.x, tp.y);
                pos.z = -0.1f;
                coroutine = StartCoroutine(SingleOrb(pos, 0.75f, 3f));
                yield return new WaitForSeconds(0.3f);
            }
            yield return coroutine;
            fsm.SendEvent("OVER");
            yield break;
        }
        void Add3LittleOrb(PlayMakerFSM o_fsm)
        {
            float angle = UnityEngine.Random.Range(0, 360f);
            float v = 5f;
            for (int i = 0; i < 3; i++)
            {
                float new_angle = angle + i * 120f;
                GameObject new_little_orb = orb.Spawn(o_fsm.transform.position);
                new_little_orb.LocateMyFSM("Orb Control").RemoveAction("Recycle", 0);
                new_little_orb.LocateMyFSM("Orb Control").AddAction("Recycle", new DestroySelf());
                new_little_orb.LocateMyFSM("Orb Control").GetAction<SetDamageHeroAmount>("Init", 5).damageDealt = 1;
                new_little_orb.LocateMyFSM("Orb Control").GetAction<SetScale>("Init", 2).x = 0.7f;
                new_little_orb.LocateMyFSM("Orb Control").GetAction<SetScale>("Init", 2).y = 0.7f;
                new_little_orb.LocateMyFSM("Orb Control").GetAction<SetIsKinematic2d>("Init", 1).isKinematic = false;
                new_little_orb.SetActive(false);
                new_little_orb.SetActive(true);
                new_little_orb.GetComponent<Rigidbody2D>().velocity = new Vector2(v * Mathf.Cos(new_angle / 180f * Mathf.PI), v * Mathf.Sin(new_angle / 180f * Mathf.PI));
                new_little_orb.LocateMyFSM("Orb Control").SendEvent("FIRE");
            }
        }

        _com.InsertCustomAction("Spawn Fireball", (fsm) =>
        {
            var orb = fsm.FsmVariables.FindFsmGameObject("Projectile").Value;
            var final_fsm = orb.LocateMyFSM("Final Control");
            final_fsm.RemoveAction("Recycle", 0);
            final_fsm.AddAction("Recycle", new DestroySelf());
            var orb_fsm = orb.LocateMyFSM("Orb Control");
            orb_fsm.RemoveAction("Recycle", 0);
            orb_fsm.AddAction("Recycle", new DestroySelf());
            orb_fsm.InsertCustomAction("Impact", Add3LittleOrb, 0);
            orb_fsm.InsertCustomAction("Stop Particles", Add3LittleOrb, 0);
        }, 2);
        //Orb2替代BeamSweepL
        var orb_wait_2 = _cho.CopyState("Orb Wait", "Orb Wait 2");
        var orbs_2 = _cho.CopyState("Orbs", "Orbs 2");
        var orb_recover = _cho.CopyState("Orb Recover", "Orb Recover 2");
        _cho.ChangeTransition("A1 Choice", "BEAM SWEEP L", "Orb Wait 2");
        orb_wait_2.ChangeTransition("CAST READY", "Orbs 2");
        orbs_2.ChangeTransition("ATTACK END", "Orb Recover 2");
        orb_recover.ChangeTransition("FINISHED", "Return");
        orbs_2.GetAction<SendEventByName>(0).sendEvent = "ORBS2";
        var com_orb2 = _com.AddState("Orb2");
        _com.AddGlobalTransition("ORBS2", "Orb2");
        com_orb2.AddTransition("OVER", "Orb End");
        _com.AddCustomAction("Orb2", (com) =>
        {
            base.StartCoroutine(Orbs2(com));
        });





    }

    private void ModifyEyeBeams()
    {
        //前摇0.425s
        //攻击0.3s
        var nuclear_beam = _com.AddState("Nuclear Beam");
        _com.ChangeTransition("NF Glow", "FINISHED", "Nuclear Beam");
        nuclear_beam.AddTransition("OVER", "EB End");
        _com.AddCustomAction("Nuclear Beam", (fsm) =>
        {
            StartCoroutine(NuclearBeamBurst(fsm));
        });

        IEnumerator NuclearBeamBurst(PlayMakerFSM fsm)
        {
            GameObject bb = _com.FsmVariables.FindFsmGameObject("Eye Beam Burst1").Value;
            // yield return new WaitForSeconds(5.7f);
            float angle = UnityEngine.Random.Range(0, 360f);
            bb.transform.SetRotation2D(angle);
            SendEventToBB(bb, "ANTIC");
            yield return new WaitForSeconds(0.425f);
            SendEventToBB(bb, "FIRE");
            for (int i = 0; i < 10; i++, angle += 5f)
            {
                bb.transform.SetRotation2D(angle);
                yield return new WaitForSeconds(0.1f);
                //TODO beam logic
            }
            SendEventToBB(bb, "END");
            fsm.SendEvent("OVER");
        }

    }

    private void ModifyNailWall(bool rel)
    {
        _com.GetAction<SetFsmInt>("Comb L 2", 1).setValue = 1;
        _com.GetAction<SetFsmInt>("Comb R 2", 1).setValue = 2;
        _cho.GetAction<SendEventByName>("Nail Top Sweep", 0).sendEvent = "COMB L2";
        _cho.GetAction<SendEventByName>("Nail Top Sweep", 1).sendEvent = "COMB R2";
        _cho.GetAction<SendEventByName>("Nail Top Sweep", 2).sendEvent = "COMB L2";
        _cho.GetAction<SendEventByName>("Nail Top Sweep", 3).sendEvent = "COMB R2";

        _cho.GetAction<SendEventByName>("Nail L Sweep", 1).sendEvent = "COMB R2";
        _cho.GetAction<SendEventByName>("Nail L Sweep", 1).delay = 1.2f;
        _cho.GetAction<SendEventByName>("Nail L Sweep", 2).sendEvent = "COMB TOP";



        _cho.GetAction<SendEventByName>("Nail R Sweep", 1).sendEvent = "COMB L2";
        _cho.GetAction<SendEventByName>("Nail R Sweep", 1).delay = 1.2f;
        _cho.GetAction<SendEventByName>("Nail R Sweep", 2).sendEvent = "COMB TOP";



    }

    class StopWhenHitTerrain : MonoBehaviour
    {
        bool stay = false;
        void OnEnable()
        {
            var rb = base.GetComponent<Rigidbody2D>();


        }
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (!stay)
                if (collider.gameObject.layer == 8)
                    base.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        void OnTriggerStay2D(Collider2D collider)
        {
            if (!stay)
                if (collider.gameObject.layer == 8)
                    base.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        void OnDisable()
        {
            Destroy(base.gameObject);
        }
    }
    private void ModifyNailWall()
    {
        IEnumerator SwordBurst()
        {
            yield return new WaitForSeconds(1.7f);
            float f1 = 49.82f;
            float f2 = 71.78f;
            float mid = (f1 + f2) / 2;

            var now_sword = sword.Spawn();
            float x;
            if (HeroController.instance.transform.position.x > mid)
                x = Random.Range(mid, f2);
            else
                x = Random.Range(f1, mid);
            now_sword.transform.position = new Vector3(x, 30f, glow.transform.position.z);
            now_sword.transform.SetRotation2D(180f);
            now_sword.SetActive(true);
            now_sword.LocateMyFSM("Control").SendEvent("FLY");
            now_sword.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
            now_sword.GetAddComponent<StopWhenHitTerrain>();
            yield return new WaitUntil(() => now_sword.GetComponent<Rigidbody2D>().velocity == Vector2.zero);
            float timer = 0;
            while (timer < 10f)
            {
                now_sword.GetComponent<tk2dSprite>().color = Color.Lerp(Color.white, Color.red, timer / 10f);
                timer += Time.deltaTime;
                yield return null;
            }
            GameObject bb = GameObject.Instantiate(_com.FsmVariables.FindFsmGameObject("Eye Beam Burst2").Value);
            bb.transform.position = now_sword.transform.position;
            bb.SetActive(true);
            float angle = UnityEngine.Random.Range(0, 360f);
            bb.transform.SetRotation2D(angle);
            SendEventToBB(bb, "ANTIC");
            yield return new WaitForSeconds(0.425f);
            SendEventToBB(bb, "FIRE");
            yield return new WaitForSeconds(0.3f);
            SendEventToBB(bb, "END");
            now_sword.GetComponent<tk2dSprite>().color = Color.white;
            now_sword.LocateMyFSM("Control").SendEvent("NAIL END");
            yield return new WaitForSeconds(0.5f);
            Destroy(bb);
        }

        _cho.InsertCustomAction("Nail L Sweep", () =>
            {
                StartCoroutine(SwordBurst());
            }, 0);
        _cho.InsertCustomAction("Nail R Sweep", () =>
        {
            StartCoroutine(SwordBurst());
        }, 0);
    }

    public static float DoGetAngle(Vector2 self, Vector2 target)
    {
        float num = target.y - self.y;
        float num2 = target.x - self.x;
        float num3;
        for (num3 = Mathf.Atan2(num, num2) * (180f / (float)Math.PI); num3 < 0f; num3 += 360f)
        {
        }
        return num3;
    }
    GameObject flag_sword = null;
    private void ModifyNailFan()
    {
        var antic = _com.AddState("Sword Antic");
        var left = _com.AddState("Sword Left Attack");
        var right = _com.AddState("Sword Right Attack");
        antic.AddTransition("TELEPORTED", "Sword Left Attack");
        left.AddTransition("TELEPORTED", "Sword Right Attack");
        right.AddTransition("TELEPORTED", "End");
        _com.ChangeTransition("Nail Fan", "FINISHED", "Sword Antic");
        antic.AddCustomAction(() =>
        {
            _tele.FsmVariables.FindFsmVector3("Destination").Value = new Vector3(49.82f, 29.17f, 0.006f);
            _tele.SendEvent("TELEPORT");
        });
        left.AddCustomAction(() =>
        {
            _tele.FsmVariables.FindFsmVector3("Destination").Value = new Vector3(71.78f, 29.17f, 0.006f);
            StartCoroutine(SwordAttack());

        });
        right.AddCustomAction(() =>
        {
            float x;
            do
            {
                x = Random.Range(49.82f, 71.78f);
            }
            while (71.78f - x < 5f);

            _tele.FsmVariables.FindFsmVector3("Destination").Value = new Vector3(71.78f, 29.17f, 0.006f);
            StartCoroutine(SwordAttack());
        });

        IEnumerator SwordAttack()
        {
            yield return null;
            _tele.SendEvent("TELEPORT");
            float angle = DoGetAngle(glow.transform.position, knight.transform.position);
            angle -= 90f;
            bool cw = UnityEngine.Random.Range(0, 2) == 0;
            float add = 0f;
            if (cw)
            {
                angle += 30f;
                add = -30f;
            }
            else
            {
                angle -= 30f;
                add = 30f;
            }
            List<GameObject> swords = new();
            for (int i = 0; i < 3; i++, angle += add)
            {
                var now_sword = sword.Spawn(glow.transform.position);
                now_sword.SetActive(true);
                now_sword.transform.SetRotation2D(angle);
                swords.Add(now_sword);
                yield return new WaitForFixedUpdate();
                now_sword.LocateMyFSM("Control").SendEvent("FAN ANTIC");
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var sword in swords)
            {
                if (cw)
                {
                    sword.LocateMyFSM("Control").SendEvent("FAN ATTACK CW");
                }
                else
                {
                    sword.LocateMyFSM("Control").SendEvent("FAN ATTACK CCW");
                }
            }
            yield break;
        }

        var nail_fan_wait = _cho.CopyState("Nail Fan Wait", "Nail Fan Wait 2");
        var nail_fan = _cho.CopyState("Nail Fan", "Nail Fan 2");
        var nail_fan_recover = _cho.CopyState("Nail Fan Recover", "Nail Fan Recover 2");
        _cho.ChangeTransition("A1 Choice", "BEAM SWEEP R", "Nail Fan Wait 2");
        nail_fan_wait.ChangeTransition("CAST READY", "Nail Fan 2");
        nail_fan.ChangeTransition("ATTACK END", "Nail Fan Recover 2");
        nail_fan_recover.ChangeTransition("FINISHED", "Return");
        nail_fan.GetAction<SendEventByName>(0).sendEvent = "NAIL FAN 2";
        var com_nail_fan2 = _com.AddState("Nail Fan 2");
        _com.AddGlobalTransition("NAIL FAN 2", "Nail Fan 2");
        com_nail_fan2.AddTransition("OVER", "End");
        _com.AddCustomAction("Nail Fan 2", (com) =>
        {
            StartCoroutine(FaceSwordAttack());

        });
        slash_inst = GameObject.Instantiate(slash_go);
        slash_inst.transform.SetParent(base.transform);
        slash_inst.SetActive(true);
        flag_sword = sword.Spawn();
        flag_sword.SetActive(false);
        for (int i = 1; i <= 3; i++)
        {

            GameObject slash_effect = new("slash_effect");
            slash_effect.transform.SetParent(slash_inst.FindGameObjectInChildren("Slash" + i).transform);
            slash_effect.transform.localPosition = new Vector3(0, 0, 0);
            slash_effect.AddComponent<tk2dSprite>().color = new Color(1f, 1f, 0.388f, 1);
            slash_effect.AddComponent<tk2dSpriteAnimator>().Library = knight.GetComponent<tk2dSpriteAnimator>().Library;
            var deac = slash_effect.AddComponent<DeactiveWhileEndAnime>();
            deac.behaviours.Add(slash_effect.transform.parent.GetComponent<PolygonCollider2D>());
            slash_effect.SetActive(false);

            var t_nail = sword.Spawn();
            t_nail.name = "sword";
            t_nail.transform.SetParent(slash_inst.FindGameObjectInChildren("Slash" + i).transform);
            t_nail.transform.localPosition = new Vector3(0, 0, 0);
            t_nail.LocateMyFSM("Control").InsertAction("Recycle", new DestroySelf(), 0);
            t_nail.LocateMyFSM("Control").RemoveAction("Recycle", 1);
            t_nail.LocateMyFSM("Control").AddCustomAction("Appear", (fsm) =>
            {
                var aus = fsm.gameObject.GetAddComponent<AudioSource>();
                aus.clip = BeamAnticClip;
                aus.Play();
            });
            var wait = t_nail.LocateMyFSM("Control").AddState("Wait");
            wait.AddAction(new Wait() { time = 0.5f });
            wait.AddTransition("FINISHED", "Set Collider");
            t_nail.LocateMyFSM("Control").ChangeTransition("Appear", "FINISHED", "Wait");
            switch (i)
            {
                case 1:
                    slash_effect.transform.SetScaleX(1.1594f);
                    slash_effect.transform.localPosition = new Vector3(-2.6764f, -1.1455f, 0);
                    deac.enable_action = (anime) =>
                    {
                        if (anime != null)
                        {
                            anime.Play(anime.GetClipByName("NA Big Slash Effect"), 0f, 80);
                        }
                    };
                    t_nail.LocateMyFSM("Control").InsertCustomAction("Set Collider", (fsm) =>
                    {
                        fsm.GetComponent<AudioSource>().Stop();
                        fsm.GetComponent<AudioSource>().PlayOneShot(BeamFireClip);
                        fsm.gameObject.transform.parent.GetComponent<PolygonCollider2D>().enabled = true;
                        fsm.gameObject.transform.parent.gameObject.FindGameObjectInChildren("slash_effect").SetActive(true);
                        fsm.gameObject.SetActive(false);
                    }, 1);
                    break;
                case 2:
                    var collider = slash_effect.transform.parent.GetComponent<PolygonCollider2D>();
                    for (int j = 0; j < 4; j++)
                    {
                        collider.points[j].y = collider.points[j].y - 1;
                    }
                    slash_effect.transform.localPosition = new Vector3(0, -1, 0);
                    slash_effect.transform.SetScaleX(1.1594f);
                    slash_effect.transform.SetScaleY(2.1f);
                    deac.enable_action = (anime) =>
                        {
                            if (anime != null)
                            {
                                anime.Play(anime.GetClipByName("NA Dash Slash Effect"), 0, 40);
                            }
                        };
                    t_nail.LocateMyFSM("Control").InsertCustomAction("Set Collider", (fsm) =>
                    {
                        fsm.GetComponent<AudioSource>().Stop();
                        fsm.GetComponent<AudioSource>().PlayOneShot(BeamFireClip);
                        fsm.gameObject.transform.parent.GetComponent<PolygonCollider2D>().enabled = true;
                        fsm.gameObject.transform.parent.gameObject.FindGameObjectInChildren("slash_effect").SetActive(true);
                        fsm.gameObject.SetActive(false);
                    }, 1);
                    break;
                case 3:
                    slash_effect.transform.localPosition = new Vector3(-0.4f, -0.4f, 0);
                    slash_effect.transform.SetScaleX(1.7594f);
                    slash_effect.transform.SetScaleY(1.8f);
                    var collider2 = slash_effect.transform.parent.GetComponent<PolygonCollider2D>();
                    for (int j = 6; j < 8; j++)
                    {
                        collider2.points[j].y = collider2.points[j].y + 1;
                    }
                    deac.enable_action = (anime) =>
                            {
                                if (anime != null)
                                {
                                    anime.Play(anime.GetClipByName("Cyclone Effect"), 0, 40);
                                }
                            };
                    deac.need_timer = true;
                    deac.timer = 0.25f;
                    t_nail.LocateMyFSM("Control").InsertCustomAction("Set Collider", (fsm) =>
                    {
                        fsm.GetComponent<AudioSource>().Stop();
                        fsm.GetComponent<AudioSource>().PlayOneShot(BeamFireClip);
                        fsm.gameObject.transform.parent.GetComponent<PolygonCollider2D>().enabled = true;
                        fsm.gameObject.transform.parent.gameObject.FindGameObjectInChildren("slash_effect").SetActive(true);
                        fsm.gameObject.SetActive(false);
                    }, 1);
                    break;
            }
            t_nail.SetActive(false);
        }
        IEnumerator FaceSwordAttack()
        {
            tk2dSpriteAnimator ani = base.GetComponent<tk2dSpriteAnimator>();
            Vector3 pos = base.transform.position;
            flag_sword.transform.position = base.transform.position + new Vector3(-0.2f, 3f, 0);
            flag_sword.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0.8f);
            flag_sword.SetActive(true);
            flag_sword.LocateMyFSM("Control").SetState("Appear");
            yield return new WaitForSeconds(0.5f);
            int direction = -1;
            if (knight.transform.position.x < (49.82f + 71.78f) / 2)//小骑士在左，辐光右侧出现
            {
                direction = -1;
            }
            else//小骑士在右，辐光左侧出现
            {
                direction = 1;
            }
            for (int i = 1; i <= 3; i++)
            {
                var slash = slash_inst.FindGameObjectInChildren("Slash" + i);
                if (slash != null)
                {
                    slash.transform.SetScaleX(-0.8625f * direction);
                    slash.transform.position = knight.transform.position + new Vector3(-4 * direction, 0, 0);
                    slash.FindGameObjectInChildren("sword").SetActive(true);
                    slash.FindGameObjectInChildren("sword").LocateMyFSM("Control").SetState("Appear");
                    direction *= -1;
                    yield return new WaitForSeconds(0.8f);
                }
            }
            _com.SendEvent("OVER");
            flag_sword.SetActive(false);
            yield break;

            ani.Play("Tele Out");
            yield return new WaitForSeconds(0.2f);
            float z = base.transform.position.z;
            base.transform.position = knight.transform.position + new Vector3(-3 * direction, 0, 0);
            base.transform.SetPositionZ(z);
            base.transform.SetRotation2D(-20);
            base.transform.SetScaleX(direction);
            float timer = 0;
            int phase = 0;
            var effect_animator = slash_effect.GetComponent<tk2dSpriteAnimator>();
            var effect_mesh = slash_effect.GetComponent<MeshRenderer>();
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                base.transform.Translate(direction * 3f * Time.deltaTime, 0, 0, Space.World);
                if (timer >= 0f && phase == 0)
                {
                    ani.Play("Tele In");
                    effect_mesh.enabled = true;
                    effect_animator.Play(effect_animator.GetClipByName("NA Big Slash Effect"), 0f, 80);
                    slash_inst.FindGameObjectInChildren("Slash1").GetComponent<PolygonCollider2D>().enabled = true;
                    phase = 1;
                }
                if (timer >= 0.2f && phase == 1)
                {
                    effect_mesh.enabled = false;
                    slash_inst.FindGameObjectInChildren("Slash1").GetComponent<PolygonCollider2D>().enabled = false;
                }
                if (timer >= 0.3f && phase == 1)
                {
                    ani.Play("Tele Out");
                    effect_mesh.enabled = true;
                    effect_animator.Play(effect_animator.GetClipByName("NA Dash Slash Effect"), 0, 40);
                    slash_inst.FindGameObjectInChildren("Slash2").GetComponent<PolygonCollider2D>().enabled = true;
                    phase = 2;
                }
                if (timer >= 0.4f && phase == 2)
                {
                    effect_mesh.enabled = false;
                    slash_inst.FindGameObjectInChildren("Slash2").GetComponent<PolygonCollider2D>().enabled = false;
                }
                if (timer >= 0.6f && phase == 2)
                {
                    ani.Play("Tele In");
                    effect_mesh.enabled = true;
                    effect_animator.Play("NA Dash Slash Effect");
                    slash_inst.FindGameObjectInChildren("Slash3").GetComponent<PolygonCollider2D>().enabled = true;
                    phase = 3;
                }
                if (timer >= 0.7f && phase == 3)
                {
                    slash_inst.FindGameObjectInChildren("Slash3").GetComponent<PolygonCollider2D>().enabled = false;
                }
                yield return null;



            }
            base.transform.SetScaleX(1);
            base.transform.SetRotation2D(0);
            base.transform.position = pos;
            ani.Play("Cast");
            _com.SendEvent("OVER");
        }
        //TODO把前辈的三连斩搬过来并与辐光的两个动画相匹配
    }

    private void ModifySwordRain()
    {
        IEnumerator BeamAttack(bool left)
        {
            List<float> positions = new();
            for (int i = 0; i < 6; i++)
            {
                float x;
                bool flag = false;
                do
                {
                    flag = false;
                    x = Random.Range(48f, 75f);
                    foreach (var lx in positions)
                    {
                        Log(lx + " " + x);
                        if (Mathf.Abs(x - lx) < 3f)
                        {
                            flag = true;
                            break;
                        }
                    }
                } while (flag);
                positions.Add(x);
            }
            if (left)
                positions.Sort();
            else
                positions.Sort((x, y) => x > y ? -1 : 1);
            for (int i = 0; i < 6; i++)
            {
                beams[i].transform.position = new Vector3(positions[i], 20);
                beams[i].transform.SetScaleX(20);
                beams[i].transform.SetRotation2D(90);
                beams[i].SetActive(true);
                StartCoroutine(BeamFire(beams[i], 0.5f + i * 0.3f, 0.3f, 0));
            }
            yield break;
        }
        for (int i = 0; i < 4; i++)
        {
            _cho.RemoveAction("Nail Top Sweep", 0);
        }
        IEnumerator BeamRain()
        {
            bool direction = true;
            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(BeamAttack(direction));
                direction = !direction;
                yield return new WaitForSeconds(3f);
            }
        }
        _cho.InsertCustomAction("Nail Top Sweep", () =>
        {
            Log("Nail Top Sweep");
            StartCoroutine(BeamRain());
        }, 0);
    }
    private void ModifySpike()
    {
        PlayMakerFSM _spikecon = GameObject.Find("Spike Control").LocateMyFSM("Control");
        List<GameObject> spikes = new List<GameObject>();
        spikes.Add(GameObject.Find("Radiant Spike"));
        for (int i = 1; i <= 24; i++)
        {
            string name = "Radiant Spike " + "(" + i + ")";
            spikes.Add(GameObject.Find(name));
        }
        spikes.Sort((a, b) => { return a.transform.GetPositionX() < b.transform.GetPositionX() ? 1 : -1; });
        if (spike_beams == null) spike_beams = new();
        spike_beams.Clear();
        foreach (GameObject spike in spikes)
        {
            var tbeam = GameObject.Instantiate(beam);
            tbeam.SetActive(true);
            tbeam.transform.position = spike.transform.position - new Vector3(0, 3, 0);
            tbeam.transform.SetScaleX(20);
            tbeam.transform.SetRotation2D(90);
            spike_beams.Add(tbeam);
            spike.LocateMyFSM("Control").GetAction<Wait>("Floor Antic", 2).time = 2f;
            spike.LocateMyFSM("Hero Saver").RemoveAction("Send", 0);
            spike.LocateMyFSM("Control").GetAction<SetMeshRenderer>("Spike Up", 2).active = false;
            spike.LocateMyFSM("Control").GetAction<SetPolygonCollider>("Up", 0).active = false;
            spike.LocateMyFSM("Control").AddVariable<FsmInt>("beam_num");
            spike.LocateMyFSM("Control").GetVariable<FsmInt>("beam_num").Value = spikes.IndexOf(spike);
            spike.LocateMyFSM("Control").InsertCustomAction("Up", (fsm) =>
            {
                base.StartCoroutine(BeamFire(spike_beams[fsm.FsmVariables.FindFsmInt("beam_num").Value], 0.5f, 0.3f, 0, false));
                fsm.SendEvent("DOWN");
            }, 0);
            // spike.LocateMyFSM("Control").InsertCustomAction("Send", () =>将过去的余念弃置于此
            // {

            //     {
            //         int j = spikes.IndexOf(spike) + 1;
            //         int i = spikes.IndexOf(spike) - 1;
            //         while (j < spikes.Count)
            //         {
            //             string activename = spikes[j].LocateMyFSM("Control").ActiveStateName;
            //             if (activename == "Down" || activename == "Downed") break;
            //             else
            //             {
            //                 spikes[j].LocateMyFSM("Control").SendEvent("DOWN");
            //             }
            //             j++;
            //         }
            //         while (i >= 0)
            //         {
            //             string activename = spikes[i].LocateMyFSM("Control").ActiveStateName;
            //             if (activename == "Down" || activename == "Downed") break;
            //             else
            //             {
            //                 spikes[i].LocateMyFSM("Control").SendEvent("DOWN");
            //             }
            //             i--;
            //         }
            //         spike.LocateMyFSM("Control").SendEvent("DOWN");
            //     }
            // }, 0);
        }
        if (_spikecon != null)
        {
            for (int i = 2; i <= 6; i++)
            {
                _spikecon.RemoveAction("Wave L", 2);
                _spikecon.RemoveAction("Wave R", 2);
            }
            _spikecon.InsertCustomAction("Wave L", (fsm) =>
            {
                if (_hp.hp <= spikeWaves_HP - 400)
                {
                    fsm.GetAction<WaitRandom>("Wave L", 3).timeMin = 6;
                    fsm.GetAction<WaitRandom>("Wave L", 3).timeMax = 6;
                }
                SpikeBeamFire();
            }, 2);
            _spikecon.InsertCustomAction("Wave R", (fsm) =>
            {
                if (_hp.hp <= spikeWaves_HP - 400)
                {
                    fsm.GetAction<WaitRandom>("Wave R", 3).timeMin = 6;
                    fsm.GetAction<WaitRandom>("Wave R", 3).timeMax = 6;
                }
                SpikeBeamFire();
            }, 2);
            _spikecon.AddAction("Spikes Outer", new Wait() { time = 6f, finishEvent = FsmEvent.FindEvent("SPIKES DOWN") });
            void SpikeBeamFire()
            {
                List<int> ints = new();
                int supposed_count = general_random.Next(2, 5);
                if (_hp.hp <= spikeWaves_HP - 400)
                    supposed_count = general_random.Next(3, 6);
                while (ints.Count < supposed_count)
                {
                    int t = general_random.Next(0, spikes.Count);
                    if (!ints.Contains(t)) ints.Add(t);
                }
                foreach (int i in ints)
                {
                    spikes[i].LocateMyFSM("Control").SendEvent("UP");
                }
            }
        }
    }

    private void ModifyRadiance()
    {
        _hp.hp = initHp;
        _phcon.FsmVariables.FindFsmInt("P2 Spike Waves").Value = spikeWaves_HP;
        _phcon.FsmVariables.FindFsmInt("P3 A1 Rage").Value = a1Rage_HP;
        _phcon.FsmVariables.FindFsmInt("P4 Stun1").Value = stun_HP;
        _phcon.FsmVariables.FindFsmInt("P5 Acend").Value = p5Acend_HP;
        _con.GetAction<SetHP>("Scream", 7).hp = Scream_HP;
        _con.FsmVariables.FindFsmInt("Death HP").Value = death_HP;
        // _cho.GetAction<SendRandomEventV3>("A1 Choice", 1).weights = new FsmFloat[] { 0, 0, 0, 0, 0, 9999, 0, 0 };
        // _cho.GetAction<SendRandomEventV3>("A1 Choice", 1).missedMax = new FsmInt[] { 9999, 9999, 9999, 9999, 9999, 999, 999, 999 };
        // _cho.GetAction<SendRandomEventV3>("A1 Choice", 1).eventMax = new FsmInt[] { 0, 0, 0, 0, 0, 99999, 0, 0 };


    }

    private void ModifyPlatsPhase()
    {
        _cho.ChangeTransition("A2 Choice", "BEAM SWEEP L", "Orb Wait 2");
        _cho.ChangeTransition("A2 Choice", "BEAM SWEEP R", "Nail Fan Wait 2");

        return;
        //常态
        _com.RemoveTransition("Aim Back", "FINISHED");
        _com.GetAction<Wait>("AB Start", 1).time = 0.02f;

        _con.InsertCustomAction("Tele Cast?", () =>
        {

            if (_con.FsmVariables.FindFsmInt("Last Tele Pos").Value != 6)
            {
                float num = knight.transform.position.y - 1.5f - gameObject.transform.position.y;
                float num2 = knight.transform.position.x - gameObject.transform.position.x;
                float num3;
                for (num3 = Mathf.Atan2(num, num2) * (180f / (float)Math.PI); num3 < 0f; num3 += 360f)
                {
                }
                num3 += UnityEngine.Random.Range(-5f, 5f);
                GameObject beam = Instantiate(oribeam);
                beam.transform.position = gameObject.transform.position;
                beam.transform.SetPositionY(gameObject.transform.position.y + 1.5f);
                beam.transform.SetRotation2D(num3);
                StartCoroutine(BeamFire(beam, 1.0f, 0.5f, 0f));
            }
            else
            {
                GameObject beam = Instantiate(oribeam);
                beam.transform.position = gameObject.transform.position;
                beam.transform.SetPositionY(gameObject.transform.position.y + 1.5f);
                beam.transform.SetRotation2D(180f);
                StartCoroutine(BeamFire(beam, 0f, 2f, 0f));
                StartCoroutine(BBrorate(beam, 90f, 2f, true));
            }

        }, 0);


        //脸激光变光球圈
        _com.ChangeTransition("Eb Extra Wait", "FINISHED", "EB End");
        _com.GetAction<Wait>("Eb Extra Wait", 0).time = 3.5f;
        _com.InsertCustomAction("Eb Extra Wait", () =>
        {

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                StartCoroutine(OrbCircle(gameObject, true));
            }
            else
            {
                StartCoroutine(OrbCircle(knight, false));
            }



        }, 0);


        //光球激光速度减慢
        _cho.GetAction<SendRandomEventV3>("A2 Choice", 1).weights[2] = 0.5f;
        _com.GetAction<Wait>("Orb Pause", 0).time = 0.75f;
        _com.InsertCustomAction("Orb Antic", () =>
        {
            foreach (var beam in a2beams)
            {
                Destroy(beam);
            }
            a2beams.Clear();
        }, 3);
        _com.RemoveAction("Orb Pos", 7);
        _com.GetAction<Wait>("Orb Summon", 2).time = 0.5f;
        _com.InsertCustomAction("Final Event", () =>
        {
            AudioSource aus;
            aus = a2beams[0].GetAddComponent<AudioSource>();
            aus.PlayOneShot(BeamAnticClip, 0.5f * GameManager.instance.gameSettings.soundVolume / 10f);
            foreach (var beam in a2beams)
            {
                beam.LocateMyFSM("Control").SendEvent("ANTIC");
            }
        }, 0);
        _com.InsertCustomAction("Orb Check", () =>
        {
            AudioSource aus;
            aus = a2beams[0].GetAddComponent<AudioSource>();
            aus.PlayOneShot(BeamFireClip, 0.5f * GameManager.instance.gameSettings.soundVolume / 10f);
            foreach (var beam in a2beams)
            {
                beam.LocateMyFSM("Control").SendEvent("FIRE");
            }
            StartCoroutine(BeamEndAndAntic());
        }, 0);

        _com.InsertCustomAction("Orb End", () =>
        {
            foreach (var beam in a2beams)
            {
                beam.SetActive(false);
            }
        }, 0);

        //横刺乱向

        _com.InsertCustomAction("Comb L 2", () =>
        {
            int r = UnityEngine.Random.Range(6, 9);
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetAction<SetPosition>("Top 2", 0).y = 58f;
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetAction<SetFloatValue>("Top 2", 2).floatValue = 12f;
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetAction<iTweenMoveBy>("Tween", 0).vector = new Vector3(0, 50, 0);

            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetVariable<FsmInt>("Type").Value = r;// > 2 ? r + 3 : r;
        }, 3);

        _com.InsertCustomAction("Comb R 2", () =>
        {
            int r = UnityEngine.Random.Range(6, 9);
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetAction<SetPosition>("Top 2", 0).y = 58f;
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetAction<SetFloatValue>("Top 2", 2).floatValue = 12f;
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetAction<iTweenMoveBy>("Tween", 0).vector = new Vector3(0, 50, 0);
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").GetVariable<FsmInt>("Type").Value = r; //> 2 ? r + 3 : r;
        }, 3);

    }

    private IEnumerator OrbCircle(GameObject gameObject, bool v)
    {
        float a;
        Color color = GameObject.Find("Halo").GetComponent<SpriteRenderer>().color;
        a = color.a;
        color.a = 1f;
        GameObject.Find("Halo").GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.5f);
        int num = 12;
        float degree = 360f / num;
        float distance = v ? 3f : 12f;
        int turn = 0;
        for (int i = 0; i < 9; i++)
        {
            Vector3 pos;
            for (int j = 1 + turn * num; j < num + turn * num; j++)
            {
                pos = gameObject.transform.position + new Vector3(Mathf.Cos(j * degree) * distance, Mathf.Sin(j * degree) * distance, 0);

                useorbs[j].transform.position = pos;

                useorbs[j].LocateMyFSM("Orb Control").SetState("Init");
                useorbs[j].SetActive(true);


            }
            distance += 1f * (v ? 2f : -1);
            yield return new WaitForSeconds(0.3f);
            for (int j = 1 + turn * num; j < num + turn * num; j++)
            {
                useorbs[j].LocateMyFSM("Orb Control").SendEvent("DESTROY");
            }
            turn = turn + 1 > 4 ? 0 : turn + 1;
        }
        useorbs[0].LocateMyFSM("Orb Control").SendEvent("DESTROY");
        Color color1 = GameObject.Find("Halo").GetComponent<SpriteRenderer>().color;
        color1.a = a;
        GameObject.Find("Halo").GetComponent<SpriteRenderer>().color = color1;
        yield break;
    }

    private IEnumerator BeamEndAndAntic()
    {
        yield return new WaitForSeconds(0.05f);
        foreach (var beam in a2beams)
        {
            beam.LocateMyFSM("Control").SendEvent("END");
        }
        yield return null;
        foreach (var beam in a2beams)
        {
            beam.LocateMyFSM("Control").SendEvent("ANTIC");
        }
    }

    private IEnumerator Shown()
    {
        while (knight.transform.position.y < 50f)
            yield return null;
        ShowConvo("黄昏会消去耀阳");
        while (knight.transform.position.y < 75f)
            yield return null;
        ShowConvo("新月会取代满月");
        while (knight.transform.position.y < 100f)
            yield return null;
        ShowConvo("疾风会卷去落木");
        while (knight.transform.position.y < 125f)
            yield return null;
        ShowConvo("泪水会消散时间");
        while (knight.transform.position.y < 140f)
            yield return null;
        ShowConvo("万物皆有其起落");
        yield break;



    }

    private IEnumerator Zoomout()
    {
        float vel = 0.4f;
        GameObject camera = GameCameras.instance.gameObject.FindGameObjectInChildren("CameraParent").FindGameObjectInChildren("tk2dCamera");
        yield return new WaitForSeconds(1.5f);
        if (camera != null)
        {
            zoomscale = camera.GetComponent<tk2dCamera>().ZoomFactor;
            zoom = true;

            while (camera.GetComponent<tk2dCamera>().ZoomFactor > 0.8)
            {
                camera.GetComponent<tk2dCamera>().ZoomFactor -= vel * Time.deltaTime;
                yield return null;
            }
        }
        yield break;
    }
    private IEnumerator Zoomin()
    {
        float vel = 0.4f;
        GameObject camera = GameCameras.instance.gameObject.FindGameObjectInChildren("CameraParent").FindGameObjectInChildren("tk2dCamera");
        if (camera != null)
        {


            while (camera.GetComponent<tk2dCamera>().ZoomFactor < 0.9)
            {
                camera.GetComponent<tk2dCamera>().ZoomFactor += vel * Time.deltaTime;
                yield return null;
            }
        }
        yield break;
    }
    private void ModifyAcend()
    {
        gap = 0.5f;
        height = 70f;
        _com.InsertCustomAction("Comb Top 2", () =>
        {
            GameObject nails = _com.FsmVariables.FindFsmGameObject("Attack Obj").Value;
            if (height < 160f) { height += 4.5f; }
            else { gap = 1.2f; }
            nails.LocateMyFSM("Control").GetAction<SetPosition>("Top 2", 0).y = height;
            nails.LocateMyFSM("Control").GetAction<SetFloatValue>("Top 2", 2).floatValue = 8f;
            nails.LocateMyFSM("Control").GetAction<iTweenMoveBy>("Tween", 0).vector = new Vector3(0, 150, 0);
            nails.LocateMyFSM("Control").InsertCustomAction("Antic", () =>
            {
                List<GameObject> nailist = new List<GameObject>();
                nails.LocateMyFSM("Control").FsmVariables.FindFsmGameObject("Nails").Value.FindAllChildren(nailist);
                foreach (var nail in nailist)
                {

                    if (nail.name.Contains("Radiant Nail"))
                    {
                        nail.transform.SetPositionY(nail.transform.GetPositionY() + UnityEngine.Random.Range(-3f, 3f));
                        nail.transform.SetPositionX(nail.transform.GetPositionX() + UnityEngine.Random.Range(-0.75f, 0.75f));
                    }
                }
                nails.LocateMyFSM("Control").RemoveAction("Antic", 2);
            }, 2);
        }, 3);

        _con.RemoveAction("Plat Setup", 4);
        _con.RemoveAction("Plat Setup", 2);
        _con.InsertCustomAction("Plat Setup", () =>
        {
            GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("APPEAR");
            GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").SendEvent("APPEAR");
            PlayerData.instance.SetHazardRespawn(new Vector3(59f, 46f, 0), true);

            Clock = Instantiate(_com.GetAction<SpawnObjectFromGlobalPool>("Spawn Fireball", 1).gameObject.Value);
            clock = Clock.GetComponent<AudioSource>();
            clock.Stop();
            Clock.transform.SetPosition2D(0f, 0f);
            hourHand = Instantiate(oribeam);
            clock = hourHand.GetComponent<AudioSource>();
            if (clock != null)
                clock.Stop();
            hourHand.transform.SetRotation2D(0);
            hourHand.transform.SetPosition2D(0f, 0f);
            minuteHand = Instantiate(oribeam);
            clock = minuteHand.GetComponent<AudioSource>();
            if (clock != null)
                clock.Stop();
            minuteHand.transform.SetRotation2D(0);
            minuteHand.transform.SetPosition2D(0f, 0f);
            secondHand = Instantiate(oribeam);
            clock = secondHand.GetComponent<AudioSource>();
            if (clock != null)
                clock.Stop();
            clock = secondHand.GetAddComponent<AudioSource>();
            secondHand.transform.SetRotation2D(0);
            secondHand.transform.SetPosition2D(0f, 0f);


            Clock.transform.SetParent(timeClock.transform);
            hourHand.transform.SetParent(timeClock.transform);
            minuteHand.transform.SetParent(timeClock.transform);
            secondHand.transform.SetParent(timeClock.transform);
            hourHand.transform.localScale = new Vector3(5f, 3f, 1f);
            minuteHand.transform.localScale = new Vector3(10f, 2f, 1f);
            secondHand.transform.SetScaleY(1f);
            timeClock.transform.SetPosition2D(63f, 97.5f);
            hourHand.transform.SetRotation2D(hour);
            minuteHand.transform.SetRotation2D(minute);
            secondHand.transform.SetRotation2D(second);
            timeClock.SetActiveChildren(true);
            StartCoroutine(BeamFire(hourHand, 5f, 9999f, 0));
            StartCoroutine(BeamFire(minuteHand, 5f, 9999f, 0));
            StartCoroutine(BeamFire(secondHand, 5f, 9999f, 0));




        }, 2);
        _con.GetAction<Wait>("Plat Setup", 5).time = 0.5f;
        _con.GetAction<SendEventByName>("Ascend Cast", 1).sendEvent = "COMB TOP2";


        _con.InsertCustomAction("Scream", () =>
        {
            _com.GetVariable<FsmGameObject>("Attack Obj").Value.LocateMyFSM("Control").SetState("Reset");
            GameObject.Find("Abyss Pit").LocateMyFSM("Ascend").FsmVariables.FindFsmFloat("Hero Y").Value = knight.transform.position.y;
            GameObject.Find("Abyss Pit").LocateMyFSM("Ascend").SendEvent("ASCEND");
            global::PlayerData.instance.SetHazardRespawn(new Vector3(58, 153, 0), true);
            timeClock.SetActive(false);
            final = true;
        }, 8);



    }

    private void ModifyFinal()
    {

        _com.RemoveTransition("Set Final Orbs", "FINISHED");
        int num = 3;
        float distance = 3f;
        _con.InsertAction("Scream", new Wait() { time = 5f, realTime = false }, 8);

        _con.InsertCustomAction("Scream", () =>
        {

            for (int i = 1; i < num + 1; i++)
            {
                useorbs[i].LocateMyFSM("Orb Control").GetAction<ChaseObjectV2>("Chase Hero", 3).target = gameObject;
                useorbs[i].LocateMyFSM("Orb Control").GetAction<ChaseObjectV2>("Chase Hero 2", 4).target = gameObject;
                useorbs[i].LocateMyFSM("Orb Control").RemoveAction("Chase Hero", 2);
                useorbs[i].LocateMyFSM("Orb Control").RemoveAction("Chase Hero", 0);
                useorbs[i].LocateMyFSM("Orb Control").RemoveAction("Chase Hero 2", 3);
                useorbs[i].LocateMyFSM("Orb Control").RemoveAction("Chase Hero 2", 2);
                useorbs[i].LocateMyFSM("Orb Control").RemoveAction("Chase Hero 2", 0);
                useorbs[i].transform.position = gameObject.transform.position + distance * new Vector3(Mathf.Cos(i * (360f / num)), Mathf.Sin(i * (360f / num)), 0);
                useorbs[i].LocateMyFSM("Orb Control").SetState("Init");
            }
            StartCoroutine(finalshow());
        }, 2);

        _com.InsertCustomAction("Final Hit", () =>
        {
            for (int i = 1; i < num + 1; i++)
            {
                useorbs[i].LocateMyFSM("Orb Control").SendEvent("DESTROY");
                useorbs[i].SetActive(false);
            }
        }, 0);

    }

    IEnumerator finalshow()
    {
        float distance = 5f;
        float delay = 1.5f;
        float v = 80f;
        float now = 0f;
        int num = 1;
        List<GameObject> orbs = new List<GameObject>();
        yield return new WaitForSeconds(0.2f);
        useorbs[1].transform.position = gameObject.transform.position + new Vector3(0, distance, 0);
        useorbs[1].LocateMyFSM("Orb Control").GetAction<AudioPlaySimple>("Init", 0).volume = 0.5f;
        useorbs[1].SetActive(true);
        orbs.Add(useorbs[1]);
        num = 2;
        while (num < 5)
        {
            if (now > delay)
            {
                now = 0f;
                if (num < 4)
                {
                    useorbs[num].transform.position = gameObject.transform.position + new Vector3(0, distance, 0);
                    useorbs[num].LocateMyFSM("Orb Control").GetAction<AudioPlaySimple>("Init", 0).volume = 0.5f;
                    useorbs[num].SetActive(true);
                    orbs.Add(useorbs[num]);
                }
                num++;
            }
            foreach (var orb in orbs)
            {
                orb.transform.RotateAround(gameObject.transform.position, Vector3.back, v * Time.deltaTime);
            }
            now += Time.deltaTime;
            yield return null;
        }
        foreach (var orb in orbs) { orb.LocateMyFSM("Orb Control").SendEvent("FIRE"); }
        yield break;
    }

    int update_count = 0;
    private void Update()
    {
        // update_count++;
        // Log(_cho.ActiveStateName + " " + update_count);
        if (!foundbeam)
        {
            string actstr = _com.ActiveStateName;
            if (actstr == "Idle")
            {
                oribeam = _com.FsmVariables.FindFsmGameObject("Ascend Beam").Value;

                foundbeam = true;
            }
        }

        //Log(_com.GetVariable<FsmVector3>("Hero Pos"));

        // if (_con.ActiveStateName == "Ascend Cast")
        // {
        //     if (gapnow >= gap)
        //     {
        //         _com.SendEvent("COMB TOP2");
        //         gapnow = 0f;
        //     }
        //     gapnow += Time.deltaTime;

        //     if (bbgapnow >= bbgap)
        //     {
        //         second -= 6f;
        //         clock.PlayOneShot(tip, 1.5f * GameManager.instance.gameSettings.soundVolume / 10f);

        //         second %= 360f;
        //         minute %= 360f;
        //         hour %= 360f;
        //         secondHand.transform.SetRotation2D(second);
        //         minuteHand.transform.SetRotation2D(minute);
        //         hourHand.transform.SetRotation2D(hour);
        //         bbgapnow = 0f;
        //     }
        //     bbgapnow += Time.deltaTime;

        // }

        // if (gameObject.transform.position.y > 150f && _hp.hp <= 1500 && !finalSet1 && final)
        // {
        //     finalSet1 = true;
        //     GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").ChangeTransition("Idle", "SLOW VANISH", "Vanish Antic");
        //     GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").ChangeTransition("Idle", "SLOW VANISH", "Vanish Antic");
        //     GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").GetAction<Wait>("Vanish Antic", 1).time = 1f;
        //     GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").GetAction<Wait>("Vanish Antic", 1).time = 1f;

        //     GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").ChangeTransition("Appear 2", "SLOW VANISH", "Vanish Antic");
        //     GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").ChangeTransition("Appear 2", "SLOW VANISH", "Vanish Antic");
        //     if (UnityEngine.Random.Range(0, 2) == 0)
        //     {
        //         GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        //         finalSet2 = true;
        //         PlayerData.instance.SetHazardRespawn(new Vector3(58, 153, 0), true);
        //     }
        //     else
        //     {
        //         GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        //         finalSet2 = false;
        //         PlayerData.instance.SetHazardRespawn(new Vector3(68, 153, 0), false);
        //     }
        // }
        // if (finalSet1)
        // {
        //     if (finalgapnow >= finalgap)
        //     {
        //         finalgapnow = 0f;
        //         if (!finalSet2)
        //         {
        //             finalSet2 = true;
        //             GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        //             GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("APPEAR");
        //             PlayerData.instance.SetHazardRespawn(new Vector3(58, 153, 0), true);


        //         }
        //         else
        //         {
        //             finalSet2 = false;
        //             GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("SLOW VANISH");
        //             GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").SendEvent("APPEAR");
        //             PlayerData.instance.SetHazardRespawn(new Vector3(68, 153, 0), false);
        //         }
        //     }
        //     finalgapnow += Time.deltaTime;
        // }


    }

    private void OnDestroy()
    {
        foundbeam = false;
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HutongGames.PlayMaker.FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        if (playMakerFSM.GetState("Set Convo").Actions.Length == 6)
        {
            playMakerFSM.RemoveAction("Set Convo", 4);
        }
        foreach (var orb in orbList) { if (orb != null) { Destroy(orb); } }
        foreach (var orb in useorbs) { if (orb != null) { Destroy(orb); } }
        foreach (var beam in a2beams) { if (beam != null) { Destroy(beam); } }
        if (zoom) { GameCameras.instance.gameObject.FindGameObjectInChildren("CameraParent").FindGameObjectInChildren("tk2dCamera").GetComponent<tk2dCamera>().ZoomFactor = zoomscale; }
    }


    private void ShowConvo(string msg)
    {
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HutongGames.PlayMaker.FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        if (playMakerFSM.GetState("Set Convo").Actions.Length == 5)
        {
            playMakerFSM.InsertCustomAction("Set Convo", () =>
            {
                playMakerFSM.FsmVariables.GetFsmString("Convo Text").Value = convoTitle;
            }, 4);
        }
        playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "Radiance";
        playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;
        convoTitle = msg;
        playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
    }

    private void Log(object obj)
    {
        if (obj == null)
        {
            Modding.Logger.Log("[Uradiance]:" + null);
        }
        else Modding.Logger.Log("[Uradiance]:" + obj);
    }

    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        slash_go = Instantiate(preloadedObjects[PreloadManager.hk_scene][PreloadManager.hk_slash_effect_name]);
        GameObject.DontDestroyOnLoad(slash_go);
        slash_go.SetActive(false);
    }
}
