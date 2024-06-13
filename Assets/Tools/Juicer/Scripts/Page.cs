using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Page : MonoBehaviour
{
    [SerializeField] private int pageNumber;
    [SerializeField] GameObject frontFace;
    [SerializeField] GameObject backFace;
    [SerializeField] TextMeshProUGUI frontNumberDisplay;
    [SerializeField] TextMeshProUGUI backNumberDisplay;
    bool isFlipped = false;
    float absoluteAngle = 0f;

    private void Start()
    {
        frontNumberDisplay.text = (pageNumber + 1).ToString();
        backNumberDisplay.text = (pageNumber + 2).ToString();
    }

    public void UpdateView()
    {
        absoluteAngle = Mathf.Abs(transform.rotation.eulerAngles.y);
        isFlipped = absoluteAngle > 90f && absoluteAngle < 270f;
        frontFace.SetActive(!isFlipped);
        backFace.SetActive(isFlipped);
    }
}
