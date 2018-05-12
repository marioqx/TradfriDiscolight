# TradfriDiscolight

This program captures the level of one selected sound output device from Windows and sends a respective level-dependent color or dimmer setting via the IKEA Tradfri gateway to the selected bulb or group of tradfri-devices.

This way it is possible to create a discolight or partylight with the IKEA lighting devices from audio playback on a windows PC. The program hooks into the sound output device and is not dependent on the used player for playback.

# Compiling

This project was created with MS Visual Studio 2017. Open the project in Visual Studio and add the respositories NAudio and CsharpTradfri using the Nuget package manager from the menu.

The CSharpTradfri-library in its current verion 0.3.0.x needs to be patched. You need to add:
```
        public override string ToString()
        {
            return Name;
        }
```
to TradFriDevice-class defined in TradFriDevice.cs file and TradFriGroup-class defined in TradFriGroup.cs file. For the current Tradfri-Library further patches are required, thus a pre-compiled version is included for download. This will be changed once the changes to the TradfriLibrary are accepted and included.

# Usage

The upper panel lists in the drop-down menu the available audio output devices on the windows computer. Select the audio output device you want to listen to and start listening by pressing the "start" button. The bar shall show the level and the box below the button shall show the associated color, if sound is output through the selected audio output device.

The lower panel allows to input the settings for the tradfri-gateway. These are the Adress of the gateway in your local network, a connection identifier string and a connection specific key. Do not enter the secret key printed on the bottom of gateway. The Adress of the gateway can bei either an IP-adress or the DNS-name. The connection identifier is a string and can be arbitarily chosen. The Connection Pre-shared Key can be created by pressing the "GenKey"-button, just follow the instructions shown in the dialog. The settings entered will be stored on closing and loaded on next program start.
Once the IP-adress and connection pre-shared key is entered, press Connect. If connecting to the gateway was successful, the dropdown-list shows all devices and groups registered with the gateway. Choose the device or group you want to re-direct the VU-level information and the bulb/group will change color or dimmer-level synchronous with the sound played back on the windows PC.

Please note that changing the dimmer-level has some delay, as the "Transition-time" needs to be reduced to 0, what is not yet implemented.

# Disclaimer

Please note that switching the light on/off or changing the color in rapid succession may harm the life-expectancy of the bulb. I have not experienced any issue so far, but you have to be aware that the bulb may be damaged by running this application. Therefore, use this program at your own risk.
The gateway may get very warm, because many updates have to be transmitted to update the buld/group. It happened during development, that a 20ms update cycle is too fast, also a 40ms cycle. The gateway creashed and thus the code was designed to lower the update-rate.

# Acknowledgements

This program builds on CSharpTradfri-library from [tomidix](https://github.com/tomidix/CSharpTradFriLibrary) and [NAudio](https://github.com/naudio/NAudio"). My particular thanks to tomidix, for prompt changing and adapting his library according my requests!
