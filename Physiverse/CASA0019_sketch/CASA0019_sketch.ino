#include <WiFiNINA.h>
#include <ezTime.h>
#include <PubSubClient.h>
#include <Servo.h>
#include "arduino_secrets.h" 

//WIFI and MQTT connection parameters
const char* ssid     = SECRET_SSID;
const char* password = SECRET_PASS;
const char* mqttuser = SECRET_MQTTUSER;
const char* mqttpass = SECRET_MQTTPASS;
const char* mqtt_server ="mqtt.cetools.org";

WiFiClient espClient;
PubSubClient client(espClient);
Servo myservo;  // create servo object to control a servo
Timezone GB;  // Date and time

int pos = 0;    // variable to store the servo position
long lastMsg = 0;
char msg[50];
int value = 0;
int len;  //Set length of array
char *Array_value[20];  //Array to hold MQTT plant monitor names
char *Array_topic[20];  //Array to hold MQTT plant monitor moisture readings

void setup() {
  Serial.begin(9600);
  
  setup_wifi();
  syncDate();
  myservo.attach(9);  // attaches the servo on pin 9 to the servo object
  client.setServer(mqtt_server, 1884);
  client.setCallback(callback);
  len =0;
  for (int i = 0; i < 20; ++i){ //Initialise arrays
    Array_topic[i] = (char*)malloc( 20); // string length up to 19 bytes + null
    Array_value[i] = (char*)malloc( 10 ); // string length up to 9 bytes + null
  }
  reconnect();
}

void syncDate() {
  // get real date and time
  waitForSync();
  Serial.println("UTC: " + UTC.dateTime());
  GB.setLocation("Europe/London");
  Serial.println("London time: " + GB.dateTime());

}

/* Function: ssetupWifi()
 * Desc: Use the Wifi UNO rev2 wifi libraries
 * to create the WIFI network with
 * the provided credentials.
 */
void setup_wifi() {
  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);
  Serial.print("status");
  Serial.print(WiFi.status());
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

/* Function: callback()
 * Desc: Function is called each time sensor
 * readings are published on MQTT server which you have subscribed to:
 * on topic: student/CASA0014/plant/#
 */
void callback(char* topic, byte* message, unsigned int length) {
  Serial.println("Message arrived\n");

  int isElementPresent =0;
  String s = String(topic);

  int elementAtIndex =0;
  if(strstr(topic,"moisture") != NULL)  //Sort the list of topics to find moisture topics
  {
    char messageTemp[4];
    
    for (int i = 0; i < length; i++) {
      messageTemp[i]=(char)message[i];  //Store the MQTT value of topic
      
    }
    messageTemp[length]='\0'; //Set the last char to null
    int i = 0;  
    //Check if the array topic exists in the topic array
    while(len != 0 && i < len ) {
        if ((s.indexOf(Array_topic[i]))>0) {  
            elementAtIndex = i; //save array index to update value
            isElementPresent = 1;
            break;
        }
        i++;
    }
    //If element is not present, push new element. Else update value for existing element.
    if(!(isElementPresent))
      {
            Serial.print("Push new element\n");
            memset(Array_topic[len], '\0', sizeof(Array_topic[len]));
            memset(Array_value[len], '\0', sizeof(Array_value[len]));
            strncpy(Array_topic[len],topic+23,((strlen(topic)))*sizeof(char)); //Push only the last part of the topic name
            strncpy(Array_value[len],messageTemp,(length+1)*sizeof(char));  //Push array value into a separate array

            len ++;
      } 
      else {
        
            Serial.print("Update existing\n");
            memset(Array_value[len], '\0', sizeof(Array_value[len])); //clear the previous value
            strncpy(Array_value[i],messageTemp,(length+1)*sizeof(char));  //update array value at saved index
      }
  }  

}

/* Function: reconnect()
 * Desc: Function is called when loopMqtt()
 * is called for the first time. If server 
 * gets disconnected, keep retrying to establish
 * server connection before publishing data.
 */
void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    
    // Attempt to connect with clientID, username and password
    if (client.connect("ESP8266Client",mqttuser,mqttpass)) {
      Serial.println("connected");
      // ... and resubscribe
      client.subscribe("student/CASA0014/plant/#");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void loop() {
  delay(200);
  int count =0;
  if (minuteChanged()) {
    loopMQTT();
    Serial.println(GB.dateTime("H:i:s")); // UTC.dateTime("l, d-M-y H:i:s.v T")
  }
  
  client.loop();  

  long now = millis();
  if (now - lastMsg > 5000) { //Update LCD and gauge pointer at fixed intervals
    String topics; 
    lastMsg = now;
    Serial.print("\npublish data\n");
    for (int i = 0; i < len; i++) {
        if((atoi(Array_value[i])) <10)
        {
          topics += Array_topic[i];
          topics += " ";
          count++ ; //Count the number of topics with moisture value < 10
        }
    }
   float percentThirsty = (float)(count * 100)/len ; //Calculate percentage of thirsty plants
   int pos = map(percentThirsty,0,100,0,180); //convert percentage of plants into servo angle
   Serial.print("Servo angle");
   Serial.print(pos);
   myservo.write(pos);
   Serial.print("Number of thirsty plants/Number of total plants   ");
   Serial.print(count); 
   Serial.print("/");
   Serial.print(len); 
   Serial.println();
   Serial.println(topics);   
   Serial.println();
    
  }
}

void loopMQTT() {
 // try to reconnect if not
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
  
}
