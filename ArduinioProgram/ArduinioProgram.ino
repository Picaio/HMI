
/*
*P= pwm data
*A=led state 1
*D=digital state 1
*E=digital state 2
*F=digital state 3
*T=pantalla
*C=clearpantalla
*Q=rele1
*R=rele2
*/

#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

#define SCREEN_WIDTH 128 // OLED display width, in pixels
#define SCREEN_HEIGHT 64 // OLED display height, in pixels

// Declaration for an SSD1306 display connected to I2C (SDA, SCL pins)
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, -1);


int analogPin = A0; 
int val = 0;
char datoCmd;
int pwmValue=0;
int ledPwm=9;
int Led1 = 7;
int Led2 = 6;
int Led3 = 5;
int rele1 = 4;
int rele2 = 3;
String serialData;
int lines =0; //lleva cuenta del tamaño de pantalla



void setup() {
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
  pinMode(Led1, OUTPUT);
  pinMode(Led2, OUTPUT);
  pinMode(Led3, OUTPUT);
  pinMode(rele1, OUTPUT);
  pinMode(rele2, OUTPUT);
  Serial.begin(115200);
  digitalWrite(Led1, LOW);
  digitalWrite(Led2, LOW);
  digitalWrite(Led3, LOW);
  digitalWrite(rele1, LOW);
  digitalWrite(rele2, LOW);

   if(!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) { // Address 0x3D for 128x64
    Serial.println(F("SSD1306 allocation failed"));
    for(;;);
  }
  delay(2000);
  display.clearDisplay();

  display.setTextSize(1);
  display.setTextColor(WHITE);
  display.setCursor(0, 5);
  // Display static text
  display.println("Hello, world!");
  display.display(); 
  
}

// the loop function runs over and over again forever
void loop() {

analogData();
//ledData();
analogWrite(ledPwm,pwmValue);
delay(200);

}

void analogData(){
   val = analogRead(analogPin);  // read the input pin
   Serial.print("T");
   Serial.println(val*1.0);          // debug value
  }
void ledData(){
  digitalWrite(LED_BUILTIN, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(200);
  Serial.print("A");
  Serial.println("1");// wait for a second
  digitalWrite(LED_BUILTIN, LOW);    // turn the LED off by making the voltage LOW
  delay(200);                       // wait for a second
  Serial.print("A");
  Serial.println("0");// wait for a second 
  }
  
void serialEvent() {
   datoCmd = (char)Serial.read();
  if(datoCmd == 'P')
  {
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
            pwmValue= serialData.toInt();
      }
        serialData="";
   }
   if(datoCmd == 'Q')
    {
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
            int dato = serialData.toInt();
            digitalWrite(rele1, dato);
      }
        serialData="";
    }
    if(datoCmd == 'R')
    {
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
            int dato = serialData.toInt();
            digitalWrite(rele2, dato);
      }
        serialData="";
    }

   if(datoCmd == 'D')
    {
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
            int dato = serialData.toInt();
            digitalWrite(Led1, dato);
      }
        serialData="";
    }
     if(datoCmd == 'E')
    {
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
            int dato = serialData.toInt();
            digitalWrite(Led2, dato);
      }
        serialData="";
    }
     if(datoCmd == 'F')
    {
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
            int dato = serialData.toInt();
            digitalWrite(Led3, dato);
      }
        serialData="";
    }
    if(datoCmd == 'C')
    {
      clearOled();
    }
    if(datoCmd == 'T')
    {
      lines++;
      if(lines>4){clearOled();lines=0;}
    while(Serial.available())
      {
            datoCmd = (char)Serial.read();
            serialData += datoCmd;
                   
      }
        display.println(serialData);
        display.display(); 
        serialData="";
    }
}
void clearOled()
{
     display.clearDisplay();
     display.setTextSize(1);
     display.setTextColor(WHITE);
     display.setCursor(0, 5);
     display.display(); 
}
