// Use this script on an empty gameobject to create a gradient mesh. Scale the object as necessary.
using UnityEngine;

// Use a unique namespace to ensure there are no name conflicts
namespace Imphenzia
{
    // Requrie MeshFilter and MeshRenderer components
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]

    // Execute in edit mode so gradient is updated if the color gradient is changed
    [ExecuteInEditMode]

    // This class inherits from GradientSkyCommon.cs where there is common and reusable code
    public class GradientSkyObject : GradientSkyCommon {

        // Keep a reference to the MeshFilter component for increased performance - but hide in Inspector since we don't need to see it
        // We keep it public instead of private so it becomes serialized and maintains persistence between edit/play mode.
        [HideInInspector]
        public MeshFilter meshFilter;

        /// <summary>
        /// Reset is called when a component is added or when reset is pressed for the object in the inspector.    
        /// </summary>
        void Reset()
        {
            // Create a default gradient (see GradientSkyCommon.cs)
            CreateDefaultGradient();

            // Get a reference for the MeshFilter component (for better performance since we need to check it in every Update()
            meshFilter = GetComponent<MeshFilter>();

            // Create a mesh and set it to become the mesh of the MeshFilter (see GradientSkyCommon.cs)
            meshFilter.sharedMesh = CreateMesh();

            // Create a MeshRenderer (see GradientSkyCommon.cs)
            CreateMeshRenderer();

            // Set the Material to the shader with without any Zwrite and RenderQueue tags
            Material _material = new Material(Shader.Find("Custom/VertexColorObject"));
            GetComponent<MeshRenderer>().sharedMaterial = _material;

            // Set an initial scale of the object so the sky becomes easily visible
            transform.localScale = new Vector3(20, 10, 1);
        }

        /// <summary>
        /// Check if the gradient values and cached gradient values are different - if they are, recreate the mesh to update the gradient.
        /// </summary>
        void Update()
        {
            // If the number of color keys in the gradient and the cache gradient don't match...
            if (gradient.colorKeys.Length != cacheGradient.colorKeys.Length)
            {
                // Recreate the mesh with the correct number of keys (this also updates the cached gradient)
                meshFilter.sharedMesh = CreateMesh();

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
                    meshFilter.sharedMesh = CreateMesh();

                    // Return, we don't need to check if any more values are different this frame since the mesh is already recreated
                    return;
                }
            }
        }
    }
}