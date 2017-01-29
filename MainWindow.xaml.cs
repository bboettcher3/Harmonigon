using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Utils;
using NAudio.Wave.SampleProviders;

namespace Harmonogon
{
    /// <summary>
    /// An interactive harmonic table made for Minnehacks 2017
    /// </summary>
    public partial class MainWindow : Window
    {

        //WaveOut waveOut;
        WaveOut[,] waves;
        float[,] freqs;
        SolidColorBrush redBrush;
        float timerSecs;
        //new sig gen
        //private SignalGenerator[,] waves;
        //private SignalGenerator wg;
        //private readonly IWavePlayer[,] drivers;
        //private readonly IWavePlayer driverOut;

        public MainWindow()
        {
            InitializeComponent();
            var sineWaveProvider = new SineWaveProvider32();
            redBrush = new SolidColorBrush(Colors.Red);
            timerSecs = 2;

            //new stuff
            // Init Audio
            //driverOut = new WaveOutEvent();
            //driverOut = new AsioOut(0);
            //waves = new SignalGenerator[5, 12];

            freqs = new float[5, 12] {
                //          C       C#      D       D#      E       F       F#      G       G#      A       A#      B
                /*2 */   {65.41f, 69.3f, 73.42f, 77.78f, 82.41f, 87.31f, 92.5f, 98, 103.8f, 110, 116.5f, 123.5f}, 
                /*3 */  {130.8f, 138.6f, 146.8f, 155.6f, 164.8f, 174.6f, 185, 196, 207.7f, 220, 233.1f, 246.9f}, 
                /*4 */  {261.6f, 277.2f, 293.7f, 311.1f, 329.6f, 349.2f, 370.0f, 392.0f, 415.3f, 440.0f, 466.2f, 493.9f},
                /*5 */  {523.3f, 554.4f, 587.3f, 622.3f, 659.3f, 698.5f, 740.0f, 784.0f, 830.6f, 880.0f, 932.3f, 987.8f},
                /*6 */  {1047f, 1109f, 1175f, 1245f, 1319f, 1397f, 1480f, 1568f, 1661f, 1760f, 1865f, 1976f}
            };
            waves = new WaveOut[5, 12];
        }

        //delay slider updated
        private void sliderChanged(object sender, RoutedEventArgs e)
        {
            Slider slider = (Slider)sender;
            timerSecs = (float)slider.Value;
            //((Label)sliderValue).Content = timerSecs;
        }

        //play button
        private void onPlay(object sender, RoutedEventArgs e)
        {
            Button ctrl = ((Button)sender);
            string tag = (string)((Button)sender).Tag;
            int octave = tag.ToCharArray()[2] - '0' - 2;
            string key = tag.Substring(0, tag.Length - 1);
            if (ctrl.Foreground != redBrush)
            {
                ctrl.Foreground = redBrush;
            }
            else
            {
                ctrl.Foreground = new SolidColorBrush(Colors.Black);
            }
            switch (key)
            {
                case "aS":
                    StartStopSineWave(freqs[octave, 10], octave, 10, sender);
                    break;
                case "a_":
                    StartStopSineWave(freqs[octave, 9], octave, 9, sender);
                    break;
                case "gS":
                    StartStopSineWave(freqs[octave, 8], octave, 8, sender);
                    break;
                case "g_":
                    StartStopSineWave(freqs[octave, 7], octave, 7, sender);
                    break;
                case "fS":
                    StartStopSineWave(freqs[octave, 6], octave, 6, sender);
                    break;
                case "f_":
                    StartStopSineWave(freqs[octave, 5], octave, 5, sender);
                    break;
                case "e_":
                    StartStopSineWave(freqs[octave, 4], octave, 4, sender);
                    break;
                case "dS":
                    StartStopSineWave(freqs[octave, 3], octave, 3, sender);
                    break;
                case "d_":
                    StartStopSineWave(freqs[octave, 2], octave, 2, sender);
                    break;
                case "cS":
                    StartStopSineWave(freqs[octave, 1], octave, 1, sender);
                    break;
                case "c_":
                    StartStopSineWave(freqs[octave, 0], octave, 0, sender);
                    break;
                case "b_":
                    StartStopSineWave(freqs[octave, 11], octave, 11, sender);
                    break;
                default:
                    Console.WriteLine("ERROR: NO NOTE FOUND");
                    break;
            }
        }

        private void StartStopSineWave(float freq, int i1, int i2, object button)
        {
            if (waves[i1, i2] == null)
            {
                //waves[i1, i2] = new SignalGenerator();
                //waves[i1, i2].Type = SignalGeneratorType.Sin;
                //waves[i1, i2].Frequency = freq;
                //drivers[i1, i2] = new WaveOutEvent();
                //drivers[i1, i2].Init(waves[i1, i2]);
                //waves[i1, i2].Init(wg);
                //waves[i1, i2].fr
                //waves[i1, i2].Play();
                var sineWaveProvider = new SineWaveProvider32();
                sineWaveProvider.SetWaveFormat(16000, 2); // 16kHz mono
                sineWaveProvider.Frequency = freq;
                sineWaveProvider.Amplitude = 0.20f;
                //sineWaveProvider.
                waves[i1, i2] = new WaveOut();
                //waves[i1, i2].NumberOfBuffers = 2;
                waves[i1, i2].Init(sineWaveProvider);
                waves[i1, i2].Play();
            }
            else
            {
                waves[i1, i2].Stop();
                waves[i1, i2].Dispose();
                waves[i1, i2] = null;
                
            }
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(timerSecs) };
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                if (waves[i1, i2] != null)
                {
                    waves[i1, i2].Stop();
                    waves[i1, i2].Dispose();
                    //drivers[i1, i2].Stop();
                    waves[i1, i2] = null;
                    Button ctrl = ((Button)button);
                    if (ctrl.Foreground == redBrush)
                    {
                        ctrl.Foreground = new SolidColorBrush(Colors.Black); ;
                    }
                }
                
            };
        }

    }



    public class SineWaveProvider32 : WaveProvider32
    {
        int sample;

        public SineWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f; // let's not hurt our ears            
        }

        public float Frequency { get; set; }
        public float Amplitude { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
                sample++;
                //if (sample >= sampleRate) sample = 0;
                if (sample >= (int)(sampleRate / Frequency)) sample = 0;﻿
            }
            return sampleCount;
        }
    }
}
