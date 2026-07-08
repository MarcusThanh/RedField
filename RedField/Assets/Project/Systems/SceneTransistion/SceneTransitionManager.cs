using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager _instance;
    private static bool _hasInitializedOnce = false;

    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneTransitionManager>();
                
                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("SceneTransitionManager");
                    _instance = managerObject.AddComponent<SceneTransitionManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Cài đặt UI")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    [Header("Cài đặt Vị trí")]
    public float spawnOffset = 1f;

    public bool IsTransitioning { get; private set; }

    private PortalType spawnTargetType;
    private bool isFirstLoad = true;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Nếu đã init trước đó (được tạo từ scene khác), không bỏ qua scene hiện tại
            if (_hasInitializedOnce)
            {
                isFirstLoad = false;
            }
            _hasInitializedOnce = true;

            // Nếu fadeCanvasGroup chưa được assign, tìm hoặc tạo Canvas
            if (fadeCanvasGroup == null)
            {
                SetupFadeCanvas();
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void SetupFadeCanvas()
    {
        // Tìm CanvasGroup hiện có trong scene
        fadeCanvasGroup = FindObjectOfType<CanvasGroup>();

        if (fadeCanvasGroup == null)
        {
            Debug.Log("[SceneTransitionManager] Creating fade Canvas...");
            
            // Tạo Canvas mới
            GameObject canvasObject = new GameObject("FadeCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            
            fadeCanvasGroup = canvasObject.AddComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f;

            // Tạo Image (màu đen) để che màn hình
            GameObject imageObject = new GameObject("FadeImage");
            imageObject.transform.SetParent(canvasObject.transform, false);
            
            Image fadeImage = imageObject.AddComponent<Image>();
            fadeImage.color = Color.black;
            
            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            DontDestroyOnLoad(canvasObject);
            
            Debug.Log("[SceneTransitionManager] Fade Canvas created successfully!");
        }
        else
        {
            Debug.Log("[SceneTransitionManager] Found existing CanvasGroup in scene");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"[SceneTransitionManager] Initialized and registered to scenes");
    }
    
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public void TransitionToScene(string sceneName, PortalType targetType)
    {
        spawnTargetType = targetType;
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        IsTransitioning = true;

        // 1. Fade Out 
        yield return StartCoroutine(Fade(1f));

        // 2. Load Scene
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return null;

        // 3. Fade In
        yield return StartCoroutine(Fade(0f));

        IsTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("[SceneTransitionManager] CanvasGroup not found - skipping fade effect!");
            yield break;
        }

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Bỏ qua lần bật game đầu tiên
        if (isFirstLoad)
        {
            Debug.Log($"[SceneTransitionManager] Skipping first scene load: {scene.name}");
            isFirstLoad = false;
            return;
        }

        Debug.Log($"[SceneTransitionManager] Spawning player at {spawnTargetType} in scene {scene.name}");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) 
        {
            Debug.LogWarning("[SceneTransitionManager] Player not found in scene!");
            return;
        }

        Portal[] portals = FindObjectsOfType<Portal>();

        foreach (Portal portal in portals)
        {
            if (portal.currentPortalType == spawnTargetType)
            {
                Vector3 spawnPosition = portal.transform.position;

                // offset 1f để an toàn thoát khỏi trigger
                if (spawnTargetType == PortalType.Portal_Left)
                {
                    spawnPosition.x += spawnOffset;
                }
                else if (spawnTargetType == PortalType.Portal_Right)
                {
                    spawnPosition.x -= spawnOffset;
                }

                Debug.Log($"[SceneTransitionManager] Spawning player at {spawnPosition}");
                player.transform.position = spawnPosition;
                break;
            }
        }
    }
}