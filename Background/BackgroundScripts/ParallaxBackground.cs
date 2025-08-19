using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Background Manager Reference")]
    [SerializeField] private BackgroundManager bgManager;

    private GameObject[] backgroundLayers;
    private int totalLayers;

    [Space(10)]
    [Header("Layer Movement Settings")]
    [SerializeField] private float[] layerSpeeds;

    public Renderer[] layerRenderers { get; private set; }
    public Vector2[] layerSpeedVectors { get; private set; }
    public Vector2[] layerTextureOffsets { get; private set; }

    private void Start()
    {
        // Lấy thông tin về các layer từ BackgroundManager
        bgManager = GetComponent<BackgroundManager>();

        backgroundLayers = bgManager.Layers;
        totalLayers = bgManager.LayerCount;

        // Gắn renderer và clone material cho từng layer
        // (clone để chỉnh offset riêng mà không ảnh hưởng tới prefab gốc)
        if (layerRenderers == null || layerRenderers.Length != totalLayers)
        {
            layerRenderers = new Renderer[totalLayers];

            for (int i = 0; i < totalLayers; i++)
            {
                layerRenderers[i] = backgroundLayers[i].GetComponent<Renderer>();
                layerRenderers[i].material = Instantiate(layerRenderers[i].material);

                // Thêm script DynamicLayer để xử lý cuộn nền
                if (layerRenderers[i].GetComponent<DynamicLayer>() == null)
                    backgroundLayers[i].AddComponent<DynamicLayer>();
            }
        }

        // Tạo giá trị tốc độ cuộn cho từng layer
        // Layer xa → tốc độ nhỏ hơn để tạo hiệu ứng chiều sâu
        if (layerSpeeds == null || layerSpeeds.Length != totalLayers ||
            layerSpeedVectors == null || layerSpeedVectors.Length != totalLayers ||
            layerTextureOffsets == null || layerTextureOffsets.Length != totalLayers)
        {
            layerSpeeds = new float[totalLayers];
            layerSpeedVectors = new Vector2[totalLayers];
            layerTextureOffsets = new Vector2[totalLayers];

            for (int i = 0; i < totalLayers; i++)
            {
                // Lerp từ 0.05f tới 0.15f → layer càng gần thì tốc độ càng cao
                layerSpeeds[i] = Mathf.Lerp(0.05f, 0.15f,
                    totalLayers <= 1 ? 0f : (float)i / (totalLayers - 1));

                // Vector2: cuộn cả X và Y theo cùng tốc độ
                layerSpeedVectors[i] = new Vector2(layerSpeeds[i], layerSpeeds[i]);
            }
        }
    }
}
