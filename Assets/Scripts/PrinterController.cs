using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrinterController : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float tiempoImpresion = 10f;

    [Header("Opcional")]
    [SerializeField] private GameObject informePrefab;
    [SerializeField] private Transform puntoSalidaInforme;

    private LevelManager levelManager;
    private bool playerDentro = false;
    private bool imprimiendo = false;
    private bool yaImpreso = false;

    private void Start()
    {
        levelManager = GameObject.FindFirstObjectByType<LevelManager>();
    }

    private void Update()
    {
        if (!playerDentro)
            return;

        if (imprimiendo || yaImpreso)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(ImprimirInforme());
        }
    }

    private IEnumerator ImprimirInforme()
    {
        imprimiendo = true;
        Debug.Log("Imprimiendo informe...");

        yield return new WaitForSeconds(tiempoImpresion);

        yaImpreso = true;
        imprimiendo = false;

        if (levelManager != null)
            levelManager.MarcarInformeImpreso();

        if (informePrefab != null && puntoSalidaInforme != null)
            Instantiate(informePrefab, puntoSalidaInforme.position, Quaternion.identity);

        Debug.Log("Informe listo");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerDentro = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerDentro = false;
    }
}