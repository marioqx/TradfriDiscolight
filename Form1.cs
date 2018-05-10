using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NAudio.CoreAudioApi;
using NAudio.Wave;

using Tomidix.CSharpTradFriLibrary;
using Tomidix.CSharpTradFriLibrary.Controllers;
using Tomidix.CSharpTradFriLibrary.Models;

namespace VUmeter
{
    public partial class Form1 : Form
    {
        private double level = 0;

        private TradFriCoapConnector gatewayConnection = null;
        private DeviceController dc = null;
        private GroupController gc = null;

        private const double FILTER_CONSTANT = 0.1;

        Color[] colors = new Color[] { Color.FromArgb(0x4a, 0x41, 0x8a ),
                                      Color.FromArgb( 0x8f, 0x26, 0x86 ),
                                      Color.FromArgb( 0xdc, 0x4b, 0x31 ),
                                      Color.FromArgb( 0xda, 0x5d, 0x41 ),
                                      Color.FromArgb( 0x6c, 0x83, 0xba ),
                                      Color.FromArgb( 0xd9, 0x33, 0x7c ),
                                      Color.FromArgb( 0xe5, 0x73, 0x45 ),
                                      Color.FromArgb( 0xe7, 0x88, 0x34 ),
                                      Color.FromArgb( 0xa9, 0xd6, 0x2b ),
                                      Color.FromArgb( 0xeb, 0xb6, 0x3e ),
                                      Color.FromArgb( 0xc9, 0x84, 0xbb ),
                                      Color.FromArgb( 0xd6, 0xe4, 0x4b ),
                                      Color.FromArgb( 0xe4, 0x91, 0xaf ),
                                      Color.FromArgb( 0xef, 0xd2, 0x75 ),
                                      Color.FromArgb( 0xe8, 0xbe, 0xdd ),
                                      Color.FromArgb( 0xf1, 0xe0, 0xb5 ),
                                      Color.FromArgb( 0xf2, 0xec, 0xcf ),
                                      Color.FromArgb( 0xdc, 0xf0, 0xf8 ),
                                      Color.FromArgb( 0xea, 0xf6, 0xfb ),
                                      Color.FromArgb( 0xf5, 0xfa, 0xf6 ) };
        string[] colorStr = new string[] {  "Blue",
                                            "Light Blue",
                                            "Saturated Purple",
                                            "Lime",
                                            "Light Purple",
                                            "Yellow",
                                            "Saturated Pink",
                                            "Dark Peach",
                                            "Saturated Red",
                                            "Cold sky",
                                            "Pink",
                                            "Peach",
                                            "Warm Amber",
                                            "Light Pink",
                                            "Cool daylight",
                                            "Candlelight",
                                            "Warm glow",
                                            "Warm white",
                                            "Sunrise",
                                            "Cool white"
                                        };
        string[] colorHex = new string[] {  "4a418a",
                                            "8f2686",
                                            "dc4b31",
                                            "da5d41",
                                            "6c83ba",
                                            "d9337c",
                                            "e57345",
                                            "e78834",
                                            "a9d62b",
                                            "ebb63e",
                                            "c984bb",
                                            "d6e44b",
                                            "e491af",
                                            "efd275",
                                            "e8bedd",
                                            "f1e0b5",
                                            "f2eccf",
                                            "dcf0f8",
                                            "eaf6fb",
                                            "f5faf6"
                                        };


    public Form1()
        {
            InitializeComponent();

            // Fill the Audio-devices selection
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var audiodevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            comboBox1.Items.AddRange( audiodevices.ToArray() );

            // Fill the settings for the tradfri-gateway-connection
            textBox3.Text = Properties.Settings.Default.GatewayIP;
            textBox1.Text = Properties.Settings.Default.ConnectionName;
            textBox2.Text = Properties.Settings.Default.GatewaySecret;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            bool SaveSettings = ( Properties.Settings.Default.GatewayIP.Equals(textBox3.Text) ) &
                                ( Properties.Settings.Default.ConnectionName.Equals(textBox1.Text)) &
                                ( Properties.Settings.Default.GatewaySecret.Equals(textBox2.Text));

            if ( !SaveSettings )
            {
                Properties.Settings.Default.GatewayIP = textBox3.Text;
                Properties.Settings.Default.ConnectionName = textBox1.Text;
                Properties.Settings.Default.GatewaySecret = textBox2.Text;
                Properties.Settings.Default.Save();
            }
        }


        private void LoadAllDevices()
        {
            GatewayController gwc = new GatewayController(gatewayConnection.Client);

            comboBox2.Enabled = false;
            comboBox2.BeginUpdate();
            comboBox2.Items.Clear();

            // Read the devices
            foreach ( long deviceID in gwc.GetDevices() )
            {
                DeviceController dcl = new DeviceController(deviceID, gatewayConnection.Client);
                TradFriDevice device = dcl.GetTradFriDevice();
                comboBox2.Items.Add( device );
            }

            // Read the groups
            foreach (long groupID in gwc.GetGroups())
            {
                GroupController gcl = new GroupController(groupID, gatewayConnection.Client);
                TradFriGroup currentGroup = gcl.GetTradFriGroup();
                comboBox2.Items.Add( currentGroup );
            }

            comboBox2.SelectedIndex = -1;
            comboBox2.EndUpdate();
            comboBox2.Enabled = true;
        }


        private void ConnectTradfri()
        {
            try
            {
                if (gatewayConnection == null )
                {
                    gatewayConnection = new TradFriCoapConnector( textBox1.Text, textBox3.Text, textBox2.Text);
                    gatewayConnection.Connect();
                }

                // Query all devices and list in the comboBox.
                LoadAllDevices();
            }
            catch (Exception)
            {
                MessageBox.Show("Couldn't connect to TradFri gateway with provided settings");
            }

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                bool update = false;
                var device = (MMDevice)comboBox1.SelectedItem;
                double index = Math.Round(device.AudioMeterInformation.MasterPeakValue * (colors.Length - 1));

                // Update graphic VU-Meter in panel.
                progressBar1.Value = (int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100));

                // Upodate the panel-background color and the color of the bulb, such that they change
                // consistently and at a much lower rate than the VU-meter shown in progressBar1.
                // This is necessary, because a too high update-rate crashes the tradfri-gateway and further
                // may limit live-expectancy of the bulb.
                if ((index - level >= 3) || (level - index >= 5))
                {
                    level = index;
                    update = true;
                }
                else
                {
                    int old_level = (int)level;
                    level = (1 - FILTER_CONSTANT) * level + FILTER_CONSTANT * index;
                    if (old_level != (int)level) update = true;
                }

                // If required, then update the Tradfri-bulb color/dimmer and correspondigly the backcolor of the panel.
                // For the individual buld, change the color. For the group change the dimmer.
                if (update)
                {
                    if (dc != null)
                    {
                        panel1.BackColor = colors[(int)level];
                        dc.SetColor(colorHex[(int)level]);
                    }
                    else if (gc != null)
                    {
                        int dimmer = (int)(level * 256 / colors.Length);
                        gc.SetDimmer(dimmer);
                        panel1.BackColor = Color.FromArgb(dimmer, dimmer, dimmer);
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if ( button1.Text == "Start")
            {
                button1.Text = "Stop";
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
                level = 0;
                progressBar1.Value = 0;
                button1.Text = "Start";
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Connecting";
            button2.Enabled = false;
            button2.Update();
            ConnectTradfri();       // Note: This call hangs in case settings do not allow for successfully connection to the gateway.
            button2.Text = "Connect";
            button2.Enabled = true;
            button2.Update();
        }


        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if ( comboBox2.SelectedIndex >= 0 )
            {
                if ( comboBox2.SelectedItem.GetType() == typeof( TradFriDevice ))
                {
                    var deviceToChangeProperties = (TradFriDevice) comboBox2.SelectedItem;
                    dc = new DeviceController(deviceToChangeProperties.ID, gatewayConnection.Client);
                    gc = null;
                }
                else if (comboBox2.SelectedItem.GetType() == typeof( TradFriGroup ))
                {
                    var deviceToChangeProperties = (TradFriGroup) comboBox2.SelectedItem;
                    gc = new GroupController(deviceToChangeProperties.ID, gatewayConnection.Client);
                    dc = null;
                }
                else
                {
                    dc = null;
                    gc = null;
                }
            }
        }
    }
}
