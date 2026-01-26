using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DiceThrower : MonoBehaviour
{
    [SerializeField] private Dice dice;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0f, 2f, 0f);

    [Header("Dice Settings")]
    [SerializeField] private int faceUp = 1;
    [SerializeField] private int faceDown = 6;
    [SerializeField] private int faceRight = 3;
    [SerializeField] private int faceLeft = 4;
    [SerializeField] private int faceForward = 2;
    [SerializeField] private int faceBack = 5;

    [SerializeField] private bool createFaceTextOnAwake = true;
    [SerializeField] private bool refreshFaceTextOnValidate = true;
    [SerializeField] private bool flipFaceText = true;
    [SerializeField] private float faceTextSurfaceOffset = 0.01f;
    [SerializeField] private float faceTextFontSize = 6f;
    [SerializeField] private Vector2 faceTextRectSize = new Vector2(1f, 1f);
    [SerializeField] private Color faceTextColor = Color.black;

    [SerializeField] private float linearSpeedThreshold = 0.05f;
    [SerializeField] private float angularSpeedThreshold = 0.1f;
    [SerializeField] private float settleTime = 0.5f;
    [SerializeField] private bool logResult = true;

    [Header("Auto Create")]
    [SerializeField] private bool autoCreatePlaneAndDiceIfMissing = true;
    [SerializeField] private Vector3 planePosition = Vector3.zero;
    [SerializeField] private Vector3 planeScale = new Vector3(5f, 1f, 5f);
    [SerializeField] private Color planeColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color diceColor = new Color(0.9f, 0.2f, 0.2f, 1f);

    [Header("Throw")]
    [SerializeField] private Vector3 initialVelocity = new Vector3(1.5f, 0f, 0.5f);
    [SerializeField] private Vector3 initialAngularVelocity = new Vector3(12f, 20f, 8f);
    [SerializeField] private bool randomizeRotation = true;
    [SerializeField] private bool randomizeAngularVelocity = true;
    [SerializeField] private float randomAngularSpeed = 25f;

    [Header("Camera Cinematic")]
    [SerializeField] private bool playCameraCinematicOnThrow = true;
    [SerializeField] private Camera cinematicCamera;
    [SerializeField] private bool useMainCameraIfNull = true;
    [SerializeField] private Transform cameraObservationPoint;
    [SerializeField] private bool autoCreateObservationPointIfNull = true;
    [SerializeField] private Vector3 defaultObservationOffset = new Vector3(1.2f, 2f, -4.5f);
    [SerializeField] private bool snapCameraToObservationPointOnStart = true;
    [SerializeField] private bool snapToObservationPointBeforeCinematic = true;
    [SerializeField] private bool useObservationPointFovIfAvailable = true;
    [SerializeField] private float cinematicDuration = 1.2f;
    [SerializeField] private float orbitRevolutions = 1.5f;
    [SerializeField] private float startRadius = 6f;
    [SerializeField] private float endRadius = 3f;
    [SerializeField] private float startHeight = 3f;
    [SerializeField] private float endHeight = 1.5f;
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 0.3f, 0f);
    [SerializeField] private AnimationCurve cinematicEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool animateFov = true;
    [SerializeField] private float startFov = 60f;
    [SerializeField] private float endFov = 40f;
    [SerializeField] private float shakeAmplitude = 0.15f;
    [SerializeField] private float shakeFrequency = 10f;
    [SerializeField] private bool restoreCameraAfterCinematic = true;
    [SerializeField] private float restoreDuration = 0.25f;

    [Header("Controls")]
    [SerializeField] private KeyCode throwKey = KeyCode.Space;
    [SerializeField] private KeyCode resetAndThrowKey = KeyCode.R;

    private Rigidbody rb;

    private Coroutine cinematicRoutine;
    private Vector3 savedCamPos;
    private Quaternion savedCamRot;
    private float savedCamFov;
    private bool hasSavedCamera;

    private void Awake()
    {
        if (autoCreatePlaneAndDiceIfMissing)
        {
            EnsureDemoObjects();
        }

        if (dice == null)
        {
            dice = FindFirstObjectByType<Dice>();
        }

        if (dice != null)
        {
            ApplyDiceSettings(ensureAndRefreshFaceText: true);
            rb = dice.GetComponent<Rigidbody>();
        }

        if (autoCreateObservationPointIfNull)
        {
            EnsureObservationPoint();
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && dice != null)
        {
            ApplyDiceSettings(ensureAndRefreshFaceText: false);
        }
    }

    [ContextMenu("Create Camera Observation Point")]
    private void CreateCameraObservationPointContextMenu()
    {
        EnsureObservationPoint(forceRecreate: true);
    }

    [ContextMenu("Play Camera Cinematic")]
    private void PlayCameraCinematicContextMenu()
    {
        BeginCameraCinematic();
    }

    [ContextMenu("Create Demo Plane & Dice")]
    private void CreateDemoPlaneAndDice()
    {
        EnsureDemoObjects();

        if (dice != null)
        {
            ApplyDiceSettings(ensureAndRefreshFaceText: true);
            rb = dice.GetComponent<Rigidbody>();
        }
    }

    [ContextMenu("Apply Dice Settings")]
    private void ApplyDiceSettingsContextMenu()
    {
        if (dice == null)
        {
            return;
        }

        ApplyDiceSettings(ensureAndRefreshFaceText: true);
    }

    private void ApplyDiceSettings(bool ensureAndRefreshFaceText)
    {
        if (dice == null)
        {
            return;
        }

        dice.SetFaceValues(faceUp, faceDown, faceRight, faceLeft, faceForward, faceBack);
        dice.SetSettleDetectionSettings(linearSpeedThreshold, angularSpeedThreshold, settleTime, logResult);
        dice.SetFaceTextSettings(
            createFaceTextOnAwake,
            refreshFaceTextOnValidate,
            flipFaceText,
            faceTextSurfaceOffset,
            faceTextFontSize,
            faceTextRectSize,
            faceTextColor,
            ensureAndRefreshNow: ensureAndRefreshFaceText);
    }

    private void EnsureDemoObjects()
    {
        EnsurePlane();
        EnsureDice();
    }

    private void EnsureObservationPoint(bool forceRecreate = false)
    {
        if (cameraObservationPoint != null && !forceRecreate)
        {
            return;
        }

        Transform existing = null;
        GameObject found = GameObject.Find("DiceCameraObservationPoint");
        if (found != null)
        {
            existing = found.transform;
        }

        if (existing != null && !forceRecreate)
        {
            cameraObservationPoint = existing;
            return;
        }

        Vector3 target = dice != null ? (dice.transform.position + lookAtOffset) : (GetSpawnPosition() + lookAtOffset);

        GameObject go = new GameObject("DiceCameraObservationPoint");
        go.transform.position = target + defaultObservationOffset;
        go.transform.LookAt(target);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.RegisterCreatedObjectUndo(go, "Create Camera Observation Point");
        }
#endif

        cameraObservationPoint = go.transform;
    }

    private void EnsurePlane()
    {
        GameObject existing = GameObject.Find("DiceDemoPlane");
        if (existing != null)
        {
            ApplyColorIfPossible(existing, planeColor);
            return;
        }

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "DiceDemoPlane";
        plane.transform.position = planePosition;
        plane.transform.localScale = planeScale;
        ApplyColorIfPossible(plane, planeColor);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.RegisterCreatedObjectUndo(plane, "Create DiceDemoPlane");
        }
#endif
    }

    private static void ApplyColorIfPossible(GameObject go, Color color)
    {
        if (go == null)
        {
            return;
        }

        if (!go.TryGetComponent(out Renderer r) || r == null)
        {
            return;
        }

        if (r.material != null)
        {
            r.material.color = color;
        }
    }

    private void EnsureDice()
    {
        if (dice != null)
        {
            ApplyColorIfPossible(dice.gameObject, diceColor);
            ApplyDiceSettings(ensureAndRefreshFaceText: true);
            return;
        }

        GameObject existing = GameObject.Find("Dice");
        if (existing != null && existing.TryGetComponent(out Dice existingDice))
        {
            dice = existingDice;
            ApplyColorIfPossible(existing, diceColor);
            ApplyDiceSettings(ensureAndRefreshFaceText: true);
            return;
        }

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Dice";
        cube.transform.position = GetSpawnPosition();
        cube.transform.rotation = randomizeRotation ? Random.rotation : Quaternion.identity;
        dice = cube.AddComponent<Dice>();
        ApplyColorIfPossible(cube, diceColor);
        ApplyDiceSettings(ensureAndRefreshFaceText: true);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.RegisterCreatedObjectUndo(cube, "Create Dice");
        }
#endif
    }

    private void Start()
    {
        if (dice != null)
        {
            ResetAndThrow();
        }

        if (snapCameraToObservationPointOnStart)
        {
            Camera cam = GetCinematicCamera();
            if (cam != null && cameraObservationPoint != null)
            {
                cam.transform.position = cameraObservationPoint.position;
                cam.transform.rotation = cameraObservationPoint.rotation;

                if (useObservationPointFovIfAvailable && cameraObservationPoint.TryGetComponent(out Camera obsCam) && obsCam != null)
                {
                    cam.fieldOfView = obsCam.fieldOfView;
                }
            }
        }
    }

    private void Update()
    {
        if (dice == null || rb == null)
        {
            return;
        }

        if (Input.GetKeyDown(throwKey))
        {
            Throw();
        }

        if (Input.GetKeyDown(resetAndThrowKey))
        {
            ResetAndThrow();
        }
    }

    private Vector3 GetSpawnPosition()
    {
        return spawnPoint != null ? spawnPoint.position : spawnPosition;
    }

    private Camera GetCinematicCamera()
    {
        if (cinematicCamera != null)
        {
            return cinematicCamera;
        }

        if (useMainCameraIfNull)
        {
            return Camera.main;
        }

        return null;
    }

    private void BeginCameraCinematic()
    {
        if (!playCameraCinematicOnThrow)
        {
            return;
        }

        if (dice == null)
        {
            return;
        }

        Camera cam = GetCinematicCamera();
        if (cam == null)
        {
            return;
        }

        if (cinematicRoutine != null)
        {
            StopCoroutine(cinematicRoutine);
            cinematicRoutine = null;
        }

        if (cameraObservationPoint != null)
        {
            savedCamPos = cameraObservationPoint.position;
            savedCamRot = cameraObservationPoint.rotation;
            savedCamFov = cam.fieldOfView;

            if (useObservationPointFovIfAvailable && cameraObservationPoint.TryGetComponent(out Camera obsCam) && obsCam != null)
            {
                savedCamFov = obsCam.fieldOfView;
            }

            hasSavedCamera = true;
        }
        else if (!hasSavedCamera)
        {
            savedCamPos = cam.transform.position;
            savedCamRot = cam.transform.rotation;
            savedCamFov = cam.fieldOfView;
            hasSavedCamera = true;
        }

        if (snapToObservationPointBeforeCinematic && cameraObservationPoint != null)
        {
            cam.transform.position = savedCamPos;
            cam.transform.rotation = savedCamRot;
            cam.fieldOfView = savedCamFov;
        }

        cinematicRoutine = StartCoroutine(CameraCinematicRoutine(cam));
    }

    private IEnumerator CameraCinematicRoutine(Camera cam)
    {
        float duration = Mathf.Max(0.01f, cinematicDuration);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            if (dice == null)
            {
                break;
            }

            float u = Mathf.Clamp01(t / duration);
            float eased = cinematicEase != null ? cinematicEase.Evaluate(u) : u;

            Vector3 center = dice.transform.position + lookAtOffset;
            float radius = Mathf.Lerp(startRadius, endRadius, eased);
            float height = Mathf.Lerp(startHeight, endHeight, eased);

            float angle = u * orbitRevolutions * 360f;
            Vector3 orbitOffset = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, -radius);

            float n1 = Mathf.PerlinNoise(Time.time * shakeFrequency, 0.123f) - 0.5f;
            float n2 = Mathf.PerlinNoise(0.456f, Time.time * shakeFrequency) - 0.5f;
            float n3 = Mathf.PerlinNoise(Time.time * shakeFrequency, Time.time * shakeFrequency) - 0.5f;
            Vector3 shake = new Vector3(n1, n2, n3) * shakeAmplitude;

            cam.transform.position = center + orbitOffset + Vector3.up * height + shake;
            cam.transform.LookAt(center);

            if (animateFov)
            {
                cam.fieldOfView = Mathf.Lerp(startFov, endFov, eased);
            }

            yield return null;
        }

        if (restoreCameraAfterCinematic)
        {
            yield return RestoreCameraRoutine(cam);
        }

        cinematicRoutine = null;
    }

    private IEnumerator RestoreCameraRoutine(Camera cam)
    {
        if (!hasSavedCamera || cam == null)
        {
            yield break;
        }

        float duration = Mathf.Max(0.01f, restoreDuration);

        Vector3 startPosLocal = cam.transform.position;
        Quaternion startRotLocal = cam.transform.rotation;
        float startFovLocal = cam.fieldOfView;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float u = Mathf.Clamp01(t / duration);
            float eased = cinematicEase != null ? cinematicEase.Evaluate(u) : u;

            cam.transform.position = Vector3.Lerp(startPosLocal, savedCamPos, eased);
            cam.transform.rotation = Quaternion.Slerp(startRotLocal, savedCamRot, eased);
            cam.fieldOfView = Mathf.Lerp(startFovLocal, savedCamFov, eased);
            yield return null;
        }

        cam.transform.position = savedCamPos;
        cam.transform.rotation = savedCamRot;
        cam.fieldOfView = savedCamFov;
    }

    public void ResetAndThrow()
    {
        if (dice == null || rb == null)
        {
            return;
        }

        dice.ResetResult();

        dice.transform.position = GetSpawnPosition();
        if (randomizeRotation)
        {
            dice.transform.rotation = Random.rotation;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.WakeUp();

        Throw();
    }

    public void Throw()
    {
        if (dice == null || rb == null)
        {
            return;
        }

        dice.ResetResult();

        BeginCameraCinematic();

        rb.velocity = initialVelocity;

        if (randomizeAngularVelocity)
        {
            float speed = Mathf.Max(0f, randomAngularSpeed);
            rb.angularVelocity = speed > 0f ? Random.onUnitSphere * speed : initialAngularVelocity;
        }
        else
        {
            rb.angularVelocity = initialAngularVelocity;
        }

        rb.WakeUp();
    }
}
