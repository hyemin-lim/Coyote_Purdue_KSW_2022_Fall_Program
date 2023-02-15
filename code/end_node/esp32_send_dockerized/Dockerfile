FROM ubuntu:20.04

LABEL author="Seokhyeon Bang"
LABEL email="kzrt0123@cau.ac.kr"

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update  \
    && apt-get install -y --no-install-recommends \
    curl  \
    git \
    build-essential \
    ffmpeg \
    ca-certificates \
    wget \
    python3 \
    pip \
    && rm -rf /var/lib/apt/lists/*

RUN curl -O https://downloads.arduino.cc/arduino-cli/arduino-cli_0.13.0_Linux_64bit.tar.gz \
    && tar -xvzf arduino-cli_0.13.0_Linux_64bit.tar.gz \
    && mv arduino-cli /usr/local/bin/ \
    && rm -rf arduino-cli_0.13.0_Linux_64bit.tar.gz \
    && pip install pyserial


WORKDIR /Arduino/esp32_send
COPY arduino-cli.yaml ./arduino-cli.yaml

RUN arduino-cli core update-index --config-file ./arduino-cli.yaml -v\
    && arduino-cli core install esp32:esp32\
    && arduino-cli lib install "MCCI LoRaWAN LMIC Library"

COPY arduino_lmic_hal_configuration.h /root/Arduino/libraries/MCCI_LoRaWAN_LMIC_library/src/hal/arduino_lmic_hal_configuration.h