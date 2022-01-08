using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Assignment01._5
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int sound = 0;
        private SpeechSynthesizer synth;
        public MainPage()
        {
            this.InitializeComponent();
            
            // The object for controlling the speech synthesis engine (voice).
            synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

        }

        private async void soundButton_Click(object sender, RoutedEventArgs e)
        {
            sound++;
            sound %= 6;

            switch (sound)
            {
                case 0:
                    // Generate the audio stream from plain text.
                    SpeechSynthesisStream stream0 = await synth.SynthesizeTextToStreamAsync("Zero");

                    // Send the stream to the media object.
                    mediaElement.SetSource(stream0, stream0.ContentType);
                    mediaElement.Play(); 
                    break;
                case 1:
                    // Generate the audio stream from plain text.
                    SpeechSynthesisStream stream1 = await synth.SynthesizeTextToStreamAsync("One");

                    // Send the stream to the media object.
                    mediaElement.SetSource(stream1, stream1.ContentType);
                    mediaElement.Play();
                    break;
                case 2:
                    // Generate the audio stream from plain text.
                    SpeechSynthesisStream stream2 = await synth.SynthesizeTextToStreamAsync("Two");

                    // Send the stream to the media object.
                    mediaElement.SetSource(stream2, stream2.ContentType);
                    mediaElement.Play();
                    break;
                case 3:
                    // Generate the audio stream from plain text.
                    SpeechSynthesisStream stream3 = await synth.SynthesizeTextToStreamAsync("Three");

                    // Send the stream to the media object.
                    mediaElement.SetSource(stream3, stream3.ContentType);
                    mediaElement.Play();
                    break;
                case 4:
                    // Generate the audio stream from plain text.
                    SpeechSynthesisStream stream4 = await synth.SynthesizeTextToStreamAsync("Four");

                    // Send the stream to the media object.
                    mediaElement.SetSource(stream4, stream4.ContentType);
                    mediaElement.Play();
                    break;
                case 5:
                    // Generate the audio stream from plain text.
                    SpeechSynthesisStream stream5 = await synth.SynthesizeTextToStreamAsync("Five");

                    // Send the stream to the media object.
                    mediaElement.SetSource(stream5, stream5.ContentType);
                    mediaElement.Play();
                    break;
            }
        }
    }
}
