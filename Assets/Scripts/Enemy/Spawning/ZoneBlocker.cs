using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBlocker : MonoBehaviour
{
    [SerializeField] ParticleSystem vfxEffect;

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (BaseHero.PlayerHero && other.gameObject == BaseHero.PlayerHero.gameObject)
        //{
        //    Vector3 offset = new Vector3(0, 1, 0);
        //    vfxEffect.transform.position = GetComponent<BoxCollider>().ClosestPoint(other.transform.position + offset);
        //    vfxEffect.Play();

        //    MessagePopUp.Instance.ShowMessage("Kill all enemies");
        //}
    }

    public void EnableWall()
    {
        gameObject.SetActive(true);
    }

    public void DisableWall()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
