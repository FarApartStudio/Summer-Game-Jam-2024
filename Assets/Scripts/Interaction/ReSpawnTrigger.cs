using Pelumi.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReSpawnTrigger : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] private UnityEvent OnUse;
    private Transform[] spawnPoints;

    private void Awake()
    {
        // ingnore the parenr object and get all the children
        spawnPoints = GetComponentsInChildren<Transform>();

        // ignore the first element
        spawnPoints = spawnPoints[1..];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pilot pilot))
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = damage;
            pilot.GetHealthController.DealDamage(damageInfo);
            if (!pilot.GetHealthController.IsAlive)
                return;

            pilot.GetHealthController.SetInvisibility(true);

            UIManager.OpenMenu<ScreenFadeMenu>().Show(.5f, .25f, ()=>
            {
 
            },
                
            OnFadeMid: () =>
            {
                Transform spawnPoint = GetRandomSpawnPoint();
                pilot.gameObject.SetActive(false);
                pilot.transform.position = spawnPoint.position;
                pilot.transform.rotation = spawnPoint.rotation;
                pilot.gameObject.SetActive(true);
            },
            ()=>
            {
                IEnumerator SetInvisibility()
                {
                    yield return new WaitForSeconds(1);
                    pilot.GetHealthController.SetInvisibility(false);
                }

                StartCoroutine(SetInvisibility());
            });

            OnUse?.Invoke();
        }
    }



    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
