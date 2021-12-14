using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class ARImgRecognition : MonoBehaviour
{
    private ARTrackedImageManager _arTrackedImageManager;

    public List<GameObject> GameObjectToSpawn = new List<GameObject>(); //List of GameObjects / Prefab we want to instantiate on the images
    public List<Texture2D> TwoDCutoutToSpawn = new List<Texture2D>(); //List of GameObjects / Prefab we want to instantiate on the images
    private List<GameObject> InstatiatedGObject = new List<GameObject>();//Internal List of the GameObjects instantiated
    private float timer; //check for how long the marker is out of the camera view


    private mqttReceiver _mqttReceiver;
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

        if (GameObject.FindGameObjectsWithTag("moisture").Length > 0)
        {
            Debug.Log("Game object found");
            _mqttReceiver= GameObject.FindGameObjectsWithTag("moisture")[0].gameObject.GetComponent<mqttReceiver>();
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttReceiver component and Tag == tagOfTheMQTTReceiver needs to be provided");
        }
        //_mqttReceiver=this.GetComponent<mqttReceiver>();
        

    }


    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
        Debug.Log("onEnable");
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        Debug.Log("onDisable");
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
        if (_mqttReceiver.isConnected == true)
            _mqttReceiver.Disconnect();
    }


    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.updated)
        {
            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-01")
            {
                InstatiatedGObject[0].transform.position = trackedImage.transform.position;
                InstatiatedGObject[0].transform.localEulerAngles = trackedImage.transform.localEulerAngles;

                //InstatiatedGObject[4].transform.position = GameObjectToSpawn[4].transform.position;
               // InstatiatedGObject[4].transform.localEulerAngles = GameObjectToSpawn[4].transform.localEulerAngles;

                InstatiatedGObject[0].transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material.mainTexture = TwoDCutoutToSpawn[0];

                Debug.Log("Set topic to subscribe"+_mqttReceiver.topicSubscribe);
                
                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    timer = 0;
                    if (!(_mqttReceiver.topicSubscribe.Contains("ucfnaka")))
                    {
                        /*if (_mqttReceiver.isConnected == true)
                        {
                            _mqttReceiver.Disconnect();
                        }*/
                        Debug.Log("1st time");
                        _mqttReceiver.topicSubscribe = "student/CASA0014/plant/ucfnaka/moisture";
                        _mqttReceiver.Connect();
                    }
                    else
                    {
                        if (_mqttReceiver.isConnected == false)
                        {
                            _mqttReceiver.Connect();
                        }
                    }
                    Debug.Log("Tracking...");
                    InstatiatedGObject[0].SetActive(true);
                   
                }

                else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {

                    Debug.Log("Limited...");
                    //_mqttReceiver.Disconnect();
                    StartCoroutine(timerMarker(0));

                   // newARObj[0].SetActive(false);

                
                }

            }

            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-02")
            {
                
                InstatiatedGObject[1].transform.position = trackedImage.transform.position;
                InstatiatedGObject[1].transform.localEulerAngles = trackedImage.transform.localEulerAngles;
                
                InstatiatedGObject[1].transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material.mainTexture = TwoDCutoutToSpawn[1];

                Debug.Log("Set topic to subscribe");
                

                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    Debug.Log("TrackingIn...");
                    timer = 0;
                    if (!(_mqttReceiver.topicSubscribe.Contains("ucfnwho")))
                    {
                        /*if (_mqttReceiver.isConnected == true)
                        {
                            _mqttReceiver.Disconnect();
                        }*/
                        Debug.Log("1st time");
                        _mqttReceiver.topicSubscribe = "student/CASA0014/plant/ucfwho/moisture";
                        _mqttReceiver.Connect();
                    }
                    else
                    {
                        if (_mqttReceiver.isConnected == false)
                        {
                            _mqttReceiver.Connect();
                        }
                    }
                    InstatiatedGObject[1].SetActive(true);
                   // _mqttReceiver.Connect();
                }

                else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
                {
                    Debug.Log("Limited...");

                    StartCoroutine(timerMarker(1));
                    //newARObj[1].SetActive(false);
                   // _mqttReceiver.Disconnect();
                }

            }

            //string to evaluate is the name of the marker provided in XR Reference Image
            if (trackedImage.referenceImage.name == "Marker-03")
            {

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
