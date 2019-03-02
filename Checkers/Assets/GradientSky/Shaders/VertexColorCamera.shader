// Plain Vertex Color Shader with background render queue and zwrite disabled

Shader "Custom/VertexColorCamera" {
	Properties{
	}
	SubShader{
		// Make sure this is rendererd as a background
		Tags{ "Queue" = "Geometry-1000" }
		// Ensure ZWrite is disabled so no other objects are occluded
		ZWrite Off
		Pass{
		}
	}
}
