using UnityEngine;

public class VfxAutoDestroy : MonoBehaviour
{
    public float lifetime = 1f;

    void OnEnable()
    {
        Destroy(gameObject, Mathf.Max(0.05f, lifetime));
    }
}
