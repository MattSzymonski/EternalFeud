using UnityEngine;
using MightyGamePack;

namespace MightyGamePack
{
    public class SimpleCameraController : MonoBehaviour
    {
        MightyGameManager gameManager;
        
        MovementState m_TargetCameraState = new MovementState();
        MovementState m_InterpolatingCameraState = new MovementState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));


        public AnimationCurve stickSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
        public float stickSensitivityMultiplier = 1;

        public float controllerMoveSpeedMultiplier = 1;

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertYMouse = false;

        public bool invertYController = false;

        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<MightyGameManager>();
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            if (Input.GetButton("Forward"))
            {
                direction += Vector3.forward;
            }
            if (Input.GetButton("Backward"))
            {
                direction += Vector3.back;
            }
            if (Input.GetButton("Left"))
            {
                direction += Vector3.left;
            }
            if (Input.GetButton("Right"))
            {
                direction += Vector3.right;
            }
            if (Input.GetButton("Down"))
            {
                direction += Vector3.down;
            }
            if (Input.GetButton("Up"))
            {
                direction += Vector3.up;
            }

            direction += Vector3.right * controllerMoveSpeedMultiplier * Input.GetAxis("Controller1 Left Stick Horizontal");
            direction += Vector3.forward * controllerMoveSpeedMultiplier * -Input.GetAxis("Controller1 Left Stick Vertical");
            direction += Vector3.up * controllerMoveSpeedMultiplier * Input.GetAxis("Controller1 Triggers");
            return direction;

        }
        
        void Update()
        {
            if (gameManager.gameState == GameState.Playing)
            {


                // Hide and lock cursor when right mouse button pressed
                if (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Controller1 Fire"))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }

                // Unlock and show cursor when right mouse button released
                if (Input.GetMouseButtonUp(1))
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                // Rotation
                if (Input.GetMouseButton(0) || Input.GetAxis("Controller1 Right Stick Horizontal") != 0 || Input.GetAxis("Controller1 Right Stick Vertical") != 0)
                {
                    var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertYMouse ? 1 : -1));

                    var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                    m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                    m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;


                    var stickDeflection = new Vector2(Input.GetAxis("Controller1 Right Stick Vertical"), Input.GetAxis("Controller1 Right Stick Horizontal") * (invertYController ? 1 : -1));
                    var stickSensitivityFactor = mouseSensitivityCurve.Evaluate(stickDeflection.magnitude);

                    m_TargetCameraState.yaw += stickDeflection.x * stickSensitivityFactor * stickSensitivityMultiplier;
                    m_TargetCameraState.pitch += stickDeflection.y * stickSensitivityFactor * stickSensitivityMultiplier;
                }

                // Translation
                var translation = GetInputTranslationDirection() * Time.deltaTime;

                // Speed up movement when shift key held
                if (Input.GetButton("Shift"))
                {
                    translation *= 10.0f;
                }

                // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
                boost += Input.mouseScrollDelta.y * 0.2f;
                translation *= Mathf.Pow(2.0f, boost);

                m_TargetCameraState.Translate(translation);

                // Framerate-independent interpolation
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
                var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
                var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

                m_InterpolatingCameraState.UpdateTransform(transform);
            }
        }
    }

}