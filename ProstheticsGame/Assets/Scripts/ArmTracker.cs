using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Vuforia;

public class ArmTracker : MonoBehaviour
{
    #region PRIVATE_MEMBERS

    private const Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.RGBA8888;
    
    private bool mFormatRegistered = false;

    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);

    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());

            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError(
                "\nFailed to register pixel format: " + mPixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");

            mFormatRegistered = false;
        }

    }

    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
            
            if (image != null)
            {
                Debug.Log(
                    "\nImage Format: " + image.PixelFormat +
                    "\nImage Size:   " + image.Width + "x" + image.Height +
                    "\nBuffer Size:  " + image.BufferWidth + "x" + image.BufferHeight +
                    "\nImage Stride: " + image.Stride + "\n"
                );

                byte[] pixels = image.Pixels;

                if (pixels != null && pixels.Length > 0)
                {
                    Mat rawMat = new Mat(image.Height, image.Width, MatType.CV_8UC4, pixels);
                   
                    Mat detected = new Mat();
                    Cv2.Canny(rawMat, detected, 50, 150);

                    Cv2.ImWrite("swagdude.png", detected);

                    /*Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, false);
                    var byteArray = new byte[image.Width * image.Height * 4];
                    System.Runtime.InteropServices.Marshal.Copy(detected.Data, byteArray, 0, byteArray.Length);
                    tex.LoadRawTextureData(byteArray);
                    tex.Apply();*/
                }
            }
            
        }
    }

    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();

            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }

    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }

    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }

    #endregion //PRIVATE_METHODS
}
