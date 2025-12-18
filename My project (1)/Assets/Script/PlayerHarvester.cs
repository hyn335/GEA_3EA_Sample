using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    public float rayDistance = 5f;
    public LayerMask hitMask = ~0;

    [Header("Fist")]
    public int fistDamage = 1;
    public float fistCooldown = 0.15f;

    [Header("Tools")]
    public int axeDamage = 3;
    public float axeCooldown = 0.10f;

    [Header("Bomb")]
    public int bombDamage = 999;
    public float bombCooldown = 0.25f;
    public bool bombConsumes = true;

    private float _nextHitTime;
    private Camera _cam;

    public Inventory inventory;
    InventoryUI invenUI;

    public GameObject selectedBlock;

    void Awake()
    {
        _cam = Camera.main;
        if (inventory == null) inventory = gameObject.AddComponent<Inventory>();
        invenUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        ItemType selectedType = ItemType.Dirt;
        bool hasSelection = invenUI != null && invenUI.selectedIndex >= 0;
        if (hasSelection)
            selectedType = invenUI.GetInventorySlot();

        bool toolMode = !hasSelection || IsTool(selectedType);

        if (toolMode)
        {
            if (selectedBlock) selectedBlock.transform.localScale = Vector3.zero;

            int damage = hasSelection ? GetToolDamage(selectedType) : fistDamage;
            float cooldown = hasSelection ? GetToolCooldown(selectedType) : fistCooldown;

            if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
            {
                _nextHitTime = Time.time + cooldown;

                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    var block = hit.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        // 💣 폭탄
                        if (hasSelection && selectedType == ItemType.Bomb)
                        {
                            if (!bombConsumes || inventory.Consume(ItemType.Bomb, 1))
                            {
                                Explode2D9(hit.collider.transform.position, damage);
                            }
                        }
                        else
                        {
                            // 🪓 도끼 / 👊 주먹
                            block.Hit(damage, inventory);
                        }
                    }
                }
            }
        }
        else
        {
            // ======================
            // 설치 모드
            // ======================
            Ray rayDebug = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(rayDebug, out var hitDebug, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
            {
                Vector3Int placePos = AdjacentCellOnHitFace(hitDebug);
                if (selectedBlock)
                {
                    selectedBlock.transform.localScale = Vector3.one;
                    selectedBlock.transform.position = placePos;
                    selectedBlock.transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                if (selectedBlock) selectedBlock.transform.localScale = Vector3.zero;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3Int placePos = AdjacentCellOnHitFace(hit);
                    var map = FindObjectOfType<NoiseVoxelMap>();

                    // ☃ 눈사람 설치 (2칸)
                    if (selectedType == ItemType.Snowman)
                    {
                        if (map.PlaceSnowman(placePos))
                        {
                            inventory.Consume(ItemType.Snowman, 1);
                        }
                    }
                    else
                    {
                        if (inventory.Consume(selectedType, 1))
                        {
                            map.PlaceTile(placePos, selectedType);
                        }
                    }
                }
            }
        }
    }

    // 💥 폭탄: 같은 높이에서 3x3 파괴
    void Explode2D9(Vector3 centerPos, int damage)
    {
        int cx = Mathf.RoundToInt(centerPos.x);
        int cy = Mathf.RoundToInt(centerPos.y);
        int cz = Mathf.RoundToInt(centerPos.z);

        for (int dx = -1; dx <= 1; dx++)
        for (int dz = -1; dz <= 1; dz++)
        {
            Vector3 pos = new Vector3(cx + dx, cy, cz + dz);

            Collider[] hits = Physics.OverlapBox(
                pos,
                Vector3.one * 0.45f,
                Quaternion.identity,
                hitMask,
                QueryTriggerInteraction.Ignore
            );

            foreach (var h in hits)
            {
                var b = h.GetComponent<Block>();
                if (b != null)
                    b.Hit(damage, inventory);
            }
        }
    }

    static Vector3Int AdjacentCellOnHitFace(in RaycastHit hit)
    {
        Vector3 baseCenter = hit.collider.transform.position;
        Vector3 adjCenter = baseCenter + hit.normal;
        return Vector3Int.RoundToInt(adjCenter);
    }

    bool IsTool(ItemType type)
    {
        return type == ItemType.Axe
            || type == ItemType.Sword
            || type == ItemType.Shovel
            || type == ItemType.Bomb;
    }

    int GetToolDamage(ItemType tool)
    {
        switch (tool)
        {
            case ItemType.Axe: return axeDamage;
            case ItemType.Bomb: return bombDamage;
            default: return fistDamage;
        }
    }

    float GetToolCooldown(ItemType tool)
    {
        switch (tool)
        {
            case ItemType.Axe: return axeCooldown;
            case ItemType.Bomb: return bombCooldown;
            default: return fistCooldown;
        }
    }
}