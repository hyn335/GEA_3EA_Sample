using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject blockPrefabDirt;
    public GameObject blockPrefabGrass;
    public GameObject blockPrefabWater;

    [Header("Tree (Trunk + Round Leaves)")]
    public GameObject blockPrefabWood;
    public GameObject blockPrefabLeaf;
    [Range(0f, 1f)] public float treeChance = 0.03f;
    public int treeMinHeight = 3;
    public int treeMaxHeight = 6;

    [Header("Leaves Shape")]
    public int leafRadius = 2; // 2 추천, 3은 풍성

    [Header("Map Size (X,Z) & Height (Y)")]
    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;

    [Header("Water")]
    public int waterLevel = 4;

    [SerializeField] private float noiseScale = 20f;

    void Start()
    {
        float offsetX = Random.Range(-9999f, 9999f);
        float offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;
                float noise = Mathf.PerlinNoise(nx, nz);

                int h = Mathf.FloorToInt(noise * maxHeight);
                if (h <= 0) h = 1;

                // 땅 생성
                for (int y = 0; y <= h; y++)
                {
                    if (y == h) PlaceGrass(x, y, z);
                    else PlaceDirt(x, y, z);
                }

                // 물 채우기
                for (int y = h + 1; y <= waterLevel; y++)
                {
                    PlaceWater(x, y, z);
                }

                // 나무 + 둥근 잎
                TrySpawnTree(x, h, z);
            }
        }
    }

    void TrySpawnTree(int x, int groundY, int z)
    {
        if (blockPrefabWood == null) return;
        if (groundY <= waterLevel) return;
        if (Random.value > treeChance) return;

        int height = Random.Range(treeMinHeight, treeMaxHeight + 1);

        // 잎 반구(leafRadius)까지 맵 높이 고려
        height = Mathf.Min(height, maxHeight - groundY - 1 - leafRadius);
        if (height <= 0) return;

        // 줄기 생성
        for (int i = 1; i <= height; i++)
        {
            PlaceWood(x, groundY + i, z);
        }

        // 둥근 잎(반구) 생성
        int topY = groundY + height;
        SpawnRoundLeaves(x, topY + 1, z, leafRadius);
    }

         // 둥글게(반구) 잎 생성
    void SpawnRoundLeaves(int cx, int cy, int cz, int radius)
    {
        if (blockPrefabLeaf == null) return;
        if (radius <= 0) return;

        for (int dy = 0; dy <= radius; dy++) // 위로만(반구)
        {
            int r = radius - dy;
            int r2 = r * r;

            for (int dx = -r; dx <= r; dx++)
            for (int dz = -r; dz <= r; dz++)
            {
                if (dx * dx + dz * dz > r2) continue;
                PlaceLeaf(cx + dx, cy + dy, cz + dz);
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

            case ItemType.Wood:
                PlaceWood(pos.x, pos.y, pos.z);
                break;

            case ItemType.Leaf:
                PlaceLeaf(pos.x, pos.y, pos.z);
                break;
        }
    }

    private void PlaceWater(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWater, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Water_{x}_{y}_{z}";

        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Water;
        b.maxHP = 2;
        b.dropCount = 1;
        b.mineable = true;
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

    private void PlaceWood(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWood, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Wood_{x}_{y}_{z}";

        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Wood;
        b.maxHP = 5;
        b.dropCount = 1;
        b.mineable = true;
    }

         //  이 함수가 빠져서 에러났던 거임 (PlaceLeaf 없음)
    private void PlaceLeaf(int x, int y, int z)
    {
        if (blockPrefabLeaf == null) return;

        var go = Instantiate(blockPrefabLeaf, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"Leaf_{x}_{y}_{z}";

        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Leaf;
        b.maxHP = 1;
        b.dropCount = 0;
        b.mineable = true;
    }
}
