using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using LiveCharts;
using LiveCharts.Configurations;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsWPF
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SerialPort _serialPort;
        DispatcherTimer timer;
        private double _trend;
        private int cont = 0;
        public bool IsReading { get; set; }

        Color color;
        bool Disconnect = false;

        public MainWindow()
        {
            InitializeComponent();
            _serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
            _serialPort.ReadTimeout = 2000;
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            ConfigureChart();
            
            IsReading = false;

            DataContext = this;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            int i = 0;
            bool quantDiferente = false; //flag para sinalizar que a quantidade de portas mudou

            /*try
            {
                if (color != null) Series.Stroke = new SolidColorBrush(color);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }*/
            if (Disconnect)
            {
                BtnConnect.Content = "Connect";
            }

            //se a quantidade de portas mudou
            if (ComboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (!ComboBox1.Items[i++].Equals(s))
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false && SerialPort.GetPortNames().Length != 0)
            {
                return;                     //retorna
            }

            //limpa comboBox
            ComboBox1.Items.Clear();

            if (SerialPort.GetPortNames().Length == 0)
            {
                ComboBox1.Items.Add("No COM Ports Available");
            }
            else
            {
                //adiciona todas as COM diponíveis na lista
                foreach (string s in SerialPort.GetPortNames())
                {
                    ComboBox1.Items.Add(s);
                }
            }

            //seleciona a primeira posição da lista
            ComboBox1.SelectedIndex = 0;
            
        }

        private void Read()
        {
            var r = new Random();

            while (IsReading)
            {
                try
                {
                    var now = DateTime.Now;
                    /*Thread.Sleep(150);
                    cont += 1;

                    if (cont == 10)
                    {
                        cont = 0;
                        _trend = r.Next(-5, 3);
                    }

                    AddChartValue(now, _trend);*/


                    /* COM PORT READ */
                    if (!_serialPort.IsOpen)
                        _serialPort.Open();
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(50);

                    byte[] byteBuffer = new byte[4];
                    _serialPort.Read(byteBuffer, 0, 4);
                    var stringValue = Encoding.ASCII.GetString(byteBuffer);
                    double value = Convert.ToDouble(stringValue.Replace('.', ','));

                    if (value > 3500.0d) value = 3500.0d;
                    if (value < 500.0d) value = 500.0d;
                    if (value < 2.0d)
                    {
                        color = new Color()
                        {
                            R = 0xF3,
                            G = 0x43,
                            B = 0x36,
                            A = 1
                        };
                        //Series.Stroke = new SolidColorBrush(color);
                        value = 2048.0d;
                    }
                    else
                    {
                        color = new Color()
                        {
                            R = 0x1E,
                            G = 0x90,
                            B = 0xFF,
                            A = 1
                        };
                        //Series.Stroke = new SolidColorBrush(color);
                    }

                    _serialPort.Close();

                    AddChartValue(now, value);
                    /*****************/
                }
                catch(Exception ex)
                {
                    _serialPort.Close();
                    Disconnect = true;
                    
                    IsReading = false;
                    //ConnectClick(null, new RoutedEventArgs());
                }
            }
        }

        bool isopen = false;
        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            var portName = ComboBox1.Items[ComboBox1.SelectedIndex].ToString();
            if (portName.Equals("No COM Ports Available"))
            {
                MessageBox.Show("There are no COM ports available to connect.", "Error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (!_serialPort.IsOpen)
            //if (!isopen)
            {
                ComboBox1.IsEnabled = false;
                _serialPort.PortName = portName;
                _serialPort.Open();
                IsReading = true;
                isopen = true;
                Task.Factory.StartNew(Read);
            }
            else
            {
                IsReading = false;
                isopen = false;
                _serialPort.Close();
                ComboBox1.IsEnabled = true;
            }
            BtnConnect.Content = _serialPort.IsOpen ? "Disconnect" : "Connect";
            //BtnConnect.Content = isopen ? "Disconnect" : "Connect";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
                _serialPort.Close();
        }

        #region Chart

        private const int NrShowingValues = 150;
        private double _axisMax;
        private double _axisMin;

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public void ConfigureChart()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<MeasureModel>();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

        public void AddChartValue(DateTime now, double trend)
        {
            ChartValues.Add(new MeasureModel
            {
                DateTime = now,
                Value = trend
            });

            SetAxisLimits(now);

            //lets only use the last N values
            if (ChartValues.Count > NrShowingValues) ChartValues.RemoveAt(0);
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}
