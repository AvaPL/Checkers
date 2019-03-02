Imphenzia - GradientSky

** WHAT IS IT? **

Components to create a simple gradient sky, either as a separate game object
or fixed to the camera.

A gradient sky as a separate game object is useful if you want to be able to 
pan around as the camera moves within the scene. This is good if you want
to create a platformer where up/down movement in the scene should result
the sky changing color. Note that you should scale the gradient sky much
larger than the what the camera displays.

A gradient sky fixed to the camera is useful if you want no movement of the
sky even if the camera moves. The camera gradient sky is resized to be
exactly the same size as the viewport of the camera and it always renders
as a background.



** HOW TO USE GRADIENT SKY AS A SEPARATE GAME OBJECT? **

1. Create an empty GameObject in your scene

2. Click on "AddComponent" in the Inspector

3. Add the "GradientSkyObject" component
   (location: Imphenzia/GradientSky/Scripts/GradientSkyObject.cs) 

4. Change position and scale of the transform of the game object to the
   desired size.

6. Click on the Gradient color range in the inspector to set the gradient



** HOW TO USE GRADIENT SKY FIXED TO THE CAMERA? **

1. Select the MainCamera object in your scene

2. Click on "AddComponent" in the inspector

3. Add the "GradientSkyCamera" component
   (location: Imphenzia/GradientSky/Scripts/GradientSkyCamera.cs) 

4. Set the Camera's Camera Clear Flag to something other than Skybox, use
   SolidColor for example.

5. Set the "Place At Clipping Plane" to either FAR or NEAR clipping plane
   (this has no visual impact, it's only for personal preference in scene view)

6. Click on the Gradient color range in the inspector to set the gradient

  