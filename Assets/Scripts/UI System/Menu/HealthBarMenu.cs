using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarMenu : GenericMenu<GameMenu>
{
    [SerializeField] private HealthUI screenHealthPrefab;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private ScreenSpaceUIController screenSpaceUIController;
    private Dictionary<HealthController, HealthUI> screenHealths = new Dictionary<HealthController, HealthUI>();

    protected override void OnCreated()
    {

    }

    protected override void OnOpened()
    {

    }

    protected override void OnClosed()
    {

    }

    protected override void OnDestoryInvoked()
    {

    }

    public override void ResetMenu()
    {

    }

    public void HandleHealthControllerCreated(HealthController controller)
    {
        HealthUI screenHealth = ObjectPoolManager.SpawnObject(screenHealthPrefab, spawnPos);
        screenHealth.Spawn(controller.transform);
        controller.OnHealthChanged += screenHealth.ChangeValue;
        screenHealths.Add(controller, screenHealth);
        screenSpaceUIController.Add(screenHealth);
    }

    public void HandleHealthControllerDestroyed(HealthController controller)
    {
        HealthUI healthUI = screenHealths[controller];

        controller.OnHealthChanged -= screenHealths[controller].ChangeValue;
        screenSpaceUIController.Remove(screenHealths[controller]);
        screenHealths.Remove(controller);
        ObjectPoolManager.ReleaseObject(healthUI);
    }

}