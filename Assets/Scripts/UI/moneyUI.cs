using TMPro;
using UnityEngine;

public class moneyUI : MonoBehaviour
{
    public static moneyUI Instance;

    private TMP_Text text;

    private void Awake()
    {
        Instance = this;
        text = GetComponent<TMP_Text>();
        text.text = "Money: 0";
    }

    public void updateCounter(int count)
    {
        text.text = "Money: " + count;
    }
}
