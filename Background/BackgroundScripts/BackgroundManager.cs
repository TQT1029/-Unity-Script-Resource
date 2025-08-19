using UnityEngine;

[ExecuteInEditMode] // Cho phép tính toán ngay cả khi chưa Play trong Unity
public class BackgroundManager : MonoBehaviour
{
    [Header("Auto Calculate Z Position")]
    [Tooltip("Automatically calculates Z position for each background layer.")]
    [SerializeField] private float farthestZ = 100f; // Layer xa nhất
    [SerializeField] private float nearestZ = 50f;   // Layer gần nhất

    public GameObject[] Layers { get; private set; }
    public int LayerCount { get; private set; }

    private void Awake()
    {
        // Lấy danh sách tất cả layer con của object này
        if (Layers == null || LayerCount == 0)
        {
            LayerCount = transform.childCount;
            Layers = new GameObject[LayerCount];

            for (int i = 0; i < LayerCount; i++)
            {
                Layers[i] = transform.GetChild(i).gameObject;

                // Đặt tên theo format để dễ quản lý
                if (Layers[i].name != $"Layer-{i}")
                    Layers[i].name = $"Layer-{i}";

                // Xác định vị trí Z theo thứ tự xa → gần
                // Layer xa nhất có Z = farthestZ, gần nhất có Z = nearestZ
                float zPos = Mathf.Lerp(farthestZ, nearestZ,
                    LayerCount <= 1 ? 1f : (float)i / (LayerCount - 1));

                Layers[i].transform.position = new Vector3(
                    Layers[i].transform.position.x,
                    Layers[i].transform.position.y,
                    zPos
                );
            }
        }
    }
}
