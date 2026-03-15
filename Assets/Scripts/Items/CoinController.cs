using System;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private LevelManager _levelManager;
    
    [SerializeField] private AudioClip SoundFx;

    private void Start()
    {
        _levelManager = GameObject.FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlayFX(SoundFx);
            _levelManager.AddCoin();
            Destroy(gameObject);
        }
    }
}
