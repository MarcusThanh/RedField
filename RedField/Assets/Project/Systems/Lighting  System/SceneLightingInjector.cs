using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SceneLightingInjector : MonoBehaviour
{
    [Header("Cấu hình Ánh sáng (Chỉnh tay trực tiếp cho Scene này)")]
    [SerializeField] private float dayIntensity = 0.6f;   
    [SerializeField] private float nightIntensity = 0.1f;

    [Header("Đèn môi trường")]
    [SerializeField] private Light2D sceneGlobalLight;

    private void Start()
    {
        if (LightingManager.Instance != null)
        {
            // Nộp thẳng 2 con số float và cái đèn lên cho Manager
            LightingManager.Instance.RegisterSceneValues(dayIntensity, nightIntensity, sceneGlobalLight);
        }
        else
        {
            Debug.LogError("RedField Error: Không tìm thấy LightingManager trong game!");
        }
    }
}