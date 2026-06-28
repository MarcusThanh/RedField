using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }

    private float currentDayIntensity;
    private float currentNightIntensity;
    private Light2D currentGlobalLight;

    private bool isNightNow = false; // Trạng thái Sáng/Tối xuyên suốt game

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// API: Nhận trực tiếp thông số dạng số float từ Scene gửi lên
    /// </summary>
    public void RegisterSceneValues(float dayInten, float nightInten, Light2D localGlobalLight)
    {
        currentDayIntensity = dayInten;
        currentNightIntensity = nightInten;
        currentGlobalLight = localGlobalLight;

        ApplyLighting();
    }

    /// <summary>
    /// API CHÍNH: Gọi để đổi trạng thái Sáng / Tối lập tức
    /// </summary>
    public void SetLightingState(bool isNight)
    {
        isNightNow = isNight;
        ApplyLighting();
    }

    private void ApplyLighting()
    {
        if (currentGlobalLight == null)
        {
            Debug.LogWarning("RedField Warning: Scene hiện tại chưa gán đèn Global Light 2D!");
            return;
        }

        // Ép thẳng số float vào cường độ đèn
        currentGlobalLight.intensity = isNightNow ? currentNightIntensity : currentDayIntensity;
    }
}