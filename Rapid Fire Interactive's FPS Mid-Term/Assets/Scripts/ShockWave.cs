using System.Collections;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public float maxScale = 5f;           // Maximum size of the shockwave
    public float expansionSpeed = 1f;     // Speed at which the shockwave expands
    public float fadeDuration = 2f;       // Time in seconds for the shockwave to fully fade

    private Material shockwaveMaterial;
    private float initialAlpha;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
           
            shockwaveMaterial = new Material(renderer.material);
            renderer.material = shockwaveMaterial; 

            initialAlpha = shockwaveMaterial.color.a;
        }

        // Start expanding and fading
        StartCoroutine(ExpandAndFade());
    }

    private IEnumerator ExpandAndFade()
    {
        Vector3 initialScale = transform.localScale;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            // Expanding the shockwave
            float scale = Mathf.Lerp(initialScale.x, maxScale, timer / fadeDuration);
            transform.localScale = new Vector3(scale, transform.localScale.y, scale);

            // Fading out the shockwave
            if (shockwaveMaterial != null)
            {
                Color color = shockwaveMaterial.color;
                color.a = Mathf.Lerp(initialAlpha, 0f, timer / fadeDuration);
                shockwaveMaterial.color = color;
            }

            yield return null;
        }

        // Destroy the shockwave object once it's fully faded
        Destroy(gameObject);
    }
}