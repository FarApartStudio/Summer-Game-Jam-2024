 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("MoveSway")]
    [SerializeField] float amount;
    [SerializeField] float maxAmount;
    [SerializeField] float smoothAmount;

    [Header("Rotation")]
    [SerializeField] float rotataionAmount;
    [SerializeField] float maxRotationAmount;
    [SerializeField] float smoothRotation;

    [Space]
    [SerializeField] bool rotationX  = true;
    [SerializeField] bool rotationY = true;
    [SerializeField] bool rotationZ = true;

    Vector3 initialPosition;
    Quaternion initialRotation; 

    float InputX;
    float InputY;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        MoveSway();
        TiltSway();
    }

    public void SetInput (Vector2 input)
    {
        InputX = -input.x;
        InputY = -input.y;
    }

    void MoveSway()
    {
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, smoothAmount * Time.deltaTime);
    }

    void TiltSway()
    {
        float tiltX = Mathf.Clamp(InputX * rotataionAmount, -maxRotationAmount, maxRotationAmount);
        float tiltY = Mathf.Clamp(InputY * rotataionAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler (new Vector3(
            rotationX ? -tiltX : 0f,
            rotationY ? tiltY : 0f,
            rotationZ ? tiltY : 0f ));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, smoothRotation * Time.deltaTime);
    }

    
}
