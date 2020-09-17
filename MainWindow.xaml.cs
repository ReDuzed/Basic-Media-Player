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

using NAudio;
using NAudio.Wave;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudioFileReader read;
        private WaveOut wave;
        private bool change;
        private bool mouseEnter;
        public MainWindow()
        {
            InitializeComponent();
            Slider();
        }
        private void Slider()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        slider_position.Dispatcher.Invoke(() =>
                        {
                            if (read != null && !mouseEnter)
                            {
                                double value = (double)read.Position / read.WaveFormat.AverageBytesPerSecond;
                                slider_position.Value = value;
                            }
                        });
                    }
                    catch
                    {
                        return;
                    }
                }
            }); 
        }
        private void On_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled || e.IsRepeat)
                return;
            if (e.Key == Key.Space)
            {
                if (wave.PlaybackState == PlaybackState.Paused)
                    wave.Play();
                else wave.Pause();
                return;
            }
            if (e.Key == Key.Left && slider_position.Value >= 5)
            {
                change = true;
                if (slider_position.Value <= 5)
                {
                    slider_position.Value = 0;
                }
                slider_position.Value -= 5;
            }
            if (e.Key == Key.Right && slider_position.Value <= slider_position.Maximum)
            {
                change = true;
                if (slider_position.Value >= slider_position.Maximum - 5)
                {
                    slider_position.Value = slider_position.Maximum;
                }
                slider_position.Value += 5;
            }
        }
        private void button_open_Click(object sender, RoutedEventArgs e)
        {
            file_dialog.ShowDialog(this);
            read = new AudioFileReader(file_dialog.FileName);
            wave = new WaveOut();
            wave.Init(read);
            label_info.Content = (int)read.TotalTime.TotalSeconds;
            slider_position.Maximum = read.TotalTime.TotalSeconds;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wave.Play();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            wave.Stop();
        }

        private void On_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (read != null && (mouseEnter || change))
            {
                read.Position = (long)slider_position.Value * read.WaveFormat.AverageBytesPerSecond;
                change = false;
            }
            label_info.Content = string.Format("{0} / {1}", (int)read.Position / read.WaveFormat.AverageBytesPerSecond, (int)read.TotalTime.TotalSeconds);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (read != null && int.TryParse(textbox_position.Text, out int position) && position > 0 && position < read.TotalTime.TotalSeconds)
            {
                read.Position = position * read.WaveFormat.AverageBytesPerSecond;
            }
            textbox_position.Focusable = false;
        }

        private void textbox_position_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textbox_position.Focusable = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pressing left and right arrow cause seeking 5 seconds before or after current placement.\n\nHitting the space bar will pause the audio. Hitting it again will continue it.\n\nThe text shows the audio in durations of seconds. Input whatever second interval, then hit Go, to seek to that placement.");
        }

        private void slider_position_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseEnter = true;
        }

        private void slider_position_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseEnter = false;
        }
    }
}
