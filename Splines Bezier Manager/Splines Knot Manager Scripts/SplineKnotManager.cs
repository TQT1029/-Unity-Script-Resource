#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// SplineKnotManager
/// - Quản lý các Knot (điểm điều khiển) của một Spline trong chế độ Editor.
/// - Hỗ trợ Undo/Redo, hiển thị Gizmo trực quan.
/// - Có sẵn chức năng Reset về mặc định.
/// - Cho phép đổi TangentMode của toàn bộ Knot.
/// </summary>
[ExecuteInEditMode]
public class SplineKnotManager : MonoBehaviour
{
    // ======================= [KHAI BÁO BIẾN] =======================

    [Header("Spline Settings")]
    [SerializeField] private SplineContainer splineContainer;   // Container chứa spline
    [SerializeField] private GameObject knotPrefab;              // Prefab đại diện cho một knot (có thể để trống → dùng Sphere mặc định)

    [Header("Runtime Data")]
    [SerializeField] private List<Transform> knotTransforms = new List<Transform>(); // Danh sách các Transform của knot (object trong scene)

    [Header("Gizmo Settings")]
    [SerializeField] private Color splineColor = Color.green;    // Màu vẽ đường spline
    [SerializeField] private Color knotColor = Color.magenta;    // Màu hiển thị knot
    [SerializeField] private float knotGizmoSize = 0.1f;         // Kích thước gizmo knot

    // Enum chọn chế độ tangent để đổi hàng loạt
    private enum TangentModeSelector
    {
        Auto,
        Linear,
        Mirrored,
        Continuous,
        Broken
    }
    [SerializeField] private TangentModeSelector selectedTangent;

    // ======================= [UNITY EVENTS] =======================

    /// <summary>
    /// Gọi khi bấm nút Reset trong Inspector hoặc khi Add component lần đầu.
    /// Dùng để đưa toàn bộ giá trị về mặc định và tạo lại knot.
    /// </summary>
    private void Reset()
    {
        // Thiết lập giá trị mặc định
        splineContainer = GetComponent<SplineContainer>();
        knotPrefab = null;
        splineColor = Color.green;
        knotColor = Color.magenta;
        knotGizmoSize = 0.1f;

        // Nếu chưa có splineContainer thì thêm vào
        if (splineContainer == null)
        {
            splineContainer = gameObject.AddComponent<SplineContainer>();
        }

        // Xóa toàn bộ knot cũ
        DeleteAllKnotObjects();

        // Tạo lại knot nếu spline tồn tại
        if (splineContainer != null && splineContainer.Spline != null)
        {
            CreateKnotObjects();
            UpdateObjectsFromSpline();
        }

        Debug.Log("SplineKnotManager đã được reset về mặc định.");
    }

    private void OnEnable()
    {
        // Đảm bảo luôn có splineContainer
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>() ?? gameObject.AddComponent<SplineContainer>();
        }
    }

    private void Awake()
    {
        // Xóa toàn bộ knot khi khởi tạo (tránh trùng lặp khi load)
        DeleteAllKnotObjects();
    }

    private void Update()
    {
        if (!Application.isPlaying) // Chỉ chạy trong Edit Mode
        {
            if (splineContainer == null) return;

            // Nếu số lượng knot object không khớp với spline → tạo lại
            if (knotTransforms == null || knotTransforms.Count != splineContainer.Spline.Count)
            {
                DeleteAllKnotObjects();
                CreateKnotObjects();
                UpdateObjectsFromSpline();
            }

            // Luôn đồng bộ ngược từ object sang spline
            UpdateSplineFromObjects();
        }
    }

    // ======================= [CÁC HÀM QUẢN LÝ KNOT] =======================

    /// <summary>Xóa toàn bộ knot object (hỗ trợ Undo trong Editor).</summary>
    private void DeleteAllKnotObjects()
    {
        if (splineContainer != null && splineContainer.Spline.Count > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
            }
            knotTransforms.Clear();
        }
    }

    /// <summary>Tạo tất cả knot object dựa trên spline.</summary>
    private void CreateKnotObjects()
    {
        var spline = splineContainer.Spline;
        if (spline.Count > 0)
        {
            for (int i = 0; i < spline.Count; i++)
            {
                CreateSingleKnotObject(i);
            }
        }
    }

    /// <summary>Tạo một knot object tại index chỉ định.</summary>
    private void CreateSingleKnotObject(int index)
    {
        GameObject knotObj = knotPrefab != null
            ? (GameObject)PrefabUtility.InstantiatePrefab(knotPrefab, transform)
            : GameObject.CreatePrimitive(PrimitiveType.Sphere);

        Undo.RegisterCreatedObjectUndo(knotObj, "Create Knot");

        knotObj.transform.SetParent(transform, false);
        knotObj.name = $"Knot_{index}";
        knotObj.transform.localScale = Vector3.one * 0.2f;
        DestroyImmediate(knotObj.GetComponent<SphereCollider>()); // Xóa collider mặc định

        knotTransforms.Add(knotObj.transform);
    }

    /// <summary>Đồng bộ vị trí knot object từ spline.</summary>
    private void UpdateObjectsFromSpline()
    {
        var spline = splineContainer.Spline;
        for (int i = 0; i < spline.Count; i++)
        {
            Vector3 worldPos = splineContainer.transform.TransformPoint(spline[i].Position);
            knotTransforms[i].position = worldPos;
        }
    }

    /// <summary>Đồng bộ spline từ vị trí knot object.</summary>
    private void UpdateSplineFromObjects()
    {
        var spline = splineContainer.Spline;
        for (int i = 0; i < spline.Count; i++)
        {
            Vector3 localPos = splineContainer.transform.InverseTransformPoint(knotTransforms[i].position);
            var knot = spline[i];
            knot.Position = localPos;
            spline[i] = knot;
        }
    }

    // ======================= [HIỂN THỊ GIZMO] =======================

    private void OnDrawGizmos()
    {
        if (splineContainer == null || splineContainer.Spline == null) return;

        var spline = splineContainer.Spline;
        if (spline.Count >= 2)
        {
            // Vẽ đường spline
            Gizmos.color = splineColor;
            Vector3 prevPos = splineContainer.transform.TransformPoint(spline[0].Position);
            for (int i = 1; i < spline.Count; i++)
            {
                Vector3 currPos = splineContainer.transform.TransformPoint(spline[i].Position);
                Gizmos.DrawLine(prevPos, currPos);
                prevPos = currPos;
            }

            // Vẽ các knot
            Gizmos.color = knotColor;
            foreach (var knot in spline)
            {
                Vector3 worldPos = splineContainer.transform.TransformPoint(knot.Position);
                Gizmos.DrawSphere(worldPos, knotGizmoSize);
            }
        }
    }

    // ======================= [ĐỔI TANGENT MODE HÀNG LOẠT] =======================

    [ContextMenu("Change Knot Tangent Mode")]
    private void ChangeKnotTangentMode()
    {
        TangentMode mode = TangentMode.Broken;

        // Chọn mode dựa trên enum đã chọn
        switch (selectedTangent)
        {
            case TangentModeSelector.Auto:
                mode = TangentMode.AutoSmooth;
                break;
            case TangentModeSelector.Linear:
                mode = TangentMode.Linear;
                break;
            case TangentModeSelector.Mirrored:
                mode = TangentMode.Mirrored;
                break;
            case TangentModeSelector.Continuous:
                mode = TangentMode.Continuous;
                break;
            case TangentModeSelector.Broken:
                mode = TangentMode.Broken;
                break;
        }

        // Áp dụng cho tất cả spline trong container
        foreach (var spline in splineContainer.Splines)
        {
            for (int i = 0; i < spline.Count; i++)
            {
                spline.SetTangentMode(i, mode);
            }
        }

        Debug.Log($"Đã đổi tất cả knots sang {mode}");
    }
}
#endif
