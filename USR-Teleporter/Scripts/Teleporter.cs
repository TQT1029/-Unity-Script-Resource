using UnityEngine;

/// <summary>
/// Gắn lên cổng vào (Enter Portal).
/// Khi Player chạm vào collider của cổng, sẽ dịch chuyển đến Exit Point tương ứng.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Teleporter : MonoBehaviour
{
    private TeleporterManager portalManager;

    private void Awake()
    {
        // Lấy TeleporterManager từ parent của portal
        portalManager = transform.parent.GetComponent<TeleporterManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Chỉ dịch chuyển nếu va chạm với Player (tag phải đúng "Player")
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra tên object để xác định cổng
            // → Nếu tên kết thúc bằng '1' → dùng exitPoint_1
            // → Nếu tên kết thúc bằng '2' → dùng exitPoint_2
            if (transform.name.EndsWith("1"))
                collision.transform.position = portalManager.exitPoint_1.position;
            else if (transform.name.EndsWith("2"))
                collision.transform.position = portalManager.exitPoint_2.position;
        }
    }
}
