using UnityEngine;
public class mqttController : MonoBehaviour
{
    public string nameController = "Controller";

    public string topic = "";
    public string tagOfTheMQTTReceiver = "";
    public GameObject objectToControl;
    private float numValue = 0.0f;

    public mqttReceiver _mqttReceiver;

    private void Start()
    {

        if (GameObject.FindGameObjectsWithTag("moisture").Length > 0)
        {
            Debug.Log("Game object found");
            _mqttReceiver = GameObject.FindGameObjectsWithTag("moisture")[0].gameObject.GetComponent<mqttReceiver>();
        }
        else
        {
            Debug.LogError("At least one GameObject with mqttReceiver component and Tag == tagOfTheMQTTReceiver needs to be provided");
        }
        _mqttReceiver.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(mqttObj mqttObject)
    {
        Debug.Log("Event Fired. The message, from Object " + nameController + " is = " + mqttObject.msg);
        string tempMoistureTopic = topic.Substring(0,topic.Length-1);
        tempMoistureTopic += "moisture";
        Debug.Log("Temp topic ="+tempMoistureTopic);
        Debug.Log("topic ="+mqttObject.topic);
        if (mqttObject.topic == tempMoistureTopic)
        {
            if(mqttObject.topic.Contains("moisture"))
            {
                numValue = float.Parse(mqttObject.msg);
                Debug.Log("1");
            // this.gameObject.transform.Find("MSG_MQTT").gameObject.GetComponent<TextMeshPro>().text = mqttObject.msg;
            // Debug.Log(mqttObject.topic + " |||" + mqttObject.msg);
            }
                    
        }

        string tempTempTopic = topic.Substring(0,topic.Length-1);
        tempMoistureTopic += "temperature";
        Debug.Log("Temp topic ="+tempTempTopic);
        Debug.Log("topic ="+mqttObject.topic);
        if (mqttObject.topic == tempTempTopic)
        {
            if(mqttObject.topic.Contains("moisture"))
            {
                Debug.Log("2");
                //this.gameObject.transform.Find("MSG_MQTT").gameObject.GetComponent<TextMeshPro>().text = mqttObject.msg;
                //Debug.Log(mqttObject.topic + " |||" + mqttObject.msg);
            }
                    
        }
    }


    /*private void OnMessageArrivedHandler(string newMsg)
    {
        numValue = float.Parse(newMsg);
        Debug.Log("Event Fired. The message, from Object " + nameController + " is = " + newMsg);
    }*/

     private void Update()
    {
        float step = 0.5f * Time.deltaTime;

        Vector3 rotationVector = new Vector3(objectToControl.transform.localEulerAngles.x, -numValue * 4.5f, objectToControl.transform.localEulerAngles.z);

        objectToControl.transform.localRotation = Quaternion.Lerp(objectToControl.transform.localRotation, Quaternion.Euler(rotationVector), step);

    }
}