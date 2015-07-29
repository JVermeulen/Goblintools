#include <EEPROM.h>
#include <RemoteReceiver.h>
#include <RemoteTransmitter.h>
#include <goblets.h>
#include <pitches.h>

#define PIN_RED 12
#define PIN_GREEN 14
#define PIN_BLUE 15

boolean enabled = true;
byte alpha = 0;
byte red = 0;
byte green = 0;
byte blue = 0;
byte buttonPressed = 0;

int ledPin = 11;
int piezoPin = 10;
int txPowerPin = 0;
int txDataPin = 1;
int rxPowerPin = 2;
int rxDataPin = 7;
int rxDataInterrupt = 2;
int buttonPin = 8;
int sclPin = 5;
int sdaPin = 6;

int value = 0;

int mario[] = {2637, 2637, 0, 2637, 0, 2093, 2637, 0,3136, 0, 0,  0, 1568};

KaKuTransmitter kaKuTransmitter(txDataPin);

const char Name[] = "RfGoblet";
const byte Features[] = {MSG_INFO, MSG_LED, MSG_BUTTON, MSG_PIEZO, MSG_RF433TX, MSG_RF433RX};

byte buffer[64]; // RawHID packets are always 64 bytes

void setup() {
  Serial.begin(9600);
  
  pinMode(ledPin, OUTPUT);  
  pinMode(PIN_RED, OUTPUT);
  pinMode(PIN_GREEN, OUTPUT);
  pinMode(PIN_BLUE, OUTPUT);
  pinMode(piezoPin, OUTPUT);
  pinMode(txPowerPin, OUTPUT);
  pinMode(txDataPin, OUTPUT);
  pinMode(rxPowerPin, OUTPUT);
  pinMode(rxDataPin, INPUT);
  pinMode(buttonPin, INPUT);
  
  RemoteReceiver::init(rxDataInterrupt, 3, input);
  
  digitalWrite(txPowerPin, HIGH);
  digitalWrite(rxPowerPin, HIGH);
  
  playMario();
}

void loop() {
  value = digitalRead(rxDataPin);
  digitalWrite(ledPin, value);
  if (value == HIGH) {
    tone(piezoPin, 4186);
  } else {
    noTone(piezoPin);
  }
  
  value = digitalRead(buttonPin);
  
  if (value != buttonPressed) {
    buttonPressed = value;
    SendButton();
  }
  
  //if (value == HIGH) {
    //kaKuTransmitter.sendSignal('M', 9, false);
    //kaKuTransmitter.sendSignal('M', 10, false);
  //}
  
  if (RawHID.recv(buffer, 0) > 0) {
    ParseBuffer();
  }
}

void ParseBuffer() {
  int msg = buffer[0] + buffer[1] * 256;
  int cmd = buffer[2];
  
  switch (msg) {
    case MSG_INFO:
      InfoHandler(cmd);
        
      break;
    case MSG_LED:
      LedHandler(cmd);
        
      break;
    case MSG_BUTTON:
      ButtonHandler(cmd);
        
      break;
    default:
      break;
    }
}

void ClearBuffer() {
  for (int i = 4 ; i < sizeof(buffer) - 1 ; i++) {
    buffer[i] = 0;
  }
}

void InfoHandler(byte cmd) {
  byte position;
  
  switch (cmd) {
    case CMD_GET:
      ClearBuffer();
      
      buffer[2] = CMD_SET;
      buffer[3] = 0; //Reserved
      
      position = 4;
      buffer[position] = sizeof(Name) - 1; //Length of String + String value
      for (int i = 0 ; i < buffer[position] ; i++) {
        buffer[i + position + 1] = Name[i];
      }
      
      position = buffer[position] + position + 1;
      buffer[position] = sizeof(Features);
      for (int i = 0 ; i < buffer[position] ; i++) {
        buffer[i + position + 1] = Features[i];
      }
      
      RawHID.send(buffer, 100);
      
      break;
    default:
      break;
  }
}

void LedHandler(byte cmd) {
  switch (cmd) {
    case CMD_DISABLE:
      enabled = false;
      
      break;
    case CMD_ENABLE:
      enabled = true;
      
      break;
    case CMD_GET:
      SendLed();
      
      break;
    case CMD_SET:
      alpha = buffer[4];
      red = buffer[5];
      green = buffer[6];
      blue = buffer[7];
      
      SendLed();
      
      break;
    case CMD_LOAD:
      alpha = EEPROM.read(4);
      red = EEPROM.read(5);
      green = EEPROM.read(6);
      blue = EEPROM.read(7);
      
      SendLed();
      
      break;
    case CMD_SAVE:
      alpha = buffer[4];
      red = buffer[5];
      green = buffer[6];
      blue = buffer[7];
      
      EEPROM.write(4, alpha);
      EEPROM.write(5, red);
      EEPROM.write(6, green);
      EEPROM.write(7, blue);
      
      SendLed();
      
      break;
    default:
      break;
  }
  
  ShowLed();
}

void ButtonHandler(byte cmd) {
  switch (cmd) {
    case CMD_GET:
      SendButton();
      
      break;
    default:
      break;
  }
  
  ShowLed();
}

void SendLed() {
  ClearBuffer();
  
  buffer[0] = MSG_LED;
  buffer[1] = 0;
  buffer[2] = CMD_SET;
  buffer[3] = 0;
  buffer[4] = alpha;
  buffer[5] = red;
  buffer[6] = green;
  buffer[7] = blue;

  RawHID.send(buffer, 100);
}

void SendButton() {
  ClearBuffer();
  
  buffer[0] = MSG_BUTTON;
  buffer[1] = 0;
  buffer[2] = CMD_SET;
  buffer[3] = 0;
  buffer[4] = buttonPressed;

  RawHID.send(buffer, 100);
}

void ShowLed() {
  if (enabled) {
    analogWrite(PIN_RED, red);
    analogWrite(PIN_GREEN, green);
    analogWrite(PIN_BLUE, blue);
  } else {
    analogWrite(PIN_RED, 0);
    analogWrite(PIN_GREEN, 0);
    analogWrite(PIN_BLUE, 0);
  }
}

void playMario() {
  int size = sizeof(mario) / sizeof(int);
     for (int thisNote = 0; thisNote < size; thisNote++) {
       int noteDuration = 1000/12; // quarter note = 1000 / 4, eighth note = 1000/8, etc.
       
       playNote(piezoPin, mario[thisNote], noteDuration);
       
       delay(noteDuration * 1.30);
       noTone(piezoPin);
     }
}

void playNote(int targetPin, long frequency, long length) {
  digitalWrite(ledPin,HIGH);

  long delayValue = 1000000 / frequency / 2;
  long numCycles = frequency * length / 1000;
  
  for (long i=0; i < numCycles; i++){
    digitalWrite(targetPin, HIGH);
    delayMicroseconds(delayValue);
    
    digitalWrite(targetPin, LOW);
    delayMicroseconds(delayValue);
  }
  
  digitalWrite(ledPin,LOW);
}

void input(unsigned long receivedCode, unsigned int period) {
  RemoteReceiver::disable();
  
  Serial.println(receivedCode);

  RemoteReceiver::enable();
}


