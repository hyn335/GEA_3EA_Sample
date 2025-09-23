using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{

    public float speed = 20f; // �̵� �ӵ� 

    public float lifeTime = 2f; // ���� �ð� (��)
    // Start is called before the first frame update
    void Start()
    {
        //���� �ð� �� �ڵ� ���� (�޸� ����)
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        //������ forward ����(��)���� �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Enemy"))
        {
            //�� �浿 �� �� ���� 
            Destroy(other.gameObject);
            //projectile ���� 
            Destroy(gameObject);

        }
    }
}
