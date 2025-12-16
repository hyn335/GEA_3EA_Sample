using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Tooltip("이동할 씬 이름 (Build Settings에 등록된 이름과 동일)")]
    public string targetSceneName = "Map2";

    [Tooltip("플레이어 태그")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        SceneManager.LoadScene(targetSceneName);
    }
}