using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemPrefab : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemText;
    public BlockType blockType;

    public void ItemSetting(Sprite itemSprite, string txt, BlockType type)
    {
        itemImage.sprite = itemSprite;
        itemText.text = txt;
        blockType = type; 
        
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
                case BlockType.Dirt:
                    //Dirt 아이템을 슬롯에 생성
                    // instantiate 활용

                    break;
                case BlockType.Grass:

                    break;

                        case BlockType.Water:
                    break;
            }
        }
    }
}
