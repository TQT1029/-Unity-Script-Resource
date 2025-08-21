using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Spawn nhiều object dọc theo một Spline.
/// Có thể áp dụng cho đường cong hoặc thẳng.
/// Yêu cầu: Unity Spline Package và script SplineKnotManager.
/// </summary>
public class CreateObjectsCurve : MonoBehaviour
{
    [Header("Cấu hình spawn")]
    [SerializeField]
    private SplineContainer splineContainer; // Spline cần dùng
    [SerializeField, Min(2)]
    private int spawnCount = 5;              // Số lượng object spawn
    [SerializeField]
    private GameObject prefabToSpawn;        // Prefab sẽ spawn
    [SerializeField]
    private Transform container;             // Nơi chứa object spawn ra

    private void Start()
    {
        // Nếu chưa set splineContainer → lấy trên chính object này
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();

        // Nếu chưa set prefab → tìm theo tên (chỉ là ví dụ)
        if (prefabToSpawn == null)
            prefabToSpawn = transform.Find("Original Object")?.gameObject;

        // Nếu chưa set container → tạo mới
        if (container == null)
        {
            container = new GameObject("Spawned Objects").transform;
            container.SetParent(transform);
        }

        if (splineContainer == null || prefabToSpawn == null)
        {
            Debug.LogWarning("Thiếu spline hoặc prefab để spawn.");
            return;
        }

        // Spawn theo spline
        for (int i = 0; i < spawnCount; i++)
        {
            float t = (float)i / (spawnCount - 1); // giá trị 0 → 1
            Vector3 spawnPos = splineContainer.EvaluatePosition(t);

            GameObject clone = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, container);
            clone.name = $"{prefabToSpawn.name}_Clone_{i + 1}";
        }
    }
}
