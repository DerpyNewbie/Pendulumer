using UnityEngine;

public class UndergroundGimmick : MonoBehaviour
{
    [SerializeField]
    private GameObject idle;

    [SerializeField]
    private GameObject active;

    private void OnTriggerEnter2D(Collider2D other)
    {
        idle.gameObject.SetActive(false);
        active.gameObject.SetActive(true);
    }
}