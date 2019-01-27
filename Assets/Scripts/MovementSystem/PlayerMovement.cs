using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float stopVelocity = 0.001f;
    public float stopCheckTime = 0.2f;
    public float maxPull = 1.5f;
    public float minPull = 0.8f;
    public float throwForce = 10;
    public bool disableMovement;
    public float flyingTime = 0;

    public AudioSource shootAudio;
    public AudioSource windUpAudio;
    public AudioSource landingAudio;
    public AudioSource flyingAudio;

    LineRenderer line;
    Rigidbody2D rigid;
    bool isStopped = true;
    Camera cam;
    GameObject platform;
    Vector3 platformDistance;
    Vector3 pivot;
    Animator anim;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        line = GetComponent<LineRenderer>();
        cam = Camera.main;
    }

    void Update()
    {
        if (disableMovement)
        {
            stop();
        }
        else
        {
            if (isStopped)
            {
                Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
                mouse.z = 0;
                if (Input.GetMouseButtonDown(0))
                {
                    if (windUpAudio)
                    {
                        windUpAudio.Play();
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    anim.SetBool("Shoot", true);
                    if (Vector3.Distance(mouse, transform.position) > maxPull)
                    {
                        Vector3 maxPosition = (mouse - transform.position).normalized * maxPull + transform.position;
                        pivot = maxPosition;
                    }
                    else
                    {
                        pivot = mouse;
                    }
                    line.enabled = true;
                    Vector3[] positions = { pivot - Vector3.forward, 2 * transform.position - pivot - Vector3.forward };
                    line.SetPositions(positions);
                    return;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    line.enabled = false;
                    float distance = Vector3.Distance(pivot, transform.position);
                    if (distance > minPull)
                    {
                        anim.SetTrigger("Flying");
                        rigid.isKinematic = false;
                        isStopped = false;
                        Vector3 velocity = pivot - transform.position;
                        rigid.velocity = new Vector2(velocity.x, velocity.y) * -throwForce * distance;
                        if(shootAudio)
                        {
                            shootAudio.Play();
                        }
                        if(flyingAudio)
                        {
                            flyingAudio.Play();
                        }
                    }
                    else
                    {
                        anim.SetBool("Shoot", false);
                    }
                    flyingTime = 0;
                }
                if (platform)
                {
                    transform.position = platform.transform.position + platformDistance;
                }
            }
            else
            {
                Vector2 v = rigid.velocity;
                float angle = Mathf.Atan2(-v.y, -v.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                if (rigid.velocity.sqrMagnitude < stopVelocity)
                {
                    StartCoroutine(stopCheck());
                    platform = null;
                    stop();
                }
                flyingTime += Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isSticky sticky = col.gameObject.GetComponent<isSticky>();
        Animator animator = col.gameObject.GetComponent<Animator>();
        if (animator)
        {
            animator.SetBool("IsMounted", true);
        }
        AudioSource audioSource = col.gameObject.GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.Play();
        }
        if (sticky)
        {
            stop();
            if (sticky.isPlatform)
            {
                platform = col.gameObject;
                platformDistance = transform.position - col.gameObject.transform.position;
            }
            Vector2 contact = col.GetContact(0).normal;
            float angle = Mathf.Atan2(-contact.y, -contact.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        }
    }

    IEnumerator stopCheck()
    {
        yield return new WaitForSeconds(stopCheckTime);
        if (rigid.velocity.sqrMagnitude < stopVelocity)
        {
            stop();
        }
    }

    void stop()
    {
        if(!isStopped)
        {
            anim.SetTrigger("Landing");
            anim.SetBool("Shoot", false);
            if (landingAudio)
            {
                landingAudio.Play();
            }
            if (flyingAudio)
            {
                flyingAudio.Stop();
            }
        }
        transform.rotation = Quaternion.identity;
        isStopped = true;
        rigid.isKinematic = true;
        rigid.angularVelocity = 0;
        rigid.velocity = new Vector2(0, 0);
    }
}
