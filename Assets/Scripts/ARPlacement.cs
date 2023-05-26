using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    // Fill in the ...

    /* Private and public variables control data access. Private variables are
     * restricted to their defining scope, while public variables are accessible
     * from anywhere.*/
    public GameObject arObjectToSpawn;
    // This GameObject should be accessible from anywhere
    public GameObject placementIndicator;

    private GameObject spawnedObject;
    // This Pose object should restricted to only this script
    private Pose placementPose;
    private ARRaycastManager aRRaycastManager;

    /* Variable types, such as bool, define the kind of data a variable can hold.
     * bool represents a boolean value, which can be either true or false. Boolean
     * are commonly used for logical operations and conditions in programming.
     * They help make decisions and control the flow of a program based on the
     * evaluation of true or false conditions.*/
    // Make this a restricted bool, with a default of false
    private bool placementPoseIsValid = false;


    /* In game development using Unity, MonoBehaviour is a base class that provides
     * functionality to create game objects and attach scripts to them. The Start
     * function is called once when the object is initialized, while the Update function
     * is repeatedly called every frame. Start is often used for initialization tasks,
     * while Update is used for continuous updating and processing of game logic.*/
    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        // We only want to spawn the object if there isn't one already, the position is good and when the screen registers a touch
        // When a object is created and not yet assigned, it's value is null
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }

        // Here we want to call two function we declare later in the script
        // One that updates the placement position of our target
        // and one that updates the visiblility of the target
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    // This function updates if the target should be visible or not
    private void UpdatePlacementIndicator()
    {
        // We only want to see our target indicator if the object isn't yet initiated and assigned, and the placement is valid
        if (spawnedObject == null && placementPoseIsValid)
        {
            // We can turn the visibility of an object on and off by setting it active or not given a Boolean
            // In this case we want to see the object
            // This link might be usefull
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            // In this case we don't want to see the object
            placementIndicator.SetActive(false);
        }
    }

    // This function updates the placement position of our target
    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        // Remeber the bool variable we default set to false?
        // Here is our Raycast casts a ray that has at least one or more hits with a plane surface, the value gets turned to true
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    // This function instantiates our object
    private void ARPlaceObject()
    {
        // We want to use a function here that given three parameters (a object to spawn, a position and a rotation) instantiates and returns a object
        // The returned object is then assigned to the spawnedObject variable
        // This link might give some help
        spawnedObject = Instantiate(arObjectToSpawn, placementPose.position, placementPose.rotation * Quaternion.Euler(0, 180, 0));
    }
}
