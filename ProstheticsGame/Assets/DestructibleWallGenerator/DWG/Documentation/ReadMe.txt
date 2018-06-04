  //******************************************************//
 //* Thank you for buying my Destructible Wall Builder! *//
//******************************************************//
//
// This tool is used to create brick walls that can be destructible. You're also given the option to create up to 4 walls in a square to make buildings and such.
// This wall generator uses the Editor to make the walls. You'll have a single game object that holds the GenerateWall component, and that's all you need to build the walls.
// You're given the option to add colliders, add physics, set kinematic on sleep, change Axis depending on your models origin (Blender has swapped Z and Y), set length, height, and sides to the wall.
//
  ///////////////////
 //* TEST IT OUT *//
///////////////////
//
// First open up the main scene that comes with the asset and check it out
// Scene is located under the folder: /DestructibleWallGenerator/Example/Scene/
// You can open the Generator by selecting DWG > Generator in the file menu above.
//
  ////////////////////////////
 //* HOW TO SETUP A SCENE *//
////////////////////////////
//
// STEP 1: 	Open up a brand new scene and create a ground of some sort so that your walls don't look like they are floating. Also add a Directional Light so you can see ;)
//
// STEP 2: 	Open the Generator (Top file menu, under DWG > Generator)
//
// STEP 3: 	Use the options in the generator to create your wall. First you need to add a GameObject for your Prefab brick.
//		   	You can use any object, but there are some Prefabs in the prefab folder you can use under /DestructibleWallGenerator/Example/Prefabs
//			For now add the Brick prefab to the inspector.
//			Otherwise you can select Create Brick to make your own GameObject
//
// Step 4:	The "Brick" prefab is an object made in Blender, because Blender uses an Up Axis of Z, select Z for the Up Axis.
//
// STEP 5:	Choose your length, heigh, sides and press Generate.
//
// Your wall should appear at Vector3.zero OR the GameObject you selected under "Wall Position". Now you're free to play around with the Inspector to add colliders, physics, etc.
// If you would like to test the physics and see your objects in game, you can go to the Prefab folder and drop in the Main Camera prefab, which has controls to move around
// The prefab camera also allows you to shoot a ball at the walls by pressing CTRL
//
// *IMPORTANT: If you're using the wall generator and you want to destroy them with your own projectile, such as a rocket or ball. Your projectile will need to have the DWGDestroyer script on it
// You can find the script under DestructibleWallGenerator/DWG/Scripts/DWGDestroyer
//
// ENJOY! All scripts are located under /DestructibleWallGenerator/DWG/Scripts and you're free to do whatever you want with them!
// For more info, watch these videos below!
//
  /////////////
 //* VIDEO *//
/////////////
//
// How to use it: http://youtu.be/IjELu_Q01eQ
//
  /////////////////////////////////////////
 //* Not so frequently asked questions *//
/////////////////////////////////////////
//
// 1.a) 	My game objects are not lining up properly when a wall is generated.
//			A.) Make sure the Up Axis property is set to the correct axis type. Unity objects like Cubes, Spheres, etc, use a Up Axis of Y
//				Other 3D models, such as ones designed in Blender, use an Up Axes of Z
//
// 1.b)		Nope! Still doesn't work, everything is all out of wack!
//			A.) Make sure you're scaling is correct. This is designed to work with brick shapes. So your Z axis must be half the size of your X axis
//				Also make sure your object has the right axis, blender uses different axis than unity objects.
//			
//					Examples: 	Unity Object: (x: 2, y: .75, z: 1)  
//								Blender Object: (x: 2, y: 1, z: .75)
//
// 2.)	How come my colliders are disabled when I generate a wall with colliders?
//		A.)	Working as intended. The reason for this is because Unity has delay issues if you have too many colliders in the Scene view.
//			If you made a wall 25x25 with 4 sides and had colliders on every brick, moving your walls in the scene view would be a nightmare.
//			To resolve this, colliders are disabled, and they become enabled when you press PLAY. There's a component called <ColliderEnabler> on-
//			every brick that has a collider, once the game starts, it enables the collider and removes the ColliderEnabler component.
//
// 3.) 	What's the point of adding Kinematic on Sleep to your walls?
//		A.) This makes it so when an object with a rigidbody goes to sleep (stops moving) it enables isKinematic so that the object no longer-
//			is effected by physics. That way other physics based objects don't cause inactive objects to move unless force is added via an explosion.
//				-> This option is completely option and not required
//
// 4.)	How come with some of my brick objects when I try to make a large wall like 25x25 it takes a long time to generate?
//		A.) This is most likely because you have a collider already on your Prefab. The generator adds colliders of your choice, so if your prefab already has a collider-
//			there is a failsafe to remove any pre-existing colliders before applying the new collider. This can delay the generator, I suggest not having any colliders on your prefabs.
//
// 5.)	Hey howcome when I use a Single Collider on my wall it doesn't line up properly??!
//		A.) My Bad! This is an issue I've been having with the math. This only happens if you have a Wall Height set to an ODD number. If it's set to an EVEN number, everything works great.
//			This is on my ToDo list!! :)
//
// 6.) 	I have more questions and problems but this FAQ didn't help me at all.
//		A) Send me an e-mail or tweet, find that information below! :)
//
  ///////////////
 //* CONTACT *//
///////////////
//
// E-Mail: mike.desjardins@outlook.com
// Twitter: @ZeroLogics
//
// I'm fairly active on Twitter and through e-mail, so I'm not too hard to get a hold of.
//
//
//
// Thanks you and enjoy!!
//