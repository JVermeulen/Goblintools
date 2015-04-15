#include <EEPROM.h>

#define PIN_RED 12
#define PIN_GREEN 14
#define PIN_BLUE 15

#define MSG_INFO 0
#define MSG_LED 1
#define MSG_DHT22 2

#define CMD_DISABLE 0
#define CMD_ENABLE 1
#define CMD_GET 2
#define CMD_SET 3
#define CMD_LOAD 4
#define CMD_SAVE 5

boolean enabled = true;
byte red = 0;
byte green = 0;
byte blue = 0;

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
      case MSG_LED:
        LedHandler(cmd);
        
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
      buffer[2] = CMD_SET;
      buffer[3] = 0;
      buffer[4] = red;
      buffer[5] = green;
      buffer[6] = blue;
      
      RawHID.send(buffer, 100);
      
      break;
    case CMD_SET:
      red = buffer[4];
      green = buffer[5];
      blue = buffer[6];
      
      break;
    case CMD_LOAD:
      red = EEPROM.read(4);
      green = EEPROM.read(5);
      blue = EEPROM.read(6);
      
      break;
    case CMD_SAVE:
      red = buffer[4];
      green = buffer[5];
      blue = buffer[6];
      
      EEPROM.write(4, red);
      EEPROM.write(5, green);
      EEPROM.write(6, blue);
      
      break;
    default:
      break;
  }
  
  Show();
}

void Show() {
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

