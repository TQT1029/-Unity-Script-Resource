using UnityEngine;

public class CreateObjectsLine : MonoBehaviour
{
    [Header("Config")]
    [SerializeField, Min(1)] private int spawnCount = 5;// Số lượng object muốn sinh ra, tối thiểu 1
    [SerializeField] private GameObject originalObject; // Prefab hoặc object mẫu để nhân bản
    [SerializeField] private Transform objectStorage;
    // Nơi chứa các object spawn ra (để quản lý gọn trong Hierarchy)

    private Transform startPos;// Điểm bắt đầu (dùng làm mốc Lerp)
    private Transform endPos;// Điểm kết thúc (dùng làm mốc Lerp)

    private void Start()
    {
        // Tìm prefab gốc theo tên → Có thể thay bằng gán trực tiếp trong Inspector để tái sử dụng
        if (originalObject == null)
            originalObject = transform.parent.Find("Original Coin").gameObject;
        // Tìm nơi chứa object spawn ra
        if (objectStorage == null)
            objectStorage = transform.Find("Object Storage");

        if (originalObject == null || objectStorage == null) return;

        // Lấy mốc start và end để spawn object
        startPos = transform.Find("Start Point");
        endPos = transform.Find("End Point");

        // Sinh ra các object giữa Start và End
        for (int i = 0; i < spawnCount; i++)
        {
            // Vector2.Lerp để chia đều vị trí giữa startPoint và endPoint
            Vector2 spawnPoint = Vector2.Lerp(startPos.position, endPos.position, (float)i / (spawnCount - 1));

            // Tạo object mới tại vị trí spawnPosition, không xoay (Quaternion.identity)
            GameObject clonedObject = Instantiate(originalObject, spawnPoint, Quaternion.identity);

            // Đặt tên gọn gàng: "Clone <Tên gốc> <Số thứ tự>"
            // Kiểm tra nếu tên gốc có tiền tố "Original " thì cắt bỏ
            string baseName = originalObject.name.StartsWith("Original ")
                ? originalObject.name.Substring(9)
                : originalObject.name;

            clonedObject.transform.name = $"Cloned {baseName} {i + 1}";

            // Đưa vào parentContainer để quản lý
            clonedObject.transform.SetParent(objectStorage, true);// true: giữ nguyên vị trí thế giới khi đổi parent
        }
    }
}

