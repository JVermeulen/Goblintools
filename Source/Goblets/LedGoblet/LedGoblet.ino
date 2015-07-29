#include <EEPROM.h>
#include <goblets.h>

#define PIN_RED 12
#define PIN_GREEN 14
#define PIN_BLUE 15

boolean enabled = true;
byte alpha = 0;
byte red = 0;
byte green = 0;
byte blue = 0;

const char Name[] = "LedGoblet";
const byte Features[] = {MSG_INFO, MSG_LED};

byte buffer[64]; // RawHID packets are always 64 bytes

void setup() {
  pinMode(PIN_RED, OUTPUT);
  pinMode(PIN_GREEN, OUTPUT);
  pinMode(PIN_BLUE, OUTPUT);
 
  LedHandler(CMD_LOAD);
}

void loop() {
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

