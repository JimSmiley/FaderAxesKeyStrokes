#include "BluetoothSerial.h"

#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif
BluetoothSerial SerialBT;

const int fwd2 = 15; ////////////////////////define hardware pin numbers
const int rvs2 = 33;                          
const int fwd1 = 14;                           
const int rvs1 = 32;                             
//const int led = 21;        // use this for simplified led behaviour, can write high or low, otherwise batOut pwm controls led.                       
const int encoder1 = 13;                            
const int encoder2 = 27;                              
const int buttonSwitch = 26;                            
const int batOut = 21;
const int topSwitch = 4;
const int batVoltage = 35; //////////////////////////////////////
bool button1;
bool button2;
const int pwmFreq = 25000;
int encoderValue;
int interrupt_cnt = 0;
int times[2];
bool toggle;
int count=0;
int sliderOne = 0;
int sliderTwo = 0;
int oldSliderOne = 0;
int oldSliderTwo = 0;
String output;
bool scrollOne = false;
bool scrollTwo = false;
int snapIncrement = 30;
const int numReadings = 6;  // number of reads to average

int readings[numReadings];      // the readings from the analog input
int readIndex = 0;              // the index of the current reading
int total = 0;                  // the running total
int average = 0;
int readings2[numReadings];      // the readings from the analog input
int readIndex2 = 0;              // the index of the current reading
int total2 = 0;                  // the running total
int average2 = 0;
int fader1Val = 0;
int fader2Val = 0;
int fader1Old = 0;
int fader2Old = 0;
int sliderOneOffset = 0;
int sliderTwoOffset = 0;
int scrollSpeed = 30;
void setup() {
 Serial.begin(115200);  // for serial coms with cable only... Helpful for troubleshooting without requiring changing serial port to bluetooth. Make sure baud rate is set in the ide as well. just need Serial.print(""); or Serial.println("");
 SerialBT.begin("RedAxis1"); //name your axis here - is what comes up in bluetooth devices. Bt serial does not need a baud rate.
ledcSetup (4, pwmFreq, 8); // sets up pwm 
for (int i = 0; i < 4; i++)
  {
    ledcSetup(i, pwmFreq, 8); // 0 - 255 duty cycle
  }
  ledcAttachPin(fwd1, 0);
  ledcAttachPin(rvs1, 1);
  ledcAttachPin(fwd2, 2);
  ledcAttachPin(rvs2, 3);
ledcAttachPin(batOut, 4); //
//pinMode(led, OUTPUT);  // more pin setups
pinMode(buttonSwitch, INPUT_PULLUP);
pinMode(topSwitch, INPUT_PULLUP);
//digitalWrite(led, HIGH);
analogReadResolution(12); //  can use highher analog read resolutions but values will get very shaky. Sid has a method that averages multiple reads for better accuracy
 
pinMode(encoder1, INPUT_PULLUP);
pinMode(encoder2, INPUT_PULLUP);
pinMode(fwd1, OUTPUT);
pinMode(rvs1, OUTPUT);
pinMode(fwd2, OUTPUT);
pinMode(rvs2, OUTPUT);
digitalWrite(fwd1, LOW);  
digitalWrite(fwd2, LOW);
digitalWrite(rvs1, LOW);
digitalWrite(rvs2, LOW);


attachInterrupt(digitalPinToInterrupt(encoder1), easyenc, RISING); // attaches interrupt on encoder.
wiggle();
wiggle();


}
 
void loop() {
  ReadAndAverageInputs();
  readsliders();
  batLevel();
  readButtons();
  

}
void readButtons()
{
  //Serial.println(digitalRead(buttonSwitch));
  //delay(10);
  if (digitalRead(buttonSwitch)!= button1)
  {
    button1 =digitalRead(buttonSwitch);
    SendOutput();
  }
  if (digitalRead(topSwitch)!= button2)
  {
    button2 =digitalRead(topSwitch);
    SendOutput();
  }
}
void batLevel()   // writes led brightness based on battery charge. Also, power switch must be on to charge battery!
{
    int bat = analogRead(batVoltage);
    ledcWrite(4, map(bat, 230, 273, 10, 255 ) );
}
void readsliders()   // this is stepped output mode only... only multiples of snapIncrement are outputted// changed to averaged values of REadAndAverageInputs
{
 int ReadSliderOne = fader1Val;
 int ReadSliderTwo = fader2Val;
 if (ReadSliderOne %snapIncrement == 0 && ReadSliderOne != oldSliderOne)  
 {
 
    bump(1, 2, 200);
    sliderOne = ReadSliderOne;
    oldSliderOne = sliderOne;
    SendOutput();
 
  
 }
  if (ReadSliderTwo %snapIncrement == 0 && ReadSliderTwo != oldSliderTwo)
 {
 
    bump(2, 2,200);
    sliderTwo = ReadSliderTwo;
    oldSliderTwo = sliderTwo;
    SendOutput();
 
 
 }

 if (analogRead(A3)/4 == 0||analogRead(A3)/4 == 1023)  // joystick snap back at ends
 {
  snapBack(1);
 }
 if (analogRead(A2)/4 == 0||analogRead(A2)/4 == 1023)
 {
  snapBack(2);
 }

 }
 void ReadAndAverageInputs() {
  total = total - readings[readIndex];
  total2 = total2 - readings2[readIndex];
  readings[readIndex] = analogRead(A3) / 4;
  readings2[readIndex] = analogRead(A2) / 4;
  total = total + readings[readIndex];
  total2 = total2 + readings2[readIndex];
  readIndex ++;
  if (readIndex >= numReadings) {
    readIndex = 0;
  }
  fader1Val =  (total / numReadings);
  fader2Val = (total2 / numReadings);
}

 void SendOutput()  
 {
  SerialBT.print(sliderOne);
  SerialBT.print (" ");
  SerialBT.print(sliderTwo);
  SerialBT.print (" ");
  SerialBT.print(encoderValue);
  SerialBT.print (" ");
  SerialBT.print(button1);
  SerialBT.print (" ");
  SerialBT.print(button2);
  SerialBT.println("");

  
 }

 void snapBack(int slider)
 {
  int delayPeriod = 5; // in ms. increase this delay if it is sending too fast in joystick mode
  if (slider ==1){
   while ((analogRead(A3)/4) <200)
   {
    scrollOne = true;
    digitalWrite(fwd1, HIGH);
  digitalWrite(rvs1, LOW);
  bump(1, scrollSpeed, 255);
  sliderOne --;
    SendOutput();
    delay(delayPeriod ); 
   }
   
   while ((analogRead(A3)/4)> 800)
   {
    scrollOne = true;
    sliderOne ++;
     digitalWrite(fwd1, LOW);
  digitalWrite(rvs1, HIGH);
  bump(3, scrollSpeed, 255);
    SendOutput();
    delay(delayPeriod );
   }
digitalWrite(fwd1, LOW);
  digitalWrite(rvs1, LOW);
  delay(300);
   scrollOne = false;
   sliderOneOffset = sliderOne;
   
 }
 else
 {
   while ((analogRead(A2)/4) <200)
   {
    scrollTwo = true;
    sliderTwo --;
    digitalWrite(fwd2, HIGH);
  digitalWrite(rvs2, LOW);
  bump(2, scrollSpeed, 255);
    SendOutput();
    delay(delayPeriod );
   }
   while ((analogRead(A2)/4)> 800)
   {
    scrollTwo = true;
     digitalWrite(fwd2, LOW);
  digitalWrite(rvs2, HIGH);
  sliderTwo ++;
  bump(4, scrollSpeed, 255);
    SendOutput();
    delay(delayPeriod );
   }
digitalWrite(fwd2, LOW);
  digitalWrite(rvs2, LOW);
  delay(300);
   scrollTwo = false;
   sliderTwoOffset = sliderTwo;
   
  
 }
 }

void bump(int slider, int period, int pwm) // bumps for stepped mode.
{
  if (slider == 1 )
  {
   // digitalWrite(fwd1, HIGH);
   //digitalWrite(rvs1, LOW);
   ledcWrite(0, pwm);
   ledcWrite(1, 0);
   delay(period);
   ledcWrite(0, 0);
   ledcWrite(1, 0);
  // digitalWrite (fwd1, LOW);
   //digitalWrite(rvs1, LOW);
  }
  else if (slider ==2)
  {

    
    //digitalWrite(fwd2, LOW);
   //digitalWrite(rvs2, HIGH);
     ledcWrite(2, 0);
   ledcWrite(3, pwm);
   delay(period);
   ledcWrite(2, 0);
   ledcWrite(3, 0);
   digitalWrite (fwd2, LOW);
   digitalWrite(rvs2, LOW);
  }
  else if (slider ==3) // backwards 1
    {
    //digitalWrite(fwd1, LOW);
   //digitalWrite(rvs1, HIGH);
   ledcWrite(0, 0);
   ledcWrite(1, pwm);
   delay(period);
    ledcWrite(0, 0);
   ledcWrite(1, 0);
   digitalWrite (fwd1, LOW);
   digitalWrite(rvs1, LOW);
  }
    else if (slider ==4) // backwards 1
    {
    //digitalWrite(fwd2, HIGH);
   //digitalWrite(rvs2, LOW);
   ledcWrite(2, pwm);
   ledcWrite(3, 0);
   delay(period);
   ledcWrite(2, 0);
   ledcWrite(3, 0);
   digitalWrite (fwd2, LOW);
   digitalWrite(rvs2, LOW);
  }
}
void wiggle()
{
  digitalWrite(fwd1, HIGH);
  digitalWrite(rvs1, LOW);
  digitalWrite(fwd2, HIGH);
  digitalWrite(rvs2, LOW);
//  digitalWrite(led, LOW);
  delay(300);
  digitalWrite(fwd1, LOW);
  digitalWrite(rvs1, HIGH);
 digitalWrite(fwd2, LOW);
  digitalWrite(rvs2, HIGH);
  //digitalWrite(led, HIGH);
    delay(300);
    digitalWrite(fwd1, LOW);
  digitalWrite(rvs1, LOW);
  digitalWrite(fwd2, LOW);
  digitalWrite(rvs2, LOW);
 // digitalWrite(led, HIGH);
    
  
}
void easyenc() // interrupt is called on rotary encoder. We can do software debouncing too if dial is jumpy.
{
 
  int inputB = digitalRead(27); 
  
   if (inputB == LOW)
    {
      if (digitalRead(buttonSwitch)==HIGH) //Press and rotate dial to change snap increment
           {
           encoderValue++;    
           SendOutput();
           }
           else 
           {
            snapIncrement ++;
           }
    }
    else
    {
      if (digitalRead(buttonSwitch) == HIGH)
      {
         encoderValue--; 
         SendOutput();
      }
      else if (snapIncrement>=10) // set ten as the minimum increment.
      {
         snapIncrement --;
      }
    }
  
}
