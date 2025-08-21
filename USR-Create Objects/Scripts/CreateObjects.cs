using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Script spawn object theo 2 chế độ:
/// 1. Line – spawn dọc theo đường thẳng giữa 2 điểm.
/// 2. Curve – spawn dọc theo Spline. (Cần sử dụng Packaged Unity Splines và USR-Spline Manager)
/// Dùng chung cho coin, item, enemy, checkpoint...
/// </summary>
public class CreateObjects : MonoBehaviour
{
    private enum SpawnMode { Line, Curve }   // Chế độ spawn

    [Header("Chế độ spawn")]
    [SerializeField, Tooltip("1) Line: tạo vật thể cách để nhau giữa 2 điểm. 2) Curve: tạo vật thể cách đều nhau dọc theo một Spline.")]
    private SpawnMode spawnMode = SpawnMode.Line;

    [Header("Cấu hình chung")]
    [SerializeField, Min(1)]
    private int objectCount = 5;              // Số lượng object spawn
    [SerializeField]
    private GameObject prefab;                // Prefab spawn
    [SerializeField]
    private Transform containerParent;        // Nơi chứa object spawn

    [Header("Cấu hình cho Line Mode")]
    [SerializeField]
    private Transform startPoint;             // Điểm bắt đầu (Line)
    [SerializeField]
    private Transform endPoint;               // Điểm kết thúc (Line)

    [Header("Cấu hình cho Curve Mode")]
    [SerializeField]
    private SplineContainer spline;           // Spline (Curve)

    private void Start()
    {
        // Kiểm tra điều kiện chung
        if (prefab == null)
        {
            Debug.LogWarning("Chưa gán prefab!");
            return;
        }

        // Nếu chưa có container → tạo mới
        if (containerParent == null)
        {
            containerParent = new GameObject("Spawned Objects").transform;
            containerParent.SetParent(transform);
        }

        // Gọi hàm spawn theo chế độ
        switch (spawnMode)
        {
            case SpawnMode.Line:
                SpawnAlongLine();
                break;
            case SpawnMode.Curve:
                SpawnAlongCurve();
                break;
        }
    }

    /// <summary>
    /// Spawn đều giữa 2 điểm
    /// </summary>
    private void SpawnAlongLine()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogWarning("Line Mode: Chưa gán StartPoint hoặc EndPoint!");
            return;
        }

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 spawnPos = Vector3.Lerp(startPoint.position, endPoint.position, (float)i / (objectCount - 1));
            CreateInstance(spawnPos);
        }
    }

    /// <summary>
    /// Spawn đều trên spline
    /// </summary>
    private void SpawnAlongCurve()
    {
        if (spline == null)
            spline = GetComponent<SplineContainer>();

        if (spline == null)
        {
            Debug.LogWarning("Curve Mode: Chưa gán spline!");
            return;
        }

        for (int i = 0; i < objectCount; i++)
        {
            float t = (float)i / (objectCount - 1);
            Vector3 pos = spline.EvaluatePosition(t);
            CreateInstance(pos);
        }
    }

    /// <summary>
    /// Hàm tạo instance & gán parent, tên
    /// </summary>
    private void CreateInstance(Vector3 position)
    {
        GameObject clone = Instantiate(prefab, position, Quaternion.identity, containerParent);
        clone.name = $"{prefab.name}_Clone_{containerParent.childCount}";
    }
}
