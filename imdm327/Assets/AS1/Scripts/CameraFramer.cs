using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFramer : MonoBehaviour
{
    public Camera targetCamera;           // The camera you want to adjust
    public List<Transform> objectsToFrame; // List of objects to frame

    public float padding = 1.1f;          // Padding to add around the objects 
    [Header("Change to frame more or less planets")]
    public int frameUpTo = 1;

    private int currentIndex;
    void Start()
    {
        currentIndex = frameUpTo;
        FrameObjects();
    }

   void Update()
    {
        if ( currentIndex != frameUpTo)
        {
            currentIndex = frameUpTo;
            FrameObjects(); // Only call this if all objects are not on screen
        }
    }


    void FrameObjects()
    {
        if (objectsToFrame == null || objectsToFrame.Count == 0)
            return;

        // Find the bounds of all objects
        Bounds bounds = new Bounds(objectsToFrame[0].position, Vector3.zero);
        // foreach (Transform obj in objectsToFrame)
        // {
            
        // }
        for(int i = 0; i <= Math.Min(frameUpTo-1,objectsToFrame.Count-1); i++){
            bounds.Encapsulate(objectsToFrame[i].GetComponent<Renderer>().bounds);
        }

        // Center the camera along the -Y axis
        Vector3 cameraDirection = Vector3.down; // -Y axis
        Vector3 targetPosition = bounds.center + (-cameraDirection * bounds.extents.y * padding);

        // Adjust the camera position to look at the bounds center
        targetCamera.transform.position = targetPosition;
        targetCamera.transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.forward);

        // Adjust the camera's field of view based on the bounds size
        float boundsSize = Mathf.Max(bounds.extents.x, bounds.extents.z);
        float distanceToCamera = (boundsSize / Mathf.Tan(Mathf.Deg2Rad * targetCamera.fieldOfView / 2f)) * padding;

        // Move the camera back by the calculated distance
        targetCamera.transform.position = bounds.center - cameraDirection * distanceToCamera;
    }
}
