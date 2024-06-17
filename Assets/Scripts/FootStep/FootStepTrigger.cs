using Pelumi.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FootStepTrigger : MonoBehaviour
{
    [SerializeField]private LayerMask layerMask;
    [SerializeField] private float Yoffset;

    private TerrainDetector terrainDetector;

    private void Awake()
    {
        terrainDetector = new TerrainDetector();
    }

    public void FootL(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            PlayFootStepSound(animationEvent);
        } 
    }

    public void FootR(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            PlayFootStepSound(animationEvent);
        }
    }

    public void PlayFootStepSound(AnimationEvent animationEvent)
    {
        FootStepSurface footStepSurface = GetFootStepSurface(out Vector3 position);

        AudioTypeID footStep = AudioTypeID.DirtFootstep;

        if (footStepSurface != null)
        {
            FootSurface footSurface = footStepSurface.GetFootSurface;
            switch (footSurface)
            {
                case FootSurface.Dirt:
                    footStep = AudioTypeID.DirtFootstep;
                    break;
                case FootSurface.Grass:
                    footStep = AudioTypeID.GrassFootstep;
                    break;
                case FootSurface.Wood:
                    footStep = AudioTypeID.WoodFootstep;
                    break;
                case FootSurface.Rock:
                    footStep = AudioTypeID.RockFootstep;
                    break;
            }
        }
        else
        {
            int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(position);

            switch (terrainTextureIndex)
            {
                case 0:
                    footStep = AudioTypeID.GrassFootstep;
                    break;
                case 1:
                    footStep = AudioTypeID.DirtFootstep;
                    break;
                case 2:
                    footStep = AudioTypeID.WoodFootstep;
                    break;
                case 3:
                    footStep = AudioTypeID.RockFootstep;
                    break;
            }
        }

        AudioSystem.PlayOneShotAudio(footStep, AudioCategory.Sfx, true, volume: Random.Range(0.8f, 1.0f));
    }

    public FootStepSurface GetFootStepSurface(out Vector3 pos)
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.down * Yoffset;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 5.0f, layerMask))
        {
            FootStepSurface footStepSurface = hit.collider.GetComponent<FootStepSurface>();
            if (footStepSurface != null)
            {
                pos = hit.point;
                return footStepSurface;
            }
        }
        pos = transform.position;
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * Yoffset, 0.1f);
    }
}
