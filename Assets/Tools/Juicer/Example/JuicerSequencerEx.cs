using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JuicerSequencerEx : MonoBehaviour
{
    [SerializeField] private GameObject testPrefab;
    [SerializeField] private Transform[] path;
    [SerializeField] private float duration;

    private void Start()
    {
        DoSequencer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoSequencer();
        }
    }

    public void DoSequencer()
    {
        Transform pathMover = Instantiate(testPrefab, path[0].position, Quaternion.identity).transform;

        JuicerSequencer jucierSequencer = Juicer.GetJuicerSequencer();

        //foreach (Transform item in path)
        //{
        //    var distance = Vector3.Distance(pointA.position, pointB.position);

        //    var moveTime = distance / _speed;
        //    jucierSequencer.Append(pathMover.JuicyMove(item.position, duration));
        //    jucierSequencer.Append( pathMover.GetComponent<MeshRenderer>().material.JuicyColour(Random.ColorHSV(), duration));
        //    jucierSequencer.Delay(1f);
        //}

        for (int i = 1; i < path.Length; i++)
        {
            var pointA = path[i - 1];
            var pointB = path[i];

            var distance = Vector3.Distance(pointA.position, pointB.position);

            var moveTime = distance / duration;
            jucierSequencer.Append(pathMover.JuicyMove(pointB.position, duration));
            jucierSequencer.Append(pathMover.GetComponent<MeshRenderer>().material.JuicyColour(Random.ColorHSV(), .2f));
            jucierSequencer.Delay(1f);
        }

        jucierSequencer.AppendCallback(() =>
        {
            Destroy(pathMover.gameObject);
        });

        jucierSequencer.Run();

        jucierSequencer.SetOnComplected(() =>
        {
            DoSequencer();
            Debug.Log("Complected");
        });
    }
}
