// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Class to visualize controller inputs
    /// </summary>
    [RequireComponent(typeof(ControllerConnectionHandler))]
    public class ControllerVisualizer : MonoBehaviour
    {
        #region Public Enum
        [System.Flags]
        public enum DeviceTypesAllowed : byte
        {
            Controller = 1,
            MobileApp = 2,
            All = Controller | MobileApp,
        }
        #endregion

        #region Private Variables
        [SerializeField, Tooltip("The controller model.")]
        private GameObject _controllerModel = null;

        [Header("Controller Parts"), Space]
        [SerializeField, Tooltip("The controller's trigger model.")]
        private GameObject _trigger = null;

        [SerializeField, Tooltip("The controller's touchpad model.")]
        private GameObject _touchpad = null;

        [SerializeField, Tooltip("The controller's home button model.")]
        private GameObject _homeButton = null;

        [SerializeField, Tooltip("The controller's bumper button model.")]
        private GameObject _bumperButton = null;

        // Color when the button state is idle.
        private Color _defaultColor = Color.white;
        // Color when the button state is active.
        private Color _activeColor = Color.grey;

        private Material _touchpadMaterial = null;
        private Material _triggerMaterial = null;
        private Material _homeButtonMaterial = null;
        private Material _bumperButtonMaterial = null;

        private float _touchpadRadius;

        private ControllerConnectionHandler _controllerConnectionHandler = null;
        private bool _wasControllerValid = true;

        private const float MAX_TRIGGER_ROTATION = 35.0f;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Initialize variables, callbacks and check null references.
        /// </summary>
        void Start()
        {
            _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();

            if (!_controllerModel)
            {
                Debug.LogError("Error: ControllerVisualizer._controllerModel is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_trigger)
            {
                Debug.LogError("Error: ControllerVisualizer._trigger is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_touchpad)
            {
                Debug.LogError("Error: ControllerVisualizer._touchpad is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_homeButton)
            {
                Debug.LogError("Error: ControllerVisualizer._homeButton is not set, disabling script.");
                enabled = false;
                return;
            }
            if (!_bumperButton)
            {
                Debug.LogError("Error: ControllerVisualizer._bumperButton is not set, disabling script.");
                enabled = false;
                return;
            }

            SetVisibility(_controllerConnectionHandler.IsControllerValid());

            _triggerMaterial = FindMaterial(_trigger);
            _touchpadMaterial = FindMaterial(_touchpad);
            _homeButtonMaterial = FindMaterial(_homeButton);
            _bumperButtonMaterial = FindMaterial(_bumperButton);

            // Calculate the radius of the touchpad's mesh
            Mesh mesh = _touchpad.GetComponent<MeshFilter>().mesh;
            _touchpadRadius = Vector3.Scale(mesh.bounds.extents, _touchpad.transform.lossyScale).x;
        }

        /// <summary>
        /// Update controller input based feedback.
        /// </summary>
        void Update()
        {
            UpdateTriggerVisuals();
            UpdateTouchpadIndicator();
            SetVisibility(_controllerConnectionHandler.IsControllerValid());
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Update the touchpad's indicator: (location, directions, color).
        /// Also updates the color of the touchpad, based on pressure.
        /// </summary>
        private void UpdateTouchpadIndicator()
        {
            if (!_controllerConnectionHandler.IsControllerValid())
            {
                return;
            }
            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            Vector3 updatePosition = new Vector3(controller.Touch1PosAndForce.x, 0.0f, controller.Touch1PosAndForce.y);

            float force = controller.Touch1PosAndForce.z;
            _touchpadMaterial.color = Color.Lerp(_defaultColor, _activeColor, force);
        }

        /// <summary>
        /// Update the rotation and visual color of the trigger.
        /// </summary>
        private void UpdateTriggerVisuals()
        {
            if (!_controllerConnectionHandler.IsControllerValid())
            {
                return;
            }
            MLInputController controller = _controllerConnectionHandler.ConnectedController;

            // Change the color of the trigger
            _triggerMaterial.color = Color.Lerp(_defaultColor, _activeColor, controller.TriggerValue);

            // Set the rotation of the trigger
            Vector3 eulerRot = _trigger.transform.localRotation.eulerAngles;
            eulerRot.x = Mathf.Lerp(0, MAX_TRIGGER_ROTATION, controller.TriggerValue);
            _trigger.transform.localRotation = Quaternion.Euler(eulerRot);
        }

        /// <summary>
        /// Attempt to get the Material of a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to search for a material.</param>
        /// <returns>Material of the GameObject, if it exists. Otherwise, null.</returns>
        private Material FindMaterial(GameObject gameObject)
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            return (renderer != null) ? renderer.material : null;
        }

        /// <summary>
        /// Set object visibility to value.
        /// </summary>
        /// <param name="value"> true or false to set visibility. </param>
        private void SetVisibility(bool value)
        {
            if (_wasControllerValid == value)
            {
                return;
            }

            Renderer[] rendererArray = _controllerModel.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in rendererArray)
            {
                r.enabled = value;
            }

            _wasControllerValid = value;
        }
        #endregion
    }
}
