using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Assignment00._5
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int color = 0;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            color++;
            color %= 6;

            switch (color)
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
        }
    }
}
