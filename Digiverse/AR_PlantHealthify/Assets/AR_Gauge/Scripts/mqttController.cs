using UnityEngine;
using TMPro;
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

        this.gameObject.transform.Find("Canvas").gameObject.transform.Find("Topic").GetComponent<TextMeshProUGUI>().text = topic;
        this.gameObject.transform.Find("Canvas").gameObject.transform.Find("Topic").GetComponent<TextMeshProUGUI>().text = topic;
        string tempMoistureTopic = topic.Substring(0,topic.Length-1);
        tempMoistureTopic += "moisture";  //Check for moisture
        Debug.Log("Temp topic ="+tempMoistureTopic);
        Debug.Log("topic ="+mqttObject.topic);

        if (mqttObject.topic == tempMoistureTopic)
        {
            if(mqttObject.topic.Contains("moisture"))
            {
                numValue = float.Parse(mqttObject.msg);
                if(numValue >= 200)
                    numValue = 199;
                Debug.Log("1");

            }
                    
        }

        string tempTempTopic = topic.Substring(0,topic.Length-1);
        tempTempTopic += "temperature"; //Check for temperature
        Debug.Log("Temp topic ="+tempTempTopic);
        Debug.Log("topic ="+mqttObject.topic);
        if (mqttObject.topic == tempTempTopic)
        {
            if(mqttObject.topic.Contains("temperature"))
            {
                Debug.Log("2");
                string tempmsg = mqttObject.msg + " C";
                this.gameObject.transform.Find("Canvas").gameObject.transform.Find("LCD-temp").GetComponent<TextMeshProUGUI>().text = tempmsg;
                
            }
                    
        }

        string tempHumTopic = topic.Substring(0,topic.Length-1);
        tempHumTopic += "humidity";    //Check for humidity
        Debug.Log("Temp topic ="+tempHumTopic);
        Debug.Log("topic ="+mqttObject.topic);
        if (mqttObject.topic == tempHumTopic)
        {
            if(mqttObject.topic.Contains("humidity"))
            {
                Debug.Log("3");
                string tempmsg = mqttObject.msg + " %";
                this.gameObject.transform.Find("Canvas").gameObject.transform.Find("LCD-hum").gameObject.GetComponent<TextMeshProUGUI>().text = tempmsg;
                
            }
                    
        }
    }


     private void Update()
    {
        float step = 0.5f * Time.deltaTime;

        Vector3 rotationVector = new Vector3(objectToControl.transform.localEulerAngles.x, -numValue * 0.9f, objectToControl.transform.localEulerAngles.z);

        objectToControl.transform.localRotation = Quaternion.Lerp(objectToControl.transform.localRotation, Quaternion.Euler(rotationVector), step);

    }
}