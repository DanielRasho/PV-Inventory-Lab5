using Unity.VisualScripting;
using UnityEngine;

public class JefaController : MonoBehaviour
{
    private LevelManager levelManager;

    public Transform cafeSpawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!levelManager.TieneCafe)
            return;

        levelManager.EntregarCafe();

        Transform holdPoint = other.transform.Find("HoldPoint");
        if (holdPoint != null && holdPoint.childCount > 0)
        {
            Transform cafe = holdPoint.GetChild(0);
            cafe.SetParent(cafeSpawnPoint);
            cafe.localPosition = Vector3.zero;
            
        }
    }

    private void Start()
    {
        levelManager = GameObject.FindFirstObjectByType<LevelManager>();
    }


}