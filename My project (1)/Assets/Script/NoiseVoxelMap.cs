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
    public int leafRadius = 2;

    [Header("Snowman Prefabs")]
    public GameObject snowmanBodyPrefab; // 아래
    public GameObject snowmanHeadPrefab; // 위

    [Header("Map Size (X,Z) & Height (Y)")]
    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;

    [Header("Water")]
    public int waterLevel = 4;

    [Header("Portal Spawn")]
    public GameObject portalPrefab;
    public int portalX = 2;
    public int portalZ = 2;
    public int portalYOffset = 1;

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

                for (int y = 0; y <= h; y++)
                {
                    if (y == h) PlaceGrass(x, y, z);
                    else PlaceDirt(x, y, z);
                }

                for (int y = h + 1; y <= waterLevel; y++)
                {
                    PlaceWater(x, y, z);
                }

                TrySpawnTree(x, h, z);

                if (portalPrefab != null && x == portalX && z == portalZ)
                {
                    Vector3 pos = new Vector3(x, h + portalYOffset, z);
                    Instantiate(portalPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }

    void TrySpawnTree(int x, int groundY, int z)
    {
        if (blockPrefabWood == null) return;
        if (groundY <= waterLevel) return;
        if (Random.value > treeChance) return;

        int height = Random.Range(treeMinHeight, treeMaxHeight + 1);
        height = Mathf.Min(height, maxHeight - groundY - 1 - leafRadius);
        if (height <= 0) return;

        for (int i = 1; i <= height; i++)
            PlaceWood(x, groundY + i, z);

        int topY = groundY + height;
        SpawnRoundLeaves(x, topY + 1, z, leafRadius);
    }

    void SpawnRoundLeaves(int cx, int cy, int cz, int radius)
    {
        if (blockPrefabLeaf == null) return;

        for (int dy = 0; dy <= radius; dy++)
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

    // =========================
    // 설치 처리
    // =========================
    public void PlaceTile(Vector3Int pos, ItemType type)
    {
        switch (type)
        {
            case ItemType.Dirt: PlaceDirt(pos.x, pos.y, pos.z); break;
            case ItemType.Grass: PlaceGrass(pos.x, pos.y, pos.z); break;
            case ItemType.Water: PlaceWater(pos.x, pos.y, pos.z); break;
            case ItemType.Wood: PlaceWood(pos.x, pos.y, pos.z); break;
            case ItemType.Leaf: PlaceLeaf(pos.x, pos.y, pos.z); break;

            case ItemType.Snowman:
                PlaceSnowman(pos);
                break;
        }
    }

    // =========================
    // ☃️ 눈사람 설치 (2칸 + 부모 묶기 + 타입 세팅)
    // =========================
    public bool PlaceSnowman(Vector3Int basePos)
    {
        if (snowmanBodyPrefab == null || snowmanHeadPrefab == null) return false;

        Vector3Int bodyPos = basePos;
        Vector3Int headPos = basePos + Vector3Int.up;

        if (IsOccupied(bodyPos) || IsOccupied(headPos)) return false;

        // 부모 오브젝트 생성(머리 부수면 전체 파괴를 안정적으로)
        GameObject root = new GameObject($"Snowman_{basePos.x}_{basePos.y}_{basePos.z}");
        root.transform.SetParent(transform);
        root.transform.position = Vector3.zero;

        // 몸통
        var body = Instantiate(snowmanBodyPrefab, (Vector3)bodyPos, Quaternion.identity, root.transform);
        var bodyBlock = body.GetComponent<Block>() ?? body.AddComponent<Block>();
        bodyBlock.type = ItemType.SnowmanBody;

        // 머리
        var head = Instantiate(snowmanHeadPrefab, (Vector3)headPos, Quaternion.identity, root.transform);
        var headBlock = head.GetComponent<Block>() ?? head.AddComponent<Block>();
        headBlock.type = ItemType.SnowmanHead;

        return true;
    }

    bool IsOccupied(Vector3Int pos)
    {
        var hits = Physics.OverlapBox((Vector3)pos, Vector3.one * 0.45f);
        foreach (var h in hits)
            if (h.GetComponent<Block>() != null) return true;

        return false;
    }

    // =========================
    // 블록 생성 함수들
    // =========================
    void PlaceWater(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWater, new Vector3(x, y, z), Quaternion.identity, transform);
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Water;
        b.maxHP = 2;
        b.dropCount = 1;
        b.mineable = true;
    }

    void PlaceDirt(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabDirt, new Vector3(x, y, z), Quaternion.identity, transform);
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Dirt;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }

    void PlaceGrass(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabGrass, new Vector3(x, y, z), Quaternion.identity, transform);
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Grass;
        b.maxHP = 3;
        b.dropCount = 1;
        b.mineable = true;
    }

    void PlaceWood(int x, int y, int z)
    {
        var go = Instantiate(blockPrefabWood, new Vector3(x, y, z), Quaternion.identity, transform);
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Wood;
        b.maxHP = 5;
        b.dropCount = 1;
        b.mineable = true;
    }

    void PlaceLeaf(int x, int y, int z)
    {
        if (blockPrefabLeaf == null) return;
        var go = Instantiate(blockPrefabLeaf, new Vector3(x, y, z), Quaternion.identity, transform);
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = ItemType.Leaf;
        b.maxHP = 1;
        b.dropCount = 0;
        b.mineable = true;
    }
}
