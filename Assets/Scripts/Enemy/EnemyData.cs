using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class MaterialSet
{
    public Material[] materials;
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "My Game/Enemy")]
[InlineEditor]
public class EnemyData : ScriptableObject
{
    public enum EnemyType {Melee, Ranged, Boss}

    [BoxGroup("Basic Info")]
    [LabelWidth(100)]
    public string enemyName;
    [BoxGroup("Basic Info")]
    [TextArea]
    public string description;

    [BoxGroup("Basic Info")]
    [PreviewField(75)]
    [HideLabel]
    public Sprite enemyIcon;

    [HorizontalGroup("Game Data", 75)]
    [PreviewField(75)]
    [HideLabel]
    public EnemyController enemyController;

    [VerticalGroup("Game Data/ Stats")]
    [LabelWidth(100)]
    public float damage;

    [VerticalGroup("Game Data/ Stats")]
    [LabelWidth(100)]
    public float health;

    [VerticalGroup("Game Data/ Stats")]
    [LabelWidth(100)]
    public EnemyType enemyType;

    [Header("Skin")]
    public SkinSet skinSets;

    [Header("Checks")]
    public bool hasRage;
    public bool canKnockBack;

    [System.Serializable]
    public class SkinSet
    {
        public MaterialSet[] materials;

        public void SelectSkin(int materialIndex, SkinnedMeshRenderer[] parts)
        {
            if (materials.Length == 0) return;

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].material = materials[materialIndex].materials[i];
            }
        }

        public void SelectRandomSkin(SkinnedMeshRenderer[] parts)
        {
            if (materials.Length == 0) return;

            int randomIndex = Random.Range(0, materials.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].material = materials[randomIndex].materials[i];
            }
        }
    }
}