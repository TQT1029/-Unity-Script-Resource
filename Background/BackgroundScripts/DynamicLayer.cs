using UnityEngine;

public class DynamicLayer : MonoBehaviour
{
    [SerializeField] private ParallaxBackground parallaxRef;

    [SerializeField] private Vector2 scrollSpeed; // Tốc độ cuộn texture
    [SerializeField] private Renderer layerRenderer;
    [SerializeField] private Vector2 textureOffset; // Offset UV hiện tại

    private int layerIndex;

    [Range(0f, 2f), SerializeField] private float maxVerticalOffset = 0.5f;
    [SerializeField] private float smoothness = 1f; // Hệ số làm mượt di chuyển
    [SerializeField] private float verticalDamping;

    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 prevPosition;

    [SerializeField] private Rigidbody2D targetRigidbody; // Đối tượng dùng để điều khiển parallax

    private void Awake()
    {
        initialPosition = prevPosition = transform.localPosition;

        // Lưu giới hạn Y tối đa từ vị trí ban đầu
        maxVerticalOffset = Mathf.Abs(transform.localPosition.y);
    }

    private void Start()
    {
        parallaxRef = transform.parent.GetComponent<ParallaxBackground>();

        try
        {
            // Có thể thay "Ball" bằng tên player object khác
            targetRigidbody = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        }
        catch { }

        if (parallaxRef != null)
        {
            // Lấy index layer từ ký tự cuối của tên (ví dụ: "Layer-2" → 2)
            layerIndex = transform.name[^1] - '0';

            scrollSpeed = parallaxRef.layerSpeedVectors[layerIndex];
            verticalDamping = scrollSpeed.y;
            layerRenderer = parallaxRef.layerRenderers[layerIndex];
        }
        else
        {
            // Nếu không tìm thấy ParallaxBackground → dùng mặc định
            scrollSpeed = new Vector2(0.1f, 0f);
            layerRenderer = GetComponent<Renderer>();
            layerRenderer.material = Instantiate(layerRenderer.material);
        }
    }

    private void LateUpdate()
    {
        prevPosition = transform.localPosition;
    }

    private void Update()
    {
        if (targetRigidbody != null)
        {
            // Cuộn ngang dựa trên vận tốc target
            textureOffset.x += (targetRigidbody.velocity.x / 10f) * scrollSpeed.x * Time.deltaTime;

            // Tính hệ số mượt cho di chuyển theo Y
            float smoothFactor = 1f - Mathf.Exp(-smoothness * Time.deltaTime);

            // Di chuyển Y ngược hướng với target để tạo hiệu ứng parallax
            float yOffset = -targetRigidbody.velocity.y / 10 * verticalDamping;
            Vector3 targetPos = prevPosition + new Vector3(0f, yOffset, 0f);

            // Giữ layer trong giới hạn maxVerticalOffset
            if (-maxVerticalOffset <= targetPos.y && targetPos.y <= maxVerticalOffset && maxVerticalOffset != 0)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothFactor);
            }
            else if (maxVerticalOffset != 0)
            {
                targetPos = prevPosition + new Vector3(0f, -yOffset, 0f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothFactor);
            }
        }
        else
        {
            // Nếu không có Rigidbody điều khiển → cuộn đều
            textureOffset += scrollSpeed * Time.deltaTime;
        }

        // Cập nhật offset cho texture để tạo hiệu ứng cuộn
        layerRenderer.material.mainTextureOffset = textureOffset;
    }
}
