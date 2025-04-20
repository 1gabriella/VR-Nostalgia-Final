You need TMP, XRInteraction packages. Should work on OpenXR an Oculus XR plugins, but you must use XRInteraction XR Origin (XR Rig) controller.
The keyboard prefab is a simple unity canvas that can work without VR.
The only thing it needs to work in VR is to have the "TrackedDeviceGraphicRaycaster" (XRInteraction package) script in the keyboard prefab root.
The inputfield you use must have the script "KeyboardInvoker" to work.
Drop keyboard prefab to scene, place the “KeyboardInvoker” script in all the input fields of your scene and press play.
When you place the prefab in the scene and it asks you to download the TMP, click yes. Once downloaded, if the keyboard does not display the letters, delete and place the prefab in the scene again.
If you place the prefab in the scene and the TMP window does not appear, install it manually.

SETUP NEW PROJECT
https://www.youtube.com/watch?v=HOLQPqsOgII