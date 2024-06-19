using Pelumi.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class FootStepTrigger : MonoBehaviour
{
    [SerializeField]private LayerMask layerMask;
    [SerializeField] private float Yoffset;
    [SerializeField] private float checkDistance = 15;


    public void FootL(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            DetectGround();
        } 
    }

    public void FootR(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            DetectGround();
        }
    }

    public void Land (AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSystem.PlayOneShotAudio(AudioTypeID.DirtFootstep, AudioCategory.Sfx, true, volume: Random.Range(0.8f, 1.0f));
        }
    }

    public void DetectGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.down * Yoffset;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, checkDistance, layerMask))
        {
            FootStepSurface footStepSurface = hit.collider.GetComponent<FootStepSurface>();
            if (footStepSurface != null)
            {
                PlayFootStepSound (footStepSurface);
                return;
            }

            TerrainDetector terrainDetector = hit.collider.GetComponent<TerrainDetector>();
            if (terrainDetector != null)
            {
                PlayFootStepSound(terrainDetector);
                return;
            }
        }

        AudioSystem.PlayOneShotAudio(AudioTypeID.DirtFootstep, AudioCategory.Sfx, true, volume: Random.Range(0.8f, 1.0f));
    }

    public void PlayFootStepSound(TerrainDetector terrainDetector)
    {
        AudioTypeID footStep = AudioTypeID.DirtFootstep;

        int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(transform.position);

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

        AudioSystem.PlayOneShotAudio(footStep, AudioCategory.Sfx, true, volume: Random.Range(0.8f, 1.0f));
    }

    public void PlayFootStepSound(FootStepSurface footStepSurface)
    {
        AudioTypeID footStep = AudioTypeID.DirtFootstep;

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
            case FootSurface.Water:
            footStep = AudioTypeID.WaterFootstep;
            break;
        }

        AudioSystem.PlayOneShotAudio(footStep, AudioCategory.Sfx, true, volume: Random.Range(0.8f, 1.0f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * Yoffset, 0.1f);
    }
}
