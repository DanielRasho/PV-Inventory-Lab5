using UnityEngine;

public class CafeController : MonoBehaviour
{
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (levelManager != null)
            levelManager.RecogerCafe();

        // Buscar el punto de agarre del jugador
        Transform holdPoint = other.transform.Find("HoldPoint");

        if (holdPoint != null)
        {
            // Hacer hijo del jugador
            transform.SetParent(holdPoint);

            // Posición local sobre la cabeza
            transform.localPosition = Vector3.zero;
        }

        // Desactivar físicas
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;
    }
}