using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointUI : MonoBehaviour, IWaypoint
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI meterText;
    [SerializeField] private IWaypointInsigator waypointInsigator;

    public Image Img => img;

    public TextMeshProUGUI MeterText => meterText;
    public IWaypointInsigator WaypointInsigator => waypointInsigator;

    public void SetUp(IWaypointInsigator waypointInsigator)
    {
        this.waypointInsigator = waypointInsigator;
        img.sprite = waypointInsigator.Icon;
    }
}
