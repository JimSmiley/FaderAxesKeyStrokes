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

void setup() {
 Serial.begin(115200);  // for serial coms with cable only... Helpful for troubleshooting without requiring changing serial port to bluetooth. Make sure baud rate is set in the ide as well. just need Serial.print(""); or Serial.println("");
 SerialBT.begin("RedAxis1"); //name your axis here - is what comes up in bluetooth devices. Bt serial does not need a baud rate.
ledcSetup (4, pwmFreq, 8); // sets up pwm 
ledcAttachPin(batOut, 4); //
//pinMode(led, OUTPUT);  // more pin setups
pinMode(buttonSwitch, INPUT_PULLUP);
pinMode(topSwitch, INPUT_PULLUP);
//digitalWrite(led, HIGH);
analogReadResolution(10); //  can use highher analog read resolutions but values will get very shaky. Sid has a method that averages multiple reads for better accuracy
 
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
void readsliders()   // this is stepped output mode only... only multiples of 40 are outputted.
{
 int ReadSliderOne = analogRead(A3);
 int ReadSliderTwo = analogRead(A2);
 if (ReadSliderOne %40 == 0 && ReadSliderOne != oldSliderOne)  
 {
 
    bump(1, 2);
    sliderOne = ReadSliderOne;
    oldSliderOne = sliderOne;
    SendOutput();
 
  
 }
  if (ReadSliderTwo %40 == 0 && ReadSliderTwo != oldSliderTwo)
 {
 
    bump(2, 2);
    sliderTwo = ReadSliderTwo;
    oldSliderTwo = sliderTwo;
    SendOutput();
 
 
 }

 if (ReadSliderOne == 0||ReadSliderOne == 1023)  // joytick snap back at ends
 {
  snapBack(1);
 }
 if (ReadSliderTwo == 0||ReadSliderTwo == 1023)
 {
  snapBack(2);
 }

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
  int delayPeriod = 1; // in ms. increase this delay if it is sending too fast in joystick mode
  if (slider ==1){
   while (analogRead(A3) <100)
   {
    scrollOne = true;
    digitalWrite(fwd1, HIGH);
  digitalWrite(rvs1, LOW);
  bump(1, 30);
  sliderOne --;
    SendOutput();
    delay(delayPeriod ); 
   }
   while (analogRead(A3)> 925)
   {
    scrollOne = true;
    sliderOne ++;
     digitalWrite(fwd1, LOW);
  digitalWrite(rvs1, HIGH);
  bump(3, 30);
    SendOutput();
    delay(delayPeriod );
   }
digitalWrite(fwd1, LOW);
  digitalWrite(rvs1, LOW);
  
   
 }
 else
 {
   while (analogRead(A2) <100)
   {
    scrollTwo = true;
    sliderTwo --;
    digitalWrite(fwd2, HIGH);
  digitalWrite(rvs2, LOW);
  bump(2, 30);
    SendOutput();
    delay(delayPeriod );
   }
   while (analogRead(A2)> 925)
   {
    scrollTwo = true;
     digitalWrite(fwd2, LOW);
  digitalWrite(rvs2, HIGH);
  sliderTwo ++;
  bump(4, 30);
    SendOutput();
    delay(delayPeriod );
   }
digitalWrite(fwd2, LOW);
  digitalWrite(rvs2, LOW);
   
  
 }
 }

void bump(int slider, int period) // bumps for stepped mode.
{
  if (slider == 1 )
  {
    digitalWrite(fwd1, HIGH);
   digitalWrite(rvs1, LOW);
   delay(period);
   digitalWrite (fwd1, LOW);
   digitalWrite(rvs1, LOW);
  }
  else if (slider ==2)
  {
    digitalWrite(fwd2, LOW);
   digitalWrite(rvs2, HIGH);
   delay(period);
   digitalWrite (fwd2, LOW);
   digitalWrite(rvs2, LOW);
  }
  else if (slider ==3) // backwards 1
    {
    digitalWrite(fwd1, LOW);
   digitalWrite(rvs1, HIGH);
   delay(period);
   digitalWrite (fwd1, LOW);
   digitalWrite(rvs1, LOW);
  }
    else if (slider ==4) // backwards 1
    {
    digitalWrite(fwd2, HIGH);
   digitalWrite(rvs2, LOW);
   delay(period);
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
      encoderValue++;  
    
      
        SendOutput();
    }
    else
    {
      encoderValue--; 
    
      
        SendOutput();
    }
  
}
