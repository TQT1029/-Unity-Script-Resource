using UnityEngine;

[ExecuteInEditMode]
public class BackgroundManager : MonoBehaviour
{

    [Header("Auto Calculate Z world pos"), Tooltip("Automatically calculate z position.")]
    [SerializeField] private float maxZPos = 100f;
    [SerializeField] private float minZPos = 50f;

    public GameObject[] layers { get; private set; }
    public int layersCounting { get; private set; }

    private void Awake()
    {
        if (layers == null || layersCounting == 0)
        {
            layersCounting = transform.childCount;

            layers = new GameObject[layersCounting];

            for (int i = 0; i < layersCounting; i++)
            {
                layers[i] = transform.GetChild(i).gameObject;

                if (layers[i].name != $"Layer-{i}") layers[i].name = $"Layer-{i}";

                float zPos = Mathf.Lerp(maxZPos, minZPos, layersCounting <= 1 ? 10f : (float)i / (layersCounting - 1));
                layers[i].transform.position = new Vector3(layers[i].transform.position.x, layers[i].transform.position.y, zPos);
            }
        }
    }
}
