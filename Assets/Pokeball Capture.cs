using System.Collections;
using UnityEngine;

public class PokeballCapture : MonoBehaviour
{
    [Header("角色與位置設定")]
    public Transform trainerHandPoint;     // 訓練家手部定位點 (HandPoint)
    public Transform targetPokemon;        // 目標寶可夢物件

    [Header("特效與投擲參數")]
    public GameObject captureEffectPrefab; // 收服粒子特效 Prefab
    public float throwDuration = 1.2f;     // 飛行總秒數
    public float arcHeight = 1.5f;         // 拋物線高度（第一人稱建議 1.2 ~ 2.0）
    public float spinSpeed = 720f;         // 精靈球旋轉速度

    private bool isThrowing = false;
    private bool isHolding = true;         // 是否正拿在訓練家手上
    private Vector3 initialBallPos;

    void Start()
    {
        isHolding = true;
        if (trainerHandPoint != null)
        {
            transform.position = trainerHandPoint.position;
        }
        initialBallPos = transform.position;
    }

    void Update()
    {
        // 還沒丟出時，讓精靈球隨時跟著訓練家的 HandPoint 移動
        if (isHolding && !isThrowing && trainerHandPoint != null)
        {
            transform.position = trainerHandPoint.position;
        }

        // 按下「空白鍵 (Space)」或「滑鼠左鍵」拋出精靈球
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isThrowing && isHolding)
        {
            if (targetPokemon != null && targetPokemon.gameObject.activeSelf)
            {
                StartCoroutine(ThrowAndCaptureRoutine());
            }
        }

        // 按下「R 鍵」重置精靈球回到訓練家目前的手部位置
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSceneState();
        }
    }

    IEnumerator ThrowAndCaptureRoutine()
    {
        isHolding = false; // 離開訓練家手部
        isThrowing = true;

        Vector3 startPos = trainerHandPoint != null ? trainerHandPoint.position : transform.position;
        Vector3 endPos = targetPokemon.position;
        float elapsedTime = 0f;

        while (elapsedTime < throwDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / throwDuration);

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = currentPos;

            transform.Rotate(Vector3.forward, -spinSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = endPos;

        // 產生收服特效並隱藏寶可夢
        if (captureEffectPrefab != null)
        {
            GameObject effect = Instantiate(captureEffectPrefab, endPos, Quaternion.identity);
            Destroy(effect, 2.0f);
        }

        targetPokemon.gameObject.SetActive(false);

        transform.position = new Vector3(endPos.x, 0.2f, endPos.z);
        transform.rotation = Quaternion.identity;

        isThrowing = false;
    }

    void ResetSceneState()
    {
        StopAllCoroutines();
        isThrowing = false;
        isHolding = true; // 恢復手持狀態

        transform.position = trainerHandPoint != null ? trainerHandPoint.position : initialBallPos;
        transform.rotation = Quaternion.identity;

        if (targetPokemon != null)
        {
            targetPokemon.gameObject.SetActive(true);
        }
    }
}