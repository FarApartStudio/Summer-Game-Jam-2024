using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] AudioClip eliminationClip;

    [Header("Body Part")]
    [SerializeField] GameObject headMesh;
    [SerializeField] GameObject bloodVfx;
    [SerializeField] ParticleSystem headExplodeVfx;

    [Header("HitBox")]
    [SerializeField] Color headHitColor;
    [SerializeField] HitPoint headHitBox;
    [SerializeField] Color bodyHitColor;
    [SerializeField] HitPoint bodyHitBox;

    AudioSource audioSource;
    Animator anim;
    HealthController healthController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthController = GetComponent<HealthController>();
    }
    void Start()
    {
        healthController.OnHit += OnHit;

        headHitBox.OnHit += HeadHit;
        bodyHitBox.OnHit += BodyHit;

        headHitBox.AssignHealthSystem(healthController);
        bodyHitBox.AssignHealthSystem(healthController);
    }

    private void OnEnable()
    {
        SetHealth(health);

        headMesh.SetActive(true);
        anim.enabled = true;
        SetRigidBodyState(true);
        SetColliderState(true);
        bloodVfx.SetActive(false);

        headHitBox.GetComponent<Collider>().enabled = true;
        bodyHitBox.GetComponent<Collider>().enabled = true;
    }

    public void SetHealth(int amount)
    {
        if (healthController != null) healthController.SetUp(amount);
    }

    private void OnHit(DamageInfo damageInfo)
    {

    }

    public void HeadHit(DamageInfo damageInfo, Vector3 damagePosition)
    {
        healthController.DealDamage(damageInfo);

        //PopUpTextManager.Instance.PopUpTextUI(damagePosition, "Head Shot", headHitColor);
        // PopUpTextManager.Instance.PopUpTextUI(damagePosition, healthSystem.amountSent.ToString(), headHitColor);

        if (!healthController.IsAlive)
        {
            StartCoroutine(DeathSequence(true));
        }
    }

    public void BodyHit(DamageInfo damageInfo, Vector3 damagePosition)
    {
        healthController.DealDamage(damageInfo);

        //PopUpTextManager.Instance.PopUpTextUI(damagePosition, "Body Shot", bodyHitColor);
        // PopUpTextManager.Instance.PopUpTextUI(damagePosition, healthSystem.amountSent.ToString(), bodyHitColor);

        if (!healthController.IsAlive)
        {
            StartCoroutine(DeathSequence(false));
        }
    }


    IEnumerator DeathSequence(bool headShot)
    {
        if (headShot)
        {
            headExplodeVfx.Play();
            headMesh.SetActive(false);
            bloodVfx.SetActive(true);
        }

        anim.enabled = false;
        headHitBox.GetComponent<Collider>().enabled = false;
        bodyHitBox.GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(0.1f);

        audioSource.PlayOneShot(eliminationClip);

        SetRigidBodyState(false);
        SetColliderState(false);
    }

    void SetRigidBodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = state;
        }
    }

    void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.isTrigger = state;
        }
    }

    public void PrepareForSlowMo()
    {
        anim.enabled = false;
    }

    public void DeactivateObject()
    {

    }
}
