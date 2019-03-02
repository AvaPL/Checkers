// Code that is common to both object and camera attached/fixed gradient skies.
// Both GradientSkyObject.cs and GradientSkyCamera.cs inherit from this class

using UnityEngine;


// Use a unique namespace to ensure there are no name conflicts
namespace Imphenzia
{
    public class GradientSkyCommon : MonoBehaviour
    {

        // Declare a public gradient - change this gradient in the inspector to change the gradient sky
        public Gradient gradient = null;

        // Declare a cached gradient to detect if any changes have been made so the mesh can be updated
        // Hide in inspector - we don't need to see it there (it can't be set to private/protected since those are not serialzied and
        // kept between play/edit mode).
        [HideInInspector]
        public Gradient cacheGradient;

        /// <summary>
        /// Method to procedurally create a flat mesh divided based on the number of gradient color keys
        /// </summary>
        /// <returns>Generated mesh</returns>
        protected virtual Mesh CreateMesh()
        {
            // If there is no default gradient, create it first
            if (gradient == null)
                CreateDefaultGradient();

            // Defined width/height variables for the mesh
            float _width = 1.0f;
            float _height = 1.0f;

            // Crete a new mesh
            Mesh _mesh = new Mesh();

            // Calculate number of vertices (number of color keys * 2)
            int _vertexCount = gradient.colorKeys.Length * 2;

            // Define arrays for vertices, normals, uvs, and colors - each vertex need all of this
            Vector3[] _vertices = new Vector3[_vertexCount];
            Vector3[] _normals = new Vector3[_vertexCount];
            Vector2[] _uvs = new Vector2[_vertexCount];
            Color[] _colors = new Color[_vertexCount];

            // iterate through the number of color keys in the gradient
            for (int _i = 0; _i < gradient.colorKeys.Length; _i++)
            {
                // Create a pair of horizontally aligned vertices and calculate the Y-position based on the color key time variable (position in the gradient)
                _vertices[_i * 2] = new Vector3(-_width / 2f, gradient.colorKeys[_i].time - (_height / 2f), 0);
                _vertices[(_i * 2) + 1] = new Vector3(_width / 2f, gradient.colorKeys[_i].time - (_height / 2f), 0);

                // Make sure the normals face backwards (this is so a default created object is aligned to show the gradient)
                _normals[_i * 2] = -Vector3.forward;
                _normals[(_i * 2) + 1] = -Vector3.forward;

                // Calculate the UV position, it's the same as the time value since mesh is 1.0 high
                _uvs[_i * 2] = new Vector3(0, gradient.colorKeys[_i].time);
                _uvs[(_i * 2) + 1] = new Vector3(1, gradient.colorKeys[_i].time);

                // Set the vertex colors based on the color key value
                _colors[_i * 2] = gradient.colorKeys[_i].color;
                _colors[(_i * 2) + 1] = gradient.colorKeys[_i].color;
            }

            // Send the vertex array to the mesh
            _mesh.vertices = _vertices;

            // Define array for triangles of the mesh, each triangle consists of three vertices and we do two triangles at a time (quad)
            int[] _triangles = new int[gradient.colorKeys.Length * 6];

            // Iterate to create the necessary number of triangles
            for (int _i = 0; _i < gradient.colorKeys.Length - 1; _i++)
            {
                // Triangle 1 = vertices _i * 2 + 0 -> 2 -> 1 -> 0
                _triangles[_i * 6] = (_i * 2) + 0;
                _triangles[(_i * 6) + 1] = (_i * 2) + 2;
                _triangles[(_i * 6) + 2] = (_i * 2) + 1;

                // Triangle 2 = vertices _i * 2 + 2 -> 3 -> 1 -> 2
                _triangles[(_i * 6) + 3] = (_i * 2) + 2;
                _triangles[(_i * 6) + 4] = (_i * 2) + 3;
                _triangles[(_i * 6) + 5] = (_i * 2) + 1;
            }

            // Set triangles, normals, UVs and colors of the mesh from the arrays
            _mesh.triangles = _triangles;
            _mesh.normals = _normals;
            _mesh.uv = _uvs;
            _mesh.colors = _colors;

            // Create a new cache gradient and copy the values from the actual gradient to the cache
            cacheGradient = new Gradient();
            GradientColorKey[] _gck = gradient.colorKeys;
            GradientAlphaKey[] _gak = gradient.alphaKeys;
            cacheGradient.SetKeys(_gck, _gak);

            // Return the mesh
            return _mesh;
        }

        /// <summary>
        /// Creates a default gradient - whiteish light blue -> blue -> blackish dark blue
        /// </summary>
        protected virtual void CreateDefaultGradient()
        {
            // Define arrays for the color and alpha keys
            GradientColorKey[] _gradientColorKeys;
            GradientAlphaKey[] _gradientAlphaKeys;

            // Create the new gradient
            gradient = new Gradient();

            // Create a new color key array of 3 values
            _gradientColorKeys = new GradientColorKey[3];

            // Whiteish Light Blue
            _gradientColorKeys[0].color = new Color(0.81640625f, 0.8984375f, 0.96484375f);
            _gradientColorKeys[0].time = 0f;

            // Blue
            _gradientColorKeys[1].color = new Color(0.36328125f, 0.703125f, 0.98828125f);
            _gradientColorKeys[1].time = 0.5f;

            // Blackish Dark Blue
            _gradientColorKeys[2].color = new Color(0.0078125f, 0.0078125f, 0.2421875f);
            _gradientColorKeys[2].time = 1f;

            // Ensure alpha value is always 1
            _gradientAlphaKeys = new GradientAlphaKey[2];
            _gradientAlphaKeys[0].alpha = 1;
            _gradientAlphaKeys[0].time = 0;
            _gradientAlphaKeys[1].alpha = 1;
            _gradientAlphaKeys[1].time = 1;

            // Set the keys for the cradient
            gradient.SetKeys(_gradientColorKeys, _gradientAlphaKeys);
        }

        /// <summary>
        /// Verify if color keys are approximately equal. We can't check if they are equal because they are floats so
        /// it may be a small fractional difference. This is used to determine if the values have changed upon which the 
        /// gradient mesh needs to be recreated with new vertex colors.
        /// </summary>
        /// <param name="_colorKey1"></param>
        /// <param name="_colorKey2"></param>
        /// <returns></returns>
        protected virtual bool IsColorKeyApproxEqual(GradientColorKey _colorKey1, GradientColorKey _colorKey2)
        {
            if (Mathf.Abs(_colorKey1.time - _colorKey2.time) > 0.01f) return false;
            if (Mathf.Abs(_colorKey1.color.r - _colorKey2.color.r) > 0.01f) return false;
            if (Mathf.Abs(_colorKey1.color.g - _colorKey2.color.g) > 0.01f) return false;
            if (Mathf.Abs(_colorKey1.color.b - _colorKey2.color.b) > 0.01f) return false;
            return true;
        }


        /// <summary>
        /// Creates a MeshRenderer disable shadows and occlusion.
        /// </summary>
        /// <param name="_gameObject"></param>
        protected virtual void CreateMeshRenderer(Transform _transform = null)
        {
            if (_transform == null)
                _transform = transform;
            MeshRenderer _meshRenderer = _transform.GetComponent<MeshRenderer>();
#if UNITY_2017_2_OR_NEWER
            _meshRenderer.allowOcclusionWhenDynamic = false;
#endif
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;            
        }


    }
}