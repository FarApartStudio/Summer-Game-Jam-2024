using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenShotTaker : MonoBehaviour
{
    private Action<Texture2D> onPipelineScreenshot;

    public void TakeScreenshot(Action<Texture2D> action, bool hideUI)
    {
        onPipelineScreenshot = action;
        if (hideUI)
        {
            StartCoroutine(TakeScreenShotWithoutUI(action));
        }
        else
        {
            onPipelineScreenshot?.Invoke(ScreenCapture.CaptureScreenshotAsTexture());
        }
    }

    IEnumerator TakeScreenShotWithoutUI(Action<Texture2D> action)
    {
        onPipelineScreenshot = action;
        yield return new WaitForSecondsRealtime(.5f);
        onPipelineScreenshot?.Invoke(ScreenCapture.CaptureScreenshotAsTexture());
    }

    public IEnumerator AdvancedScreenShot(string fileName)
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenShotTexture.ReadPixels(rect, 0, 0);
        screenShotTexture.Apply();

        byte[] byteArray = screenShotTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + fileName + ".png", byteArray);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            TakeScreenshot((texture) =>
            {
                // save the texture to disk
                string path = Application.persistentDataPath + "/Screenshot.png";
                System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            }, true);
        }
    }
}
