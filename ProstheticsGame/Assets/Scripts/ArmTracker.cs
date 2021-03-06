﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Vuforia;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class ArmTracker : MonoBehaviour, ITrackableEventHandler
{
    #region PRIVATE_MEMBERS

    private const Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.RGBA8888;
    private TrackableBehaviour mTrackableBehaviour;
    private bool mFormatRegistered = false;

    #endregion // PRIVATE_MEMBERS

    public Texture2D mOpenCvTexture;
    public GameObject mOpenCvTarget;
    public GameObject mMarker;

    public GameObject mBody;

    public GameObject myo = null;
    // The pose from the last update. This is used to determine if the pose has changed
    // so that actions are only performed upon making them rather than every frame during
    // which they are active.
    private Pose _lastPose = Pose.Unknown;
    private float mArmExtension;
    public GameObject mUpperArm;
    public GameObject mPivot;
    private Vector3 mOldPosition;
    public GameObject mBall;

    public GameObject rotateWorld;

    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);

        mTrackableBehaviour = mMarker.GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        mBody.SetActive(false);

        mOldPosition = mPivot.transform.position;
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    void Update()
    {
        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();

        var hasMoved = false;
        if (thalmicMyo.pose == Pose.WaveIn)
        {
            Debug.Log("Wave in");
            this.mArmExtension += -1.0f * Time.deltaTime;
            if (this.mArmExtension <= -1.25f)
            {
                this.mArmExtension = -1.25f;
            } else { 
                mUpperArm.transform.Translate(new Vector3(-1.0f * Time.deltaTime, 0.0f, 0.0f));
                ExtendUnlockAndNotifyUserAction(thalmicMyo);
                hasMoved = true;
            }
        }
        else if (thalmicMyo.pose == Pose.WaveOut)
        {
            Debug.Log("Wave out");
            this.mArmExtension += 1.0f * Time.deltaTime;
            if (this.mArmExtension >= 2.6f)
            {
                this.mArmExtension = 2.6f;
            }
            else
            {
                mUpperArm.transform.Translate(new Vector3(1.0f * Time.deltaTime, 0.0f, 0.0f));
                ExtendUnlockAndNotifyUserAction(thalmicMyo);
                hasMoved = true;
            }

        }
        else if (thalmicMyo.pose == Pose.Fist) {
            Debug.Log("Fist!!!!!!!!11");

            rotateWorld.transform.Rotate(new Vector3(0, 1, 0));
        }

        var delta = mPivot.transform.position - mOldPosition;

        //Debug.Log("Delta " + delta);

        if (hasMoved)
        {
            mBall.GetComponent<Rigidbody>().AddForce(-delta * 5000);
        }


        mOldPosition = mPivot.transform.position;
    }

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


        /*
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
                    Mat ping = new Mat(image.Height, image.Width, MatType.CV_8UC4, pixels);
                    Mat pong = new Mat();
                    Cv2.Resize(ping, pong, new Size(640, 480));

                    Cv2.CvtColor(pong, ping, ColorConversionCodes.BGRA2BGR);
                    Cv2.CvtColor(ping, pong, ColorConversionCodes.BGR2HSV);

                    Mat skinMaskPing = new Mat();
                    Mat skinMaskPong = new Mat();
                    Cv2.InRange(pong, new Scalar(0, 48, 80), new Scalar(120, 255, 255), skinMaskPing);

                    Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(11, 11));

                    Cv2.Erode(skinMaskPing, skinMaskPong, kernel, null, 2);
                    Cv2.Dilate(skinMaskPong, skinMaskPing, kernel, null, 2);
                    Cv2.GaussianBlur(skinMaskPing, skinMaskPong, new Size(3, 3), 0);

                    //Cv2.BitwiseAnd(pong, pong, ping, skinMaskPong);



                    //Cv2.ImWrite("swagdude.png", detected);
                    //pong = ping;
                    Cv2.CvtColor(pong, ping, ColorConversionCodes.HSV2RGB);

                    pong = ping;
                    int width = pong.Width, height = pong.Height;
                    if (mOpenCvTexture == null)
                    {
                        mOpenCvTexture = new Texture2D(width, height);
                    }
                    mOpenCvTexture.LoadImage(pong.ToBytes());

                    mOpenCvTarget.GetComponent<SpriteRenderer>().sprite = Sprite.Create(mOpenCvTexture, new UnityEngine.Rect(0.0f, 0.0f, width, height), new Vector2(0.5f, 0.5f), 1.0f);
                }
            }
            
        }*/
    }

    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Active");
            mBody.SetActive(true);
        }
        else
        {
            Debug.Log("Inactive");
            mBody.SetActive(false);
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
    void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard)
        {
            myo.Unlock(UnlockType.Timed);
        }

        myo.NotifyUserAction();
    }

    #endregion //PRIVATE_METHODS
}
