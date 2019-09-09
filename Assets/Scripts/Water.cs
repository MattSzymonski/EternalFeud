using UnityEngine;

public class Water : MonoBehaviour
{
    // Scroll main texture based on time

    public float scrollSpeed = 0.5f;
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;

        rend.material.SetTextureOffset("_BaseColorMap", new Vector2(offset, 0));
    }
}