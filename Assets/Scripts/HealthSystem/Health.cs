using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 1;
    public float fallResistance = 0.009f;
    public float health = 1;
    public float defaultFallMultiplier = 0;
    public float invincibilityTime = 1.0f;
    public GameObject spawnPoint;
    public AudioSource respawnSound;

    Rigidbody2D rigid;
    bool dead;
    PlayerMovement movement;
    Animator anim;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {

        if (health <= 0 && !dead)
        {
            StartCoroutine(death());
            dead = true;
        }
        ChangeImageOnHealth changeImage = ChangeImageOnHealth.instance;
        if(changeImage)
        {
            changeImage.setHealth((health / maxHealth) * 100);
        }
    }

    IEnumerator death()
    {
        movement.disableMovement = true;
        anim.SetBool("Dead", true);
        yield return new WaitForSeconds(1f);
        dead = false;
        transform.position = spawnPoint.transform.position;
        health = maxHealth;
        yield return new WaitForSeconds(0.5f);
        movement.disableMovement = false;
        anim.SetTrigger("Respawn");
        anim.SetBool("Dead", false);
        if (respawnSound)
        {
            respawnSound.Play();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (movement.flyingTime < invincibilityTime) return;
        FallModifier modifier = col.gameObject.GetComponent<FallModifier>();
        FlatDamage damager = col.gameObject.GetComponent<FlatDamage>();
        float multiplier = defaultFallMultiplier;
        if (damager)
        {
            health -= damager.value;
        }
        if (modifier)
        {
            multiplier = modifier.damageMultiplier;
        }
        fallDamage(multiplier);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        IsCheckpoint checkpoint = col.GetComponent<IsCheckpoint>();
        if(checkpoint)
        {
            checkpoint.updateCheckpoint();
            AudioSource audioSource = col.gameObject.GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.Play();
            }
            spawnPoint.transform.position = col.transform.position;
            health = maxHealth;
        }
    }

    void fallDamage(float damageMultiplier)
    {
        float speed = rigid.velocity.sqrMagnitude;
        float speedGracefull = speed - fallResistance;
        float damage = speedGracefull * damageMultiplier;
        health -= Mathf.Max(damage, 0);
    }
}
