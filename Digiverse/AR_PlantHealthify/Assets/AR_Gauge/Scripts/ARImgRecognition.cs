using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARImgRecognition : MonoBehaviour
{
    private ARTrackedImageManager _arTrackedImageManager;

    public List<GameObject> GameObjectToSpawn = new List<GameObject>(); //List of GameObjects / Prefab we want to instantiate on the images
    private List<GameObject> InstatiatedGObject = new List<GameObject>();//Internal List of the GameObjects instantiated
    private float timer; //check for how long the marker is out of the camera view

    public int indexOfImg=0;
    private void Awake()
    {
        //Why are we instantiating 2 objects instead of 4?
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        InstatiatedGObject.Add(Instantiate(GameObjectToSpawn[0], Vector3.zero, Quaternion.identity));
        InstatiatedGObject[0].name = GameObjectToSpawn[0].name;  //AR object 0
        InstatiatedGObject[0].SetActive(false);

        InstatiatedGObject.Add(Instantiate(GameObjectToSpawn[1], Vector3.zero, Quaternion.identity));
        InstatiatedGObject[1].name = GameObjectToSpawn[1].name;   //AR object 1
        InstatiatedGObject[1].SetActive(false);

        InstatiatedGObject.Add(Instantiate(GameObjectToSpawn[2], Vector3.zero, Quaternion.identity));
        InstatiatedGObject[2].name = GameObjectToSpawn[2].name;   //AR object 1
        InstatiatedGObject[2].SetActive(false);

        InstatiatedGObject.Add(Instantiate(GameObjectToSpawn[3], Vector3.zero, Quaternion.identity));
        InstatiatedGObject[3].name = GameObjectToSpawn[3].name;   //AR object 1
        InstatiatedGObject[3].SetActive(false);

    }


    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;

    }


    IEnumerator timerMarker(int index)
    {
        while (timer < 2)
        {
            timer += Time.deltaTime;
            Debug.Log(timer);
            yield return null;
        }

        timer = 0;
        InstatiatedGObject[index].SetActive(false);
    }


    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.updated)
        {
            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-01")
            {
                indexOfImg =0;
                InstatiatedGObject[0].transform.position = trackedImage.transform.position;
                InstatiatedGObject[0].transform.localEulerAngles = trackedImage.transform.localEulerAngles;
                Debug.Log("Spawned game object with name:"+InstatiatedGObject[0].name);
                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    timer = 0;
                    Debug.Log("Tracking...");
                    InstatiatedGObject[0].SetActive(true);
                }

                else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {

                    Debug.Log("Limited...");

                    StartCoroutine(timerMarker(0));
                   // newARObj[0].SetActive(false);

                }

            }

            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-02")
            {
                indexOfImg =1;
                InstatiatedGObject[1].transform.position = trackedImage.transform.position;
                InstatiatedGObject[1].transform.localEulerAngles = trackedImage.transform.localEulerAngles;

                Debug.Log("Spawned game object with name:"+InstatiatedGObject[0].name);
                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    Debug.Log("TrackingIn...");
                    timer = 0;
                    InstatiatedGObject[1].SetActive(true);
                }

                else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    Debug.Log("Limited...");

                    StartCoroutine(timerMarker(1));
                    //newARObj[1].SetActive(false);
                }

            }

            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-03")
            {
                indexOfImg =2;
                InstatiatedGObject[2].transform.position = trackedImage.transform.position;
                InstatiatedGObject[2].transform.localEulerAngles = trackedImage.transform.localEulerAngles;


                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    Debug.Log("TrackingIn...");
                    timer = 0;
                    InstatiatedGObject[2].SetActive(true);
                }

                else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    Debug.Log("Limited...");

                    StartCoroutine(timerMarker(2));
                    //newARObj[1].SetActive(false);
                }

            }

            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-04")
            {

                InstatiatedGObject[3].transform.position = trackedImage.transform.position;
                InstatiatedGObject[3].transform.localEulerAngles = trackedImage.transform.localEulerAngles;


                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    Debug.Log("TrackingIn...");
                    timer = 0;
                    InstatiatedGObject[1].SetActive(true);
                }

                else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    Debug.Log("Limited...");

                    StartCoroutine(timerMarker(3));
                    //newARObj[1].SetActive(false);
                }

            }
        }

    }
}
