using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProject
{
    public enum BodyPartType
    {
        Body,
        Accessory,
        Eyes,
        Gloves,
        Head,
        MouthAndNose,
        Tail
    }

    [System.Serializable]
    public class BodyPart
    {
        [SerializeField] BodyPartType type;
        [SerializeField] GameObject[] visuals;

        public BodyPartType GetBodyPartType => type;
        public GameObject[] GetVisuals => visuals;
    }

    public class CharacterSelection : MonoBehaviour
    {
        [SerializeField] BodyPart[] bodyParts;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateRandomBody();
            }
        }

        public void SetBodyPart(BodyPartType type, int index)
        {
            foreach (BodyPart bodyPart in bodyParts)
            {
                if (bodyPart.GetBodyPartType == type)
                {
                    foreach (GameObject visual in bodyPart.GetVisuals)
                    {
                        visual.SetActive(false);
                    }

                    bodyPart.GetVisuals[index].SetActive(true);
                    return;
                }
            }
        }

        public void GenerateRandomBody()
        {
            foreach (BodyPart bodyPart in bodyParts)
            {
                int randomIndex = Random.Range(0, bodyPart.GetVisuals.Length);
                SetBodyPart(bodyPart.GetBodyPartType, randomIndex);
            }
        }
    }
}
