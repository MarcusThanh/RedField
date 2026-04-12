using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlickeringLight : MonoBehaviour
{
    private Light2D _light;

    [Header("Cấu hình ánh sáng")]
    public float baseIntensity = 1.0f;     // Cường độ gốc
    public float flickerAmount = 0.17f;    //biên độ nhấp nháy
    public float speed = 3f;            // Tốc độ

    private float _randomOffset;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _randomOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(_randomOffset, Time.time * speed);

        _light.intensity = baseIntensity + (noise - 0.5f) * flickerAmount;
    }
}