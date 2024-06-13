using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JuicerTest : MonoBehaviour
{
    public enum TestMode
    {
        Position,
        Rotation,
        Scale,
    }

    [Header("Shake")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 dir;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakePower = 0.5f;
    [SerializeField] private int vibrateCount = 10;
    [SerializeField] private bool fadeOut;
    [SerializeField] private ShakeRandomnessMode randomnessMode;
    [SerializeField] private TestMode testMode;

    [Header("Juicer")]
    [SerializeField] private Transform juicerTarget;
    [SerializeField] private JuicerRuntime juicerRuntime;

    [Header("Text Test")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Scene")]
    [SerializeField] private string sceneName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
          //  TestRaw();
        }
        TestJuicer();
    }

    public void TestRaw()
    {
        juicerRuntime = Juicer.To<float>(() => juicerTarget.transform.localScale.x,
                (value) => juicerTarget.transform.localScale = new Vector3(value, juicerTarget.transform.localScale.y, juicerTarget.transform.localScale.z),
               juicerTarget.transform.localScale.x * 2, 0.2f)
                   .SetTimeMode(TimeMode.Unscaled)
                   .SetLoop(0, LoopType.Yoyo)
                   .Start();
    }

    public void TestJuicer()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("TestJuicer");
            juicerRuntime = juicerTarget.transform.JuicyScale(Vector3.one * 2, 0.2f)
                   .SetTimeMode(TimeMode.Unscaled)
                   .SetLoop(1, LoopType.Yoyo)
                   .Start();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneName);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(juicerRuntime.Target);
        }
    }

    public void TestShake()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (testMode)
            {
                case TestMode.Position:
                    target.JuicyShakePosition(shakeDuration, dir, vibrateCount, fadeOut: fadeOut, randomnessMode: randomnessMode);
                    break;
                case TestMode.Rotation:
                    target.JuicyShakeRotation(shakeDuration, dir, vibrateCount, fadeOut: fadeOut, randomnessMode: randomnessMode);
                    break;
                case TestMode.Scale:
                    target.JuicyShakeScale(shakeDuration, dir, vibrateCount, fadeOut: fadeOut, randomnessMode: randomnessMode);
                    break;
            }
        }
    }

    public void FullUsage()
    {
        JuicerRuntime juicerRuntime = transform.JuicyScale(Vector3.one, 0.2f);

        juicerRuntime.SetTimeMode(TimeMode.Unscaled);

        juicerRuntime.SetLoop(2, LoopType.Incremental);

        juicerRuntime.AddTimeEvent(0.5f, () =>
        {
            Debug.Log("Time Event");
        });

        juicerRuntime.SetOnStart(() =>
        {
            Debug.Log("On Start");
        });

        juicerRuntime.SetOnTick(() =>
        {
            Debug.Log("On Tick");
        });

        juicerRuntime.SetOnStepComplete(() =>
        {
            Debug.Log("On StepComplete");
        });

        juicerRuntime.SetOnCompleted(() =>
        {
            Debug.Log("On Complected");
        });

        juicerRuntime.Start();

        juicerRuntime.Pause();

        juicerRuntime.Stop();
    }
}