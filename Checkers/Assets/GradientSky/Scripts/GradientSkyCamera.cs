// Use this script on a Camera create a fixed size gradient mesh that alwyas follow the camera and renders as a background.

using UnityEngine;

// Use a unique namespace to ensure there are no name conflicts
namespace Imphenzia
{
    // Execute in edit mode so gradient is updated if the color gradient is changed
    [ExecuteInEditMode]

    // This class inherits from GradientSkyCommon.cs where there is common and reusable code
    public class GradientSkyCamera : GradientSkyCommon
    {
        // Allows to select if the gradient should be positioned at Near or Far clipping plane - it has no visual difference, only in the scene window
        public enum ClippingPlane { NEAR, FAR }
        public ClippingPlane placeAtClippingPlane = ClippingPlane.NEAR;

        // Reference to the child gameobject that has the mesh - this is not to pollute the Camera object with mesh related components.
        // Hide in inspector - we don't need to see it there (it can't be set to private/protected since those are not serialzied and
        // kept between play/edit mode).
        [HideInInspector]
        public GameObject childObject;

        // Private variables to reference camera, camera cached settings, and clipping plane
        private Camera _camera;
        private float _cacheFieldOfView;
        private int _cacheCameraWidth;
        private int _cacheCameraHeight;
        private ClippingPlane _cacheClippingPlane = ClippingPlane.NEAR;

        /// <summary>
        /// Destroys the gradient child object if one exists if the component is disabled.
        /// </summary>
        void OnDisable()
        {
            DestroyChildObjectIfExists();
        }

        /// <summary>
        /// Creates the gradient child object if the component is enabled
        /// </summary>
        void OnEnable()
        {
            CreateOrGetChildObject();
        }

        /// <summary>
        /// Reset is called when a component is added or when reset is pressed for the object in the inspector.    
        /// </summary>
        void Reset()
        {
            // If the component is not used on a camera- throw error and abort
            if (GetComponent<Camera>() == null)
            {
                Debug.LogError("GameObject must have a camera component - aborting.");
                return;
            }
            
            _camera = GetComponent<Camera>();

            // Unity 5.x renders the skybox so you need to change Camera Clear flag to something else, like SolidColor
            if (_camera.clearFlags == CameraClearFlags.Skybox)
            {
                Debug.LogWarning("You must set Camera Clear Flag in the inspector for the camera to something other than Skybox, use SolidColor for example, in" +
                    "Unity 5.x for the gradient sky to appear.");
            }

            // Create a default gradient
            CreateDefaultGradient();

            // Destroy the child object named CameraFixedGradientSky if one exists (we only want to have one)
            DestroyChildObjectIfExists();

            // Create the child object
            CreateOrGetChildObject();
        }

        /// <summary>
        /// Checks for the existence of a child gameobject named CameraFixedGradientSky and destroys it.
        /// </summary>
        void DestroyChildObjectIfExists()
        {
            if (transform.Find("CameraFixedGradientSky") != null)
                DestroyImmediate(transform.Find("CameraFixedGradientSky").gameObject);

        }

        /// <summary>
        /// Creates (or gets the) child gameobject
        /// </summary>
        void CreateOrGetChildObject()
        {
            // Ensure that the camera component is set if this gets called before reset
            _camera = GetComponent<Camera>();

            // If there is a child gameobject named CameraFixedGradientSky...
            if (transform.Find("CameraFixedGradientSky") != null)
            {
                // Set it as the childObject
                childObject = transform.Find("CameraFixedGradientSky").gameObject;
            }
            else
            {
                // Otherwise - create a new gameobject
                childObject = new GameObject();

                // Set the name of the object - this is used when the object is destroyed so the name is important
                childObject.name = "CameraFixedGradientSky";

                // Set the parent of the transform to the camera transform
                childObject.transform.parent = transform;

                // Reset the local rotation so it's "zero"
                childObject.transform.localRotation = Quaternion.identity;

                // Hide the child gameobject in the inspector - we don't need to see it there
                childObject.hideFlags = HideFlags.HideInHierarchy;

                // Place the gameobject just behind the near clipping plane or in front of the far clipping plane
                if (placeAtClippingPlane == ClippingPlane.NEAR)
                    childObject.transform.localPosition = new Vector3(0, 0, _camera.nearClipPlane + 0.01f);
                else
                    childObject.transform.localPosition = new Vector3(0, 0, _camera.farClipPlane - 0.01f);

                // Add MeshFilter and MeshRenderer components to the child gameobject
                childObject.AddComponent<MeshFilter>();
                childObject.AddComponent<MeshRenderer>();

                // Create a MeshRenderer for the childObject
                CreateMeshRenderer(childObject.transform);

                // Set the Material to the shader with ZWrite off and RenderQueue set to Gemetry-1000 to ensure it's always rendered as a background
                Material _material = new Material(Shader.Find("Custom/VertexColorCamera"));
                childObject.GetComponent<MeshRenderer>().sharedMaterial = _material;

            }

            // Set the cached clipping plane to the clipping plane used so we can compare if it's changed
            _cacheClippingPlane = placeAtClippingPlane;

            // Create the gradient mesh and sett it to the sharedMesh for the MeshFilter
            childObject.GetComponent<MeshFilter>().sharedMesh = CreateMesh();

            // Set the mesh local scale to ensure it fills the camera based on its position and camera field of view and size
            SetMeshLocalScale();
        }


        /// <summary>
        /// Check if the gradient values and cached gradient values are different - if they are, recreate the mesh to update the gradient.
        /// Checks if camera values and cached camera values are different - if they are, rescale the mesh to ensure it covers the entire screen.
        /// </summary>
        void Update()
        {

            // If the field of view has changed for the camera, the size of the mesh needs updating
            if (Mathf.Abs(_camera.fieldOfView - _cacheFieldOfView) > 0.01f ||
                _camera.pixelWidth != _cacheCameraWidth ||
                _camera.pixelHeight != _cacheCameraHeight)
                SetMeshLocalScale();


            if (cacheGradient == null)
            {
                childObject.GetComponent<MeshFilter>().sharedMesh = CreateMesh();
                return;
            }

            // If the number of color keys in the gradient and the cache gradient don't match...
            if (gradient.colorKeys.Length != cacheGradient.colorKeys.Length)
            {
                // Recreate the mesh with the correct number of keys (this also updates the cached gradient)
                childObject.GetComponent<MeshFilter>().sharedMesh = CreateMesh();
                // Return, we don't need to check if any other changes have been made this frame since the mesh is already recreated
                return;
            }

            // Iterate through the number of color keys...
            for (int _i = 0; _i < gradient.colorKeys.Length; _i++)
            {
                // If any color key in the gradient doesn't match the value of the cache...
                if (!IsColorKeyApproxEqual(gradient.colorKeys[_i], cacheGradient.colorKeys[_i]))
                {
                    // Recreate the mesh with the correct number of keys (this also updates the cached gradient)
                    childObject.GetComponent<MeshFilter>().sharedMesh = CreateMesh();

                    // Return, we don't need to check if any more values are different this frame since the mesh is already recreated
                    return;
                }
            }

            // If the clipping plane has changed, recreate the object and place it at the correct position
            if (placeAtClippingPlane != _cacheClippingPlane)
            {
                DestroyChildObjectIfExists();
                CreateOrGetChildObject();
            }

        }

        /// <summary>
        /// Sets the scale of the mesh based on camera properties to ensure it convers the entire viewport/screen and nothing more.
        /// </summary>
        void SetMeshLocalScale()
        {
            // Get a reference to the camera
            Camera _camera = GetComponent<Camera>();

            // ResetAspect needs to be called to ensure that aspect ratio is updated when changing between aspect ratios and between edit/play
            _camera.ResetAspect();

            // Calculate the furstrum height based on position and field of view
            float _frustumHeight = 2.0f * childObject.transform.localPosition.z * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // Set the local scale width/height
            childObject.transform.localScale = new Vector3(_frustumHeight * _camera.aspect, (_frustumHeight * _camera.aspect) / _camera.aspect, 0);

            // Update the cache for increased performance
            _cacheFieldOfView = _camera.fieldOfView;
            _cacheCameraWidth = _camera.pixelWidth;
            _cacheCameraHeight = _camera.pixelHeight;
        }

    }

}


