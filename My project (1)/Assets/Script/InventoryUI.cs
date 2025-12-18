using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    #region // 큐브 안 스프라이트
    public Sprite dirtSprite;
    public Sprite diamondSprite;
    public Sprite grassSprite;
    public Sprite waterSprite;
    public Sprite cloudSprite;
    public Sprite axeSprite;
    public Sprite WoodSprite;
    public Sprite LeafSprite;
    public Sprite BombSprite;
    public Sprite SnowmanSprite;

    #endregion

    // 슬롯 / 아이템 관련
    public List<Transform> Slot = new List<Transform>();      // UI 내의 각 슬롯들 리스트
    public GameObject slotItem;                               // 슬롯 내부에 들어가는 아이템 프리팹
    List<GameObject> items = new List<GameObject>();          // 생성된 아이템 오브젝트 전체 리스트

    // 현재 선택된 슬롯 인덱스 (-1 = 선택 없음)
    public int selectedIndex = -1;

    // 숫자키(1~9)로 슬롯 선택
    private void Update()
    {
        for (int i = 0; i < Mathf.Min(9, Slot.Count); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetSelectedIndex(i);
            }
        }
    }

    // 인벤토리 내용을 UI에 반영
    public void UpdateInventory(Inventory myInven)
    {
        // 1. 기존 슬롯 초기화
        foreach (var slotItems in items)
        {
            Destroy(slotItems); // 기존 슬롯 아이템 GameObject 삭제
        }

        items.Clear();          // 아이템 리스트 초기화
        selectedIndex = -1;     // 선택 초기화
        ResetSelection();       // 슬롯 색상 초기화

        // 2. UI에 들어갈 데이터들 전체 탐색
        int idx = 0;

        foreach (var item in myInven.items)
        {
            if (idx >= Slot.Count) break; // 슬롯 개수보다 많으면 중단

            #region // 슬롯 아이템 생성
            GameObject go = Instantiate(slotItem, Slot[idx].transform);
            go.transform.localPosition = Vector3.zero;

            SlotItemPrefab slotItemPrefab = go.GetComponent<SlotItemPrefab>();
            items.Add(go); // 아이템 리스트에 추가
            #endregion

            switch (item.Key) // 블록 타입별 아이템 추가
            {
                case ItemType.Dirt:
                    slotItemPrefab.ItemSetting(dirtSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Grass:
                    slotItemPrefab.ItemSetting(grassSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Water:
                    slotItemPrefab.ItemSetting(waterSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Axe:
                    slotItemPrefab.ItemSetting(axeSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Wood:
                    slotItemPrefab.ItemSetting(WoodSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Leaf:
                    slotItemPrefab.ItemSetting(LeafSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Bomb:
                    slotItemPrefab.ItemSetting(BombSprite, "x" + item.Value.ToString(), item.Key);
                    break;

                case ItemType.Snowman:
                    slotItemPrefab.ItemSetting(SnowmanSprite, "x" + item.Value.ToString(), item.Key);
                    break;



            }

            idx++; // 다음 슬롯 인덱스
        }
    }

    // 선택 인덱스 설정 (같은 슬롯 다시 누르면 해제)
    public void SetSelectedIndex(int idx)
    {
        ResetSelection();

        // 같은 인덱스 다시 선택하면 선택 해제
        if (selectedIndex == idx)
        {
            selectedIndex = -1;
        }
        else
        {
            // 아이템이 없는 슬롯 선택 시 선택 해제
            if (idx >= items.Count)
            {
                selectedIndex = -1;
            }
            else
            {
                SetSelection(idx);
                selectedIndex = idx;
            }
        }
    }

   
    public void ResetSelection()
    {
        foreach (var slot in Slot)
        {
            slot.GetComponent<Image>().color = Color.white;
        }
    }

    
    void SetSelection(int _idx)
    {
        Slot[_idx].GetComponent<Image>().color = Color.yellow;
    }

    // 현재 선택된 슬롯의 블록 타입 반환
    public ItemType GetInventorySlot()
    {
        return items[selectedIndex].GetComponent<SlotItemPrefab>().blockType;
    }
}