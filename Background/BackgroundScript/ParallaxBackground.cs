/*
File này cần đi kèm với file BackgroundManager 
và cả 2 cùng nằm trong object chứa các layer của background (tạm gọi backgroundObject).
backgroundObject này phải nằm trong Camera.
*/

using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Get Varriable Data")]
    [SerializeField] private BackgroundManager backgroundManager;
    private GameObject[] layers;
    private int layersCounting;

    [Space(10)]
    [Header("Layers Config")]

    [SerializeField] private float[] layersSpeeds;
    public Renderer[] layersRenderer { get; private set; }
    public Vector2[] layersVector { get; private set; }
    public Vector2[] layersOffset { get; private set; }

    private void Start()
    {
        backgroundManager = GetComponent<BackgroundManager>();


        layers = backgroundManager.layers;
        layersCounting = backgroundManager.layersCounting;

        if (layersRenderer == null || layersRenderer.Length != layersCounting)
        {
            layersRenderer = new Renderer[layersCounting];


            for (int i = 0; i < layersCounting; i++)
            {
                layersRenderer[i] = layers[i].GetComponent<Renderer>();

                layersRenderer[i].material = Instantiate(layersRenderer[i].material);
                if (layersRenderer[i].GetComponent<DyanmicLayer>() == null)
                    layers[i].AddComponent<DyanmicLayer>();
            }
        }

        if (layersSpeeds == null || layersSpeeds.Length != layersCounting ||
            layersVector == null || layersVector.Length != layersCounting ||
            layersOffset == null || layersOffset.Length != layersCounting)
        {
            layersSpeeds = new float[layersCounting];
            layersVector = new Vector2[layersCounting];
            layersOffset = new Vector2[layersCounting];

            for (int i = 0; i < backgroundManager.layersCounting; i++)
            {
                layersSpeeds[i] = Mathf.Lerp(0.05f, 0.15f, layersCounting <= 1 ? 0f : (float)i / (layersCounting - 1));
                layersVector[i] = new Vector2(layersSpeeds[i], layersSpeeds[i]);
            }
        }
    }

}
