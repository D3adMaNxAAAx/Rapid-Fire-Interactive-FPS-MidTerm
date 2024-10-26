using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    Coroutine explosionShake;
    bool isNotDead = true;
    public void setIsNotDead(bool alive) {
        isNotDead = alive;
    }
    public bool getIsNotDead() { return isNotDead; }

    public void TriggerShake(float intensity, float duration)
    {
        StartCoroutine(Shake(intensity, duration));
    }

    
    private IEnumerator Shake(float intensity, float duration)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration && isNotDead)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity + 0.75f;
            transform.localPosition = new Vector3(x, y, originalPos.z + 0.25f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
