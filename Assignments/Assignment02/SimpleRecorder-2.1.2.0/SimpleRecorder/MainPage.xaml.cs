using CaptureEncoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Hosting;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Media.SpeechSynthesis;

namespace SimpleRecorder
{
    public sealed partial class MainPage : Page
    {
        private int cycle = 0;
        private SpeechSynthesizer synth;
        private List<OutputItem> outputItems;

        public MainPage()
        {
            InitializeComponent();
            
            ApplicationView.GetForCurrentView().SetPreferredMinSize(
               new Size(539, 285));

            if (!GraphicsCaptureSession.IsSupported())
            {
                IsEnabled = false;

                var dialog = new MessageDialog(
                    "Screen capture is not supported on this device for this release of Windows!",
                    "Screen capture unsupported");

                var ignored = dialog.ShowAsync();
                return;
            }

            var compositor = Window.Current.Compositor;
            _previewBrush = compositor.CreateSurfaceBrush();
            _previewBrush.Stretch = CompositionStretch.Uniform;
            var shadow = compositor.CreateDropShadow();
            shadow.Mask = _previewBrush;
            _previewVisual = compositor.CreateSpriteVisual();
            _previewVisual.RelativeSizeAdjustment = Vector2.One;
            _previewVisual.Brush = _previewBrush;
            _previewVisual.Shadow = shadow;
            ElementCompositionPreview.SetElementChildVisual(CapturePreviewGrid, _previewVisual);

            _device = D3DDeviceManager.Device;

            outputItems = new List<OutputItem>();
            outputItems.Add(new OutputItem()
            {
                DisplayName = "Visual Mode",
                DisplayStyle = 0
            });
            outputItems.Add(new OutputItem()
            {
                DisplayName = "Colorblind Mode",
                DisplayStyle = 1
            });
            outputItems.Add(new OutputItem()
            {
                DisplayName = "Audio Mode",
                DisplayStyle = 2
            });
            optionsBox.ItemsSource = outputItems;
            optionsBox.SelectedIndex = GetStartingIndex();

            // The object for controlling the speech synthesis engine (voice).
            synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
        }
        private int GetStartingIndex()
        {
            ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return (int) settings.Containers["Options"].Values["Output"];
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new GraphicsCapturePicker();
            var item = await picker.PickSingleItemAsync();
            if (item != null)
            {
                StartPreview(item);
            }
            else
            {
                StopPreview();
            }
        }

        private void StartPreview(GraphicsCaptureItem item)
        {
            PreviewContainerGrid.RowDefinitions[1].Height = new GridLength(2, GridUnitType.Star);
            CapturePreviewGrid.Visibility = Visibility.Visible;
            CaptureInfoTextBlock.Text = item.DisplayName;

            var compositor = Window.Current.Compositor;
            _preview?.Dispose();
            _preview = new CapturePreview(_device, item);
            var surface = _preview.CreateSurface(compositor);
            _previewBrush.Surface = surface;
            _preview.StartCapture();
        }

        private void StopPreview()
        {
            PreviewContainerGrid.RowDefinitions[1].Height = new GridLength(0);
            CapturePreviewGrid.Visibility = Visibility.Collapsed;
            CaptureInfoTextBlock.Text = "Pick something to capture";
            _preview?.Dispose();
            _preview = null;

        }

        private static T ParseEnumValue<T>(string input)
        {
            return (T)Enum.Parse(typeof(T), input, false);
        }

        private IDirect3DDevice _device;

        private CapturePreview _preview;
        private SpriteVisual _previewVisual;
        private CompositionSurfaceBrush _previewBrush;

        private async void cycleButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            int outputType = optionsBox.SelectedIndex;

            // if the option has changed, update the settings
            if (optionsBox.SelectedIndex != (int) settings.Containers["Options"].Values["Output"])
            {
                settings.Containers["Options"].Values["Output"] = optionsBox.SelectedIndex;
            }
            cycle++;
            cycle %= 6;

            switch (outputType)
            {
                case 0: // visual output
                    switch (cycle)
                    {
                        case 0:
                            colorBox.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Black);
                            break;
                        case 1:
                            colorBox.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Red);
                            break;
                        case 2:
                            colorBox.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Orange);
                            break;
                        case 3:
                            colorBox.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Yellow);
                            break;
                        case 4:
                            colorBox.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Blue);
                            break;
                        case 5:
                            colorBox.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Indigo);
                            break;
                    }
                    break;
                case 1: // visual output (colorblind)

                    break;
                case 2: // audio output
                    switch (cycle)
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
                    break;
            }
        }
    }
}
