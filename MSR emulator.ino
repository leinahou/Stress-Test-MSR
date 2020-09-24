int PWM1 = 10;  //PWMA
int DIR1 = 9;  //DIRA
int PWM2 = 11;  //Not used
int DIR2 = 13;  //Not used

 

int clock_speed = 300;
int clock_half = 0;

 

char serial_line[37]= "371449635398431=1812941116203";
int serial_line_length;

 

void setup() {
  
  pinMode(PWM1, OUTPUT);
  pinMode(DIR1, OUTPUT);
  Serial.begin(9600);
  //Serial.setTimeout(1000);
  Serial.println("READY");
  Serial.flush();
}

 

void writeBit(int lowOrHigh) {
  if (lowOrHigh == 1) {
    digitalWrite(DIR1, ((clock_half == 0) ? HIGH : LOW));
    delayMicroseconds(clock_speed);
    digitalWrite(DIR1, ((clock_half == 0) ? LOW : HIGH));
    delayMicroseconds(clock_speed);
  } else {
    digitalWrite(DIR1, ((clock_half == 0) ? HIGH : LOW));
    delayMicroseconds(clock_speed * 2);
    clock_half = (clock_half + 1) % 2;
  }
}

 

void writeChar(char c) {
  byte t = 0x00;
  switch (c) {
    case '0': t = 0x01; break;
    case '1': t = 0x10; break;
    case '2': t = 0x08; break;
    case '3': t = 0x19; break;
    case '4': t = 0x04; break;
    case '5': t = 0x15; break;
    case '6': t = 0x0d; break;
    case '7': t = 0x1c; break;
    case '8': t = 0x02; break;
    case '9': t = 0x13; break;
    case ':': t = 0x0b; break;
    case ';': t = 0x1a; break;
    case '<': t = 0x07; break;
    case '=': t = 0x16; break;
    case '>': t = 0x0e; break;
    case '?': t = 0x1f; break;
  }
  for (int i = 4; i >= 0; i--) {
    writeBit((t >> i) & 0x01);
  }
}

 

int LRC(char* data, int length) {
  int lrc = 0;
  for (int i = 0; i < length; i++) {
    lrc ^= (data[i] - 0x30);
  }
  return lrc + 0x30;
}

 

void emulator() {
  int track2_len = serial_line_length + 2;
  char track2[track2_len]=";"; //Not include LRC
    for (int i = 0; i < serial_line_length; i++) {
      Serial.println(serial_line[i]);
      track2[i + 1] = serial_line[i];   
    }
  track2[track2_len - 1] = '?';
  Serial.println(track2);
  digitalWrite(PWM1, HIGH);
  for (int i = 0; i < 20; i ++) {
    writeBit(0);
  }
  for (int i = 0; i < track2_len; i++) {
    writeChar(track2[i]);
  }
  writeChar(LRC(track2, track2_len));
  for (int i = 0; i < 20; i ++) {
    writeBit(0);
  }
  digitalWrite(PWM1, LOW);
  delay(clock_speed);
}

 

void loop() {

 
while (Serial.available()){
  if (Serial.available() > 0) {
    //serial_line[] = 371449635398431=1812941116203;
    serial_line_length = Serial.readBytesUntil('$', serial_line, 38);
    Serial.println("---- serial_line_length_size ----");
    Serial.println(serial_line_length);
    Serial.println("------");
    Serial.flush();
    emulator();
    Serial.println("DONE");
  }
}
}
