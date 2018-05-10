# TradfriDiscolight
This program captures the level of one selected sound output device from Windows and sends a respective level-dependent color or dimmer setting via the IKEA Tradfri gateway to the selected bulb or group of tradfri-devices.

This way it is possible to create a discolight or partylight with the IKEA lighting devices from audio playback on a windows PC.

# Compiling

This project was created with MS Visual Studio 2017. Open the project in Visual Studio and add the respositories NAudio and CsharpTradfri using the Nuget package manager form the menu.

The CSharpTradfri-library in its current verion 0.3.0.0 needs to be patched. You need to add:
```
        public override string ToString()
        {
            return Name;
        }
```
to TradFriDevice-class defined in TradFriDevice.cs file and TradFriGroup-class defined in TradFriGroup.cs file.

# Usage

The upper panel lists in the drop-down menu the available audio output devices on the windows computer. Select the audio output device you want to listen to and start listening by pressing the "start" button. The bar shall show the level and the box below the button shall show the associated color, if sound is output through the selected audio output device.

The lower panel allows to input the settings for the tradfri-gateway. This are the IP-adress of the gateway in your local network, a connection identifier string and the secret key printed on the bottom of your gateway. At the moment only the IP-adress and the gateway-secret is used, the connection identifier is not yet used due to CSharpTradfri-library constraints. The settings entered will be stored on closing and loaded on next program start.
Once the IP-adress and gateway secret is entered, press Connect. Please note, that again due to constraints from CSharpTradfri-library the program hangs if it cannot successfully connect the gateway. If connecting the gateway was successful, the dropdown-list shows all devices and groups registered with the gateway. Choose the device or group you want to re-direct the VU-level information and the bulb/group will flicker with the sound level played back on the windows PC.


# Disclaimer

Please note that switching the light on/off or changing the color in rapid succession may harm the life-expectancy of the bulb. I have not experienced any issue so far, but you have to be aware that the bulb may be damaged by running this application. Therefore, use this program at your own risk.
The gateway may get very warm, because many updates have to be transmitted to update the buld/group. It happened during development, that a 20ms update cycle is too fast, also a 40ms cycle. The gateway creashed and thus the code was designed to lower the update-rate.

# Acknowledgements

This program builds on CSharpTradfri-library from [tomidix](https://github.com/tomidix/CSharpTradFriLibrary) and [NAudio](https://github.com/naudio/NAudio").
