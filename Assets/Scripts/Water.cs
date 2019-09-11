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

    public void OnCollisionEnter(Collision collision)
    {
       
        MightyGamePack.MightyGameManager.gameManager.particleEffectsManager.SpawnParticleEffect(collision.contacts[0].point + new Vector3(0, 0.3f, 0), Quaternion.identity, 5, 0, "WaterSplash");
        collision.collider.gameObject.GetComponent<Collider>().enabled = false;

     
    }
}