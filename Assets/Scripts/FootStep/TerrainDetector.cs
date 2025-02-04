using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetector : MonoBehaviour
{
    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private float[,,] splatmapData;
    private int numTextures;
    private Terrain terrain;

    private void Start()
    {
        terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("No active terrain found");
            return;
        }

        terrainData = terrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;

        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
    }

    private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        Vector3 splatPosition = new Vector3();
        Vector3 terPosition = terrain.transform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight;
        return splatPosition;
    }

    public int GetActiveTerrainTextureIdx(Vector3 position)
    {
        if (terrain == null)
        {
           // Debug.LogError("No active terrain found");
            return -1;
        }

        Vector3 terrainCord = ConvertToSplatMapCoordinate(position);
        int activeTerrainIndex = 0;
        float largestOpacity = 0f;

        for (int i = 0; i < numTextures; i++)
        {
            if (largestOpacity < splatmapData[(int)terrainCord.z, (int)terrainCord.x, i])
            {
                activeTerrainIndex = i;
                largestOpacity = splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
            }
        }

        return activeTerrainIndex;
    }
}