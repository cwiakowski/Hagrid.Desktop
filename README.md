# Hagrid

WPF Application for communication with devices via COM port.


### Example testing code

Simple C code that communicates via Serial Port

```
String inputString = "";
boolean stringComplete = false;

void setup() {
        Serial.begin(9600);
}

void loop() {
  if(Serial.available() > 0)
  {
    while (Serial.available() > 0) {
      char inChar = (char)Serial.read();
      if(inChar == '\n') {
        stringComplete = true;
      } 
      else {
        inputString += inChar;            
      }
    }
    if(stringComplete) {
      Serial.print("I received: ");
      Serial.println(inputString);
      inputString = ""; 
      stringComplete = false;
    }
  }
}
 
```
