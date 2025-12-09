using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoiseVoxelMap : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject blockPrefabDirt;
    public GameObject blockPrefabGrass;
    public GameObject blockPrefabWater;

    [Header("Map Size (X,Z) & Height (Y)")]
    public int width = 20;     // X
    public int depth = 20;     // Z
    public int maxHeight = 16; // Y

    [Header("Water")]
    public int waterLevel = 4; // y <= waterLevel 은 물로 채움

    [SerializeField] private float noiseScale = 20f;

   

    void Start()
    {
        // 노이즈 오프셋(시드 느낌)
        float offsetX = Random.Range(-9999f, 9999f);
        float offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 0~1 사이의 퍼린 노이즈
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;
                float noise = Mathf.PerlinNoise(nx, nz);

                // 노이즈를 최대 높이에 매핑
                int h = Mathf.FloorToInt(noise * maxHeight);
                if (h <= 0) h = 1;

                // y = 0 ~ h-1 까지는 흙, y = h 는 잔디
                for (int y = 0; y <= h; y++)
                {
                    if (y == h)
                        PlaceGrass(x, y, z);
                    else
                        PlaceDirt(x, y, z);
                }

                // 수면 이하 부분은 물로 채우기
                for (int y = h + 1; y <= waterLevel; y++)
                {
                    PlaceWater(x, y, z);
                }
            }
        }
    }
    public void PlaceTile(Vector3Int pos, ItemType type)
    {
        switch (type)
        {
            case ItemType.Dirt:
                PlaceDirt(pos.x, pos.y, pos.z);
                break;

            case ItemType.Grass:
                PlaceGrass(pos.x, pos.y, pos.z);
                break;

            case ItemType.Water:
                PlaceWater(pos.x, pos.y, pos.z);
                break;
        }
    }


    private void PlaceWater(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWater, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Water_{x}_{y}_{z}";
    }

    private void PlaceDirt(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabDirt, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Dirt_{x}_{y}_{z}";
      
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Dirt;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }

    private void PlaceGrass(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabGrass, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Grass_{x}_{y}_{z}";
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Grass;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }

   
}
