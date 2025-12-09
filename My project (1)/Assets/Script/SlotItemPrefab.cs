using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

public class SlotItemPrefab : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public TextMeshProUGUI itemText;
    public ItemType blockType;
    public CraftingPanel craftingPanel;

    public void ItemSetting(Sprite itemSprite, string txt, ItemType type)
    {
        itemImage.sprite = itemSprite;
        itemText.text = txt;
        blockType = type; 
        
    }
    private void Awake()
    {
        if (!craftingPanel)
            craftingPanel = FindObjectOfType<CraftingPanel>(true);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (!craftingPanel) return;

        craftingPanel.AddPlanned(blockType, 1);
    }

    //인벤토리 업데이트 시 호출 
    public void UpdateInventory(Inventory myInven)
    {
        // 1. 기존 슬롯 초기화

        // 2. 내 인벤토리 데이터를 전체 탐새 

        foreach (var item in myInven.items)
        {
            switch (item.Key)
            {
                case ItemType.Dirt:
                 
                    break;
                case ItemType.Grass:

                    break;

                        case ItemType.Water:
                    break;
            }
        }
    }
}
