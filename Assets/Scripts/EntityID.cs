#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[ExecuteAlways]
public class UniqueID : MonoBehaviour
{
    [SerializeField] private string id;

    public string ID => id;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (string.IsNullOrEmpty(id))
        {
            GenerateID();
        }
    }

    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);
    }

    // Detect duplicates in scene
    private void Awake()
    {
        if (!Application.isPlaying)
        {
            EnsureUnique();
        }
    }

    private void EnsureUnique()
    {
        var objects = FindObjectsOfType<UniqueID>();

        foreach (var obj in objects)
        {
            if (obj != this && obj.id == this.id)
            {
                GenerateID();
                break;
            }
        }
    }
#endif
}
