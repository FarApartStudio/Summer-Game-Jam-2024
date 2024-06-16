using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.ObjectPool;

public class ScreenSpaceHealthController : MonoBehaviour
{
    [SerializeField] private HealthUI screenHealthPrefab;
    [SerializeField] private ScreenSpaceUIController screenSpaceUIController;
    private Dictionary<HealthController, HealthUI> screenHealths = new Dictionary<HealthController, HealthUI>();

    private void Awake()
    {
      //  HealthController.OnSetUp += HandleHealthControllerCreated;
       // HealthController.OnDespawn += HandleHealthControllerDestroyed;
    }

    private void OnDestroy()
    {
      //  HealthController.OnSetUp -= HandleHealthControllerCreated;
      //  HealthController.OnDespawn -= HandleHealthControllerDestroyed;
    }

    private void HandleHealthControllerCreated(HealthController controller)
    {
        HealthUI screenHealth = ObjectPoolManager.SpawnObject(screenHealthPrefab, transform);
        screenHealth.Spawn(controller.transform);
        controller.OnHealthChanged += screenHealth.ChangeValue;
        screenHealths.Add(controller, screenHealth);
        screenSpaceUIController.Add(screenHealth);
    }

    private void HandleHealthControllerDestroyed(HealthController controller)
    {
        HealthUI healthUI = screenHealths[controller];

        controller.OnHealthChanged -= screenHealths[controller].ChangeValue;
        screenSpaceUIController.Remove(screenHealths[controller]);
        screenHealths.Remove(controller);
        ObjectPoolManager.ReleaseObject(healthUI);
    }
}
