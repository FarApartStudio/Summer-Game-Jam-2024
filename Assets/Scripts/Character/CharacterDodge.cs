using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDodge : MonoBehaviour
{
    public Func<bool> CanDodge;

    public Action OnDodgeStart;
    public Action OnDodgeStop;

    [SerializeField] private float dodgeCooldown;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private AnimationCurve dodgeCurve;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CharacterManager characterManager;

    [Header("Debug")]
    [SerializeField] private bool isDodging;
    [SerializeField]  private float dodgeDuration;

    private void Start()
    {
        dodgeDuration = dodgeCurve.keys[dodgeCurve.length - 1].time;
    }

    private void Update()
    {
        HandleDodge();
    }

    private void HandleDodge()
    {
        if (CanDodge.Invoke() && !isDodging && InputManager.Instance.GetDodgeInput())
        {
            isDodging = true;
            StartCoroutine(Dodge());
        }
    }

    IEnumerator Dodge()
    {
        isDodging = true;
        float timer = 0;
        characterManager.GetCharacterAnimatorController.PlayTargetActionAnimation("Dodge", true);
        OnDodgeStart?.Invoke();
        while (timer < dodgeDuration)
        {
            float speed = dodgeCurve.Evaluate(timer);
            Vector3 direction = (characterManager.GetCharacterController.transform.forward * speed) + (Vector3.up * gravityMultiplier);
            characterController.Move(direction * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        OnDodgeStop?.Invoke();
        yield return new WaitForSeconds(dodgeCooldown);  

        isDodging = false;
    }
}
