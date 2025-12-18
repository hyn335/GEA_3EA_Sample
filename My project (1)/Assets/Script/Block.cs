using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Dirt, Grass, Water,
    Axe, Sword, Shovel,
    Wood, Leaf, Bomb,

    Snowman,        // 인벤/설치용(아이템)
    SnowmanBody,    // 프리팹 몸통 블록
    SnowmanHead     // 프리팹 머리 블록 (부수면 전체 파괴)
}

public class Block : MonoBehaviour
{
    [Header("Bolck Stat")]
    public ItemType type = ItemType.Dirt;

    public int maxHP = 3;
    [HideInInspector] public int hp;

    public int dropCount = 1;
    public bool mineable = true;

    private void Awake()
    {
        hp = maxHP;
        if (GetComponent<Collider>() == null) gameObject.AddComponent<BoxCollider>();
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
            gameObject.tag = "Block";
    }

    public void Hit(int damage, Inventory inven)
    {
        if (!mineable) return;

        hp -= damage;

        if (hp <= 0)
        {
            //  드랍
            if (inven != null && dropCount > 0)
                inven.Add(type, dropCount);

            //  눈사람 머리 부수면 전체 파괴(부모 오브젝트)
            if (type == ItemType.SnowmanHead && transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}
