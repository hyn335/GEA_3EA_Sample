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
    public int axeDamage = 3;          // 도끼 데미지 (원하는 값으로)
    public float axeCooldown = 0.10f;  // 도끼 연타 간격(선택)

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
        // 슬롯 선택 여부에 따라 현재 선택 아이템 타입 가져오기
        ItemType selectedType = ItemType.Dirt; // 기본값(의미 없음)
        bool hasSelection = invenUI != null && invenUI.selectedIndex >= 0;
        if (hasSelection)
            selectedType = invenUI.GetInventorySlot();

        //  도구 선택이면 설치 모드가 아니라 "수확 모드"로 처리
        bool toolMode = !hasSelection || IsTool(selectedType);

        if (toolMode)
        {
            // 도구 모드에서는 프리뷰 숨김
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
                        block.Hit(damage, inventory);
                    }
                }
            }
        }
        else
        {
            //  설치 모드(블록류만)
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

                    // selectedType은 이미 가져온 값
                    if (inventory.Consume(selectedType, 1))
                    {
                        FindObjectOfType<NoiseVoxelMap>().PlaceTile(placePos, selectedType);
                    }
                }
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
        return type == ItemType.Axe || type == ItemType.Sword || type == ItemType.Shovel;
    }

    int GetToolDamage(ItemType tool)
    {
        switch (tool)
        {
            case ItemType.Axe: return axeDamage;
            default: return fistDamage;
        }
    }

    float GetToolCooldown(ItemType tool)
    {
        switch (tool)
        {
            case ItemType.Axe: return axeCooldown;
            default: return fistCooldown;
        }
    }
}
