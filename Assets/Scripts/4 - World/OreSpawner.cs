using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OreSpawner : MonoBehaviour
{

    [System.Serializable]

    public class OreWithRarity
    {
        public GameObject orePrefab;
        public int rarity;
        public Material oreMaterial;
    }


    public List<OreWithRarity> ores;
    public int[] oreCounts;
    public Tilemap tilemap;
    private TileHover tileHover;

    public int gridMin = -500;
    public int gridMax = 500;


    void Start()
    {
        tileHover = FindObjectOfType<TileHover>();
        SpawnOres();
    }

    void SpawnOres()
    {
        // Spawn least rare ores in the entire grid
        SpawnOreInRange(ores.FindAll(o => o.rarity == 1), oreCounts[0], gridMin, gridMax);

        // Spawn second rarest ores in the -500 to -250 or 250 to 500 range
        SpawnOreInRange(ores.FindAll(o => o.rarity == 2), oreCounts[2], -500, -250, 250, 500);

        // Spawn rarest ores in the -500 to -400 or 400 to 500 range
        SpawnOreInRange(ores.FindAll(o => o.rarity == 3), oreCounts[2], -500, -400, 400, 500);

    }

    void SpawnOreInRange(List<OreWithRarity> orePrefabSet, int oreCount, int rangeMinX, int rangeMaxX, int rangeMinAltX = 0, int rangeMaxAltX = 0) { 

        List<Vector3Int> spawnPositions = new List<Vector3Int>();

        for (int i = 0; i < oreCount; i++)
        {
            bool validPosition = false;
            Vector3Int spawnPosition = Vector3Int.zero;

            for (int attempts = 0; attempts < 10; attempts++)
            {
                // Randomly choose between the two possible ranges (either primary or alternative range if provided)
                int x;
                if (rangeMinAltX != 0 || rangeMaxAltX != 0)
                {
                    x = (Random.value < 0.5f)
                        ? Random.Range(rangeMinX, rangeMaxX)
                        : Random.Range(rangeMinAltX, rangeMaxAltX);
                }
                else
                {
                    x = Random.Range(rangeMinX, rangeMaxX);
                }

                // Randomly within full grid Z-axis range
                int y = Random.Range(gridMin, gridMax);

                // Get the grid position
                spawnPosition = new Vector3Int(x, y, 1);

                // check if pos is valid 
                if (IsPositionValid(spawnPosition, spawnPositions))
                {
                    validPosition = true;
                    break;
                }
            }
            
            if (validPosition)
            {
                GameObject selectedOrePrefab = ChooseOreBasedOnRarity(orePrefabSet);
                Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                // Convert the grid position to world position for actual instantiation
                Vector3 placePosition = tilemap.GetCellCenterWorld(spawnPosition);
                placePosition.y = 1.4f;
                GameObject spawnedOre = Instantiate(selectedOrePrefab, placePosition, randomRotation);
                ApplyMaterialToOre(spawnedOre, orePrefabSet.Find(ore => ore.orePrefab == selectedOrePrefab).oreMaterial);

                spawnPositions.Add(spawnPosition);
            }
        }
    }



    bool IsPositionValid(Vector3Int position, List<Vector3Int> existingPositions, float minDistance = 2f)
    {
        foreach (Vector3Int existingPosition in existingPositions)
        {
            if (Vector3Int.Distance(position, existingPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    void ApplyMaterialToOre(GameObject ore, Material material)
    {
        if (material != null)
        {
            MeshRenderer[] meshRenderers = ore.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in meshRenderers)
            {
                renderer.material = material;
            }
        }
    }

    GameObject ChooseOreBasedOnRarity(List<OreWithRarity> orePrefabsSet)
    {
        int totalWeight = 0;
        foreach (var ore in orePrefabsSet)
        {
            totalWeight += ore.rarity;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var ore in orePrefabsSet)
        {
            currentWeight += ore.rarity;
            if (randomWeight < currentWeight)
            {
                return ore.orePrefab;
            }
        }

        return orePrefabsSet[0].orePrefab;
    }
}
