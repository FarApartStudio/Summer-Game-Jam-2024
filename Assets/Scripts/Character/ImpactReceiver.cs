using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    [SerializeField] float mass = 3.0F;
    [SerializeField] float speed = 5.0F;
    [SerializeField] Vector3 impact = Vector3.zero;
    private CharacterController character;
    private bool isReceivingImpact;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (impact.magnitude > 0.2F) character.Move(impact * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, speed * Time.deltaTime);
    }

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force / mass;
    }
}