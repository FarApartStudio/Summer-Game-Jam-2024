using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceUIController : MonoBehaviour
{
    private List<IScreenSpaceUI> screenSpaceUIs = new List<IScreenSpaceUI>();
    private Camera cam;
    private RectTransform canvas;

    private void Awake()
    {
        cam = Camera.main;
        canvas = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        // Create a temporary list to store distances and indices
        List<(float distance, int index)> distances = new List<(float distance, int index)>();

        for (int i = 0; i < screenSpaceUIs.Count; i++)
        {
            IScreenSpaceUI screenHealth = screenSpaceUIs[i];
            Vector3 screenPosition = cam.WorldToScreenPoint(screenHealth.Target.position + Vector3.up * screenHealth.YOffset);
            Vector3 directionToEnemy = screenHealth.Target.position - cam.transform.position;

            float targetDistanceFromCamera = Vector3.Distance(cam.transform.position, screenHealth.Target.position);
            screenHealth.GetDistanceFromCamera?.Invoke(targetDistanceFromCamera);

            float dotProduct = Vector3.Dot(cam.transform.forward, directionToEnemy);

            if (dotProduct > 0 && screenSpaceUIs[i].IsActive)
            {
                screenSpaceUIs[i].Self.gameObject.SetActive(true);

                screenSpaceUIs[i].Self.position = screenPosition;
                // screenSpaceUIs[i].Self.anchoredPosition = GetUIPos(screenHealth.Target.position + Vector3.up * screenHealth.YOffset);
            }
            else
            {
                screenSpaceUIs[i].Self.gameObject.SetActive(false);
            }

            float distance = Vector3.Distance(cam.transform.position, screenSpaceUIs[i].Target.position);
            distances.Add((distance, i));
        }

        // Sort the list of distances and indices
        distances.Sort((a, b) => b.distance.CompareTo(a.distance)); // Use CompareTo to sort in descending order

        // Rearrange the screenHealthsList based on the sorted indices
        for (int i = 0; i < distances.Count; i++)
        {
            int index = distances[i].index;
            screenSpaceUIs[index].Self.SetSiblingIndex(i);
        }
    }

    private Vector2 GetUIPos(Vector3 worldPos)
    {
        Vector2 viewPort = cam.WorldToViewportPoint(worldPos);
        Vector2 uiPos = new Vector2(
                       ((viewPort.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
                                  ((viewPort.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));
        return uiPos;
    }

    public void Add(IScreenSpaceUI screenSpaceUI)
    {
        screenSpaceUIs.Add(screenSpaceUI);
    }

    public void Remove(IScreenSpaceUI screenSpaceUI)
    {
        screenSpaceUIs.Remove(screenSpaceUI);
    }
}
