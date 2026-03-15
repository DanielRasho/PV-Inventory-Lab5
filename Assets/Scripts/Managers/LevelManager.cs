using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int coinsRequired;

    [SerializeField] private GameObject victoryUI;
    private int coins = 0;

    [SerializeField] private int informesRequired;
    private int informes = 0;
    public int Informes => informes;

    private bool informeImpreso = false;
    public bool InformeImpreso => informeImpreso;

    [SerializeField] private Toggle entregarCafeToggle;
    [SerializeField] private Toggle informeToggle;
    [SerializeField] private Toggle imprimirInformeToggle;
    

    public bool tieneCafe = false;
    public bool cafeEntregado = false;

    public bool TieneCafe => tieneCafe;
    public bool CafeEntregado => cafeEntregado;


    public void Start()
    {
        if (informeToggle != null) informeToggle.isOn = false;
        if (imprimirInformeToggle != null) imprimirInformeToggle.isOn = false;
        if (entregarCafeToggle != null) entregarCafeToggle.isOn = false;
    }


    // ---- COINS -----
    public int Coins
    {
        get { return coins; }
    }

    public void AddCoin()
    {
        if (coins < coinsRequired)
            coins++;
        Debug.Log("Coin Added: " + coins);
    }

    private void FixedUpdate()
    {
        if (informes == informesRequired && CafeEntregado == true && informeImpreso == true)
        {
            victoryUI.SetActive(true);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif
        }
    }

    // --- imprimir ---
    public void MarcarInformeImpreso()
    {
        if (informeImpreso)
            return;

        informeImpreso = true;

        if (imprimirInformeToggle != null)
            imprimirInformeToggle.isOn = true;

        Debug.Log("Informe impreso");
    }

    // --- Informe ---
    public void AddInforme()
    {
        if (informes < informesRequired)
            informes++;

        Debug.Log("Informe recogido: " + informes);

        if (informes >= informesRequired)
            informeToggle.isOn = true;
    }

    //--- cafe ---
    public void RecogerCafe()
    {
        if (tieneCafe || cafeEntregado)
            return;

        tieneCafe = true;
        Debug.Log("Cafe recogido");

    }

    public void EntregarCafe()
    {
        if (!tieneCafe || cafeEntregado)
            return;

        tieneCafe = false;
        cafeEntregado = true;

        Debug.Log("Cafe entregado a la jefa");

        if (entregarCafeToggle != null)
            entregarCafeToggle.isOn = true;
    }

}
