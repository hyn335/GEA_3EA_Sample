using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject blockPrefab;     // 기본 블록 (예: 흙)
    public GameObject grassPrefab;     // 잔디 블록 (맨 위층)
    public GameObject waterPrefab;     // 물 블록 (수면 이하 채움용)

    [Header("Terrain Settings")]
    public int width = 20;             // 가로 (x)
    public int depth = 20;             // 세로 (z)
    public int maxHeight = 16;         // 최대 높이 (y)
    public int waterLevel = 4;         // 수면 높이

    [SerializeField] float noiseScale = 20f; // Perlin Noise 스케일 (값이 높을수록 평평)

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

                if (h <= 0) continue;

                // --- 땅 생성 ---
                for (int y = 0; y <= h; y++)
                {
                    if (y == h)
                        Place(grassPrefab, x, y, z); // 맨 위는 잔디
                    else
                        Place(blockPrefab, x, y, z); // 나머지는 흙
                }

                // --- 물 생성 ---
                // 땅 높이보다 낮고 waterLevel 이하인 경우 물로 채움
                if (h < waterLevel)
                {
                    for (int y = h + 1; y <= waterLevel; y++)
                    {
                        Place(waterPrefab, x, y, z);
                    }
                }
            }
        }
    }

    private void Place(GameObject prefab, int x, int y, int z)
    {
        var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"{prefab.name}_{x}_{y}_{z}";
    }
}