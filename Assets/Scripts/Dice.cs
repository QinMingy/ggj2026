using UnityEngine;

using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Dice : MonoBehaviour
{
    [Header("Face Values")]
    [SerializeField] private int up = 1;
    [SerializeField] private int down = 6;
    [SerializeField] private int right = 3;
    [SerializeField] private int left = 4;
    [SerializeField] private int forward = 2;
    [SerializeField] private int back = 5;

    [Header("Face Text (TMP)")]
    [SerializeField] private bool createFaceTextOnAwake = true;
    [SerializeField] private bool refreshFaceTextOnValidate = true;
    [SerializeField] private bool flipFaceText = true;
    [SerializeField] private float faceTextSurfaceOffset = 0.01f;
    [SerializeField] private float faceTextFontSize = 6f;
    [SerializeField] private Vector2 faceTextRectSize = new Vector2(1f, 1f);
    [SerializeField] private Color faceTextColor = Color.black;

    [Header("Settle Detection")]
    [SerializeField] private float linearSpeedThreshold = 0.05f;
    [SerializeField] private float angularSpeedThreshold = 0.1f;
    [SerializeField] private float settleTime = 0.5f;
    [SerializeField] private bool logResult = true;

    private Rigidbody rb;
    private float stillTime;
    private bool hasLogged;
    private int lastValue;

    private TextMeshPro[] faceText;

    public int CurrentValue => lastValue;

    public void SetFaceValues(int up, int down, int right, int left, int forward, int back)
    {
        this.up = up;
        this.down = down;
        this.right = right;
        this.left = left;
        this.forward = forward;
        this.back = back;

        RefreshFaceText();
    }

    public void SetFaceTextSettings(
        bool createFaceTextOnAwake,
        bool refreshFaceTextOnValidate,
        bool flipFaceText,
        float faceTextSurfaceOffset,
        float faceTextFontSize,
        Vector2 faceTextRectSize,
        Color faceTextColor,
        bool ensureAndRefreshNow)
    {
        this.createFaceTextOnAwake = createFaceTextOnAwake;
        this.refreshFaceTextOnValidate = refreshFaceTextOnValidate;
        this.flipFaceText = flipFaceText;
        this.faceTextSurfaceOffset = faceTextSurfaceOffset;
        this.faceTextFontSize = faceTextFontSize;
        this.faceTextRectSize = faceTextRectSize;
        this.faceTextColor = faceTextColor;

        if (ensureAndRefreshNow)
        {
            EnsureAndRefreshFaceText();
        }
        else
        {
            RefreshFaceText();
        }
    }

    public void SetSettleDetectionSettings(
        float linearSpeedThreshold,
        float angularSpeedThreshold,
        float settleTime,
        bool logResult)
    {
        this.linearSpeedThreshold = linearSpeedThreshold;
        this.angularSpeedThreshold = angularSpeedThreshold;
        this.settleTime = settleTime;
        this.logResult = logResult;
    }

    private void Awake()
    {
        if (!TryGetComponent(out BoxCollider _))
        {
            gameObject.AddComponent<BoxCollider>();
        }

        if (!TryGetComponent(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        ResetResult();

        if (createFaceTextOnAwake)
        {
            EnsureAndRefreshFaceText();
        }
    }

    private void OnValidate()
    {
        if (!refreshFaceTextOnValidate)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            EnsureAndRefreshFaceText();
        }
    }

    [ContextMenu("Create/Refresh Face Text (TMP)")]
    public void EnsureAndRefreshFaceText()
    {
        EnsureFaceTextObjects();
        RefreshFaceText();
    }

    private void EnsureFaceTextObjects()
    {
        if (faceText == null || faceText.Length != 6)
        {
            faceText = new TextMeshPro[6];
        }

        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
        {
            return;
        }

        Vector3 extents = box.size * 0.5f;

        EnsureFaceText(0, "FaceText_Up", new Vector3(0f, extents.y + faceTextSurfaceOffset, 0f), Quaternion.Euler(-90f, 0f, 0f));
        EnsureFaceText(1, "FaceText_Down", new Vector3(0f, -extents.y - faceTextSurfaceOffset, 0f), Quaternion.Euler(90f, 0f, 0f));
        EnsureFaceText(2, "FaceText_Right", new Vector3(extents.x + faceTextSurfaceOffset, 0f, 0f), Quaternion.Euler(0f, 90f, 0f));
        EnsureFaceText(3, "FaceText_Left", new Vector3(-extents.x - faceTextSurfaceOffset, 0f, 0f), Quaternion.Euler(0f, -90f, 0f));
        EnsureFaceText(4, "FaceText_Forward", new Vector3(0f, 0f, extents.z + faceTextSurfaceOffset), Quaternion.Euler(0f, 0f, 0f));
        EnsureFaceText(5, "FaceText_Back", new Vector3(0f, 0f, -extents.z - faceTextSurfaceOffset), Quaternion.Euler(0f, 180f, 0f));
    }

    private void EnsureFaceText(int index, string name, Vector3 localPosition, Quaternion localRotation)
    {
        Transform t = transform.Find(name);
        GameObject go;

        if (t == null)
        {
            go = new GameObject(name);
            go.transform.SetParent(transform, false);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.RegisterCreatedObjectUndo(go, "Create Dice Face Text");
            }
#endif
        }
        else
        {
            go = t.gameObject;
        }

        go.transform.localPosition = localPosition;
        go.transform.localRotation = flipFaceText ? localRotation * Quaternion.Euler(0f, 0f, 180f) : localRotation;
        go.transform.localScale = Vector3.one;

        if (!go.TryGetComponent(out TextMeshPro tmp))
        {
            tmp = go.AddComponent<TextMeshPro>();
        }

        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.fontSize = faceTextFontSize;
        tmp.color = faceTextColor;
        tmp.rectTransform.sizeDelta = faceTextRectSize;

        faceText[index] = tmp;
    }

    private void RefreshFaceText()
    {
        if (faceText == null || faceText.Length != 6)
        {
            return;
        }

        if (faceText[0] != null) faceText[0].text = up.ToString();
        if (faceText[1] != null) faceText[1].text = down.ToString();
        if (faceText[2] != null) faceText[2].text = right.ToString();
        if (faceText[3] != null) faceText[3].text = left.ToString();
        if (faceText[4] != null) faceText[4].text = forward.ToString();
        if (faceText[5] != null) faceText[5].text = back.ToString();
    }

    public void ResetResult()
    {
        stillTime = 0f;
        hasLogged = false;
        lastValue = 0;
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        bool isStill = rb.IsSleeping() ||
                      (rb.velocity.sqrMagnitude <= linearSpeedThreshold * linearSpeedThreshold &&
                       rb.angularVelocity.sqrMagnitude <= angularSpeedThreshold * angularSpeedThreshold);

        if (!isStill)
        {
            stillTime = 0f;
            return;
        }

        stillTime += Time.fixedDeltaTime;

        if (hasLogged || stillTime < settleTime)
        {
            return;
        }

        lastValue = GetTopFaceValue();
        hasLogged = true;

        if (logResult)
        {
            Debug.Log($"Dice result: {lastValue}", this);
        }
    }

    public int GetTopFaceValue()
    {
        Vector3 worldUp = Vector3.up;

        float dUp = Vector3.Dot(transform.up, worldUp);
        float dDown = Vector3.Dot(-transform.up, worldUp);
        float dRight = Vector3.Dot(transform.right, worldUp);
        float dLeft = Vector3.Dot(-transform.right, worldUp);
        float dForward = Vector3.Dot(transform.forward, worldUp);
        float dBack = Vector3.Dot(-transform.forward, worldUp);

        float max = dUp;
        int value = up;

        if (dDown > max)
        {
            max = dDown;
            value = down;
        }

        if (dRight > max)
        {
            max = dRight;
            value = right;
        }

        if (dLeft > max)
        {
            max = dLeft;
            value = left;
        }

        if (dForward > max)
        {
            max = dForward;
            value = forward;
        }

        if (dBack > max)
        {
            value = back;
        }

        return value;
    }
}
