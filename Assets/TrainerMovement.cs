using UnityEngine;

public class TrainerMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;          // 移動速度
    public float maxMoveDistance = 20f;   // 距離起點最大可移動範圍

    [Header("第一人稱視角設定")]
    public Transform playerCamera;        // 拖入 Main Camera
    public float mouseSensitivity = 200f; // 滑鼠靈敏度（覺得太快或太慢可調這裡）

    private Vector3 startPosition;
    private float xRotation = 0f;         // 紀錄上下抬頭低頭的角度

    void Start()
    {
        startPosition = transform.position;

        // 若忘記手動綁定，自動抓取 Main Camera
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        // 進入遊戲時將滑鼠游標鎖定在畫面中央並隱藏
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- 1. 滑鼠環顧四周 ---
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // 計算上下視角並限制在 -80 度到 80 度之間（避免脖子折斷翻轉）
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            if (playerCamera != null)
            {
                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }

            // 訓練家身體跟著滑鼠左右旋轉
            transform.Rotate(Vector3.up * mouseX);
        }

        // --- 2. 鍵盤 WASD 前後左右移動 ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forwardDir = transform.forward;
        forwardDir.y = 0f;
        forwardDir.Normalize();

        Vector3 rightDir = transform.right;
        rightDir.y = 0f;
        rightDir.Normalize();

        Vector3 moveDirection = Vector3.ClampMagnitude(forwardDir * moveZ + rightDir * moveX, 1f);
        Vector3 nextPos = transform.position + moveDirection * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(startPosition, nextPos) <= maxMoveDistance)
        {
            transform.position = nextPos;
        }

        // --- 3. 游標鎖定與解鎖切換（方便結束測試或錄影） ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}