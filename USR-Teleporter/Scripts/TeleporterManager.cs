using UnityEngine;

/// <summary>
/// Quản lý toàn bộ cổng dịch chuyển trong scene.
/// - Tìm các "Enter portal" và "Exit point" theo cấu trúc đặt tên cụ thể.
/// - Tự động gắn script Teleporter vào các cổng vào.
/// </summary>
public class TeleporterManager : MonoBehaviour
{
    // Tham chiếu tới cổng vào số 1 và điểm ra của nó
    private Transform enterPortal_1;
    public Transform exitPoint_1 { get; private set; }

    // Tham chiếu tới cổng vào số 2 và điểm ra của nó
    private Transform enterPortal_2;
    public Transform exitPoint_2 { get; private set; }

    private void Awake()
    {
        // Tìm cổng vào và điểm ra theo tên trong Hierarchy
        // Yêu cầu đặt tên đúng: "Enter portal 1" chứa "Exit point 1"
        enterPortal_1 = transform.Find("Enter portal 1");
        exitPoint_1 = enterPortal_1?.Find("Exit point 1");

        // Tương tự cho cổng số 2
        enterPortal_2 = transform.Find("Enter portal 2");
        exitPoint_2 = enterPortal_2?.Find("Exit point 2");

        // Nếu có điểm ra hợp lệ → gắn script Teleporter cho cổng vào tương ứng
        if (exitPoint_1 != null)
            enterPortal_1.gameObject.AddComponent<Teleporter>();
        if (exitPoint_2 != null)
            enterPortal_2.gameObject.AddComponent<Teleporter>();
    }
}
