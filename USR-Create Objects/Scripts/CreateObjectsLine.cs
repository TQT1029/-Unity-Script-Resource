using UnityEngine;

/// <summary>
/// Spawn nhiều object dọc theo một đường thẳng giữa 2 điểm.
/// Có thể áp dụng cho coin, item, enemy...
/// </summary>
public class CreateObjectsLine : MonoBehaviour
{
    [Header("Cấu hình spawn")]
    [SerializeField, Min(1)]
    private int spawnCount = 5;            // Số lượng object muốn sinh ra
    [SerializeField]
    private GameObject prefabToSpawn;      // Prefab sẽ spawn
    [SerializeField]
    private Transform container;           // Nơi chứa các object spawn ra

    [Header("Điểm mốc")]
    [SerializeField] private Transform pointA; // Điểm bắt đầu
    [SerializeField] private Transform pointB; // Điểm kết thúc

    private void Start()
    {
        // Nếu chưa set prefab → thử tìm theo tên (tuỳ chỉnh cho dự án của bạn)
        if (prefabToSpawn == null)
            prefabToSpawn = transform.parent.Find("Original Coin")?.gameObject;

        // Nếu chưa set container → lấy con tên "Object Storage"
        if (container == null)
            container = transform.Find("Object Storage");

        // Nếu chưa set điểm → tìm con có tên tương ứng
        if (pointA == null)
            pointA = transform.Find("Start Point");
        if (pointB == null)
            pointB = transform.Find("End Point");

        if (prefabToSpawn == null || container == null || pointA == null || pointB == null)
        {
            Debug.LogWarning("Thiếu cấu hình spawn, hãy kiểm tra Inspector.");
            return;
        }

        // Spawn object giữa 2 điểm
        for (int i = 0; i < spawnCount; i++)
        {
            // Tính vị trí nội suy (Lerp)
            Vector3 spawnPos = Vector3.Lerp(pointA.position, pointB.position, (float)i / (spawnCount - 1));

            // Tạo object mới
            GameObject clone = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, container);

            // Đặt tên theo thứ tự
            clone.name = $"{prefabToSpawn.name}_Clone_{i + 1}";
        }
    }
}
