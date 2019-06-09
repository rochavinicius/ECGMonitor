using System;
using LiveCharts;
using LiveCharts.Configurations;
using System.ComponentModel;

namespace LiveChartsWPF
{
    public class ChartFactory : INotifyPropertyChanged
    {
        private double _axisMax;
        private double _axisMin;

        private ChartValues<MeasureModel> ChartValues;
        private Func<double, string> DateTimeFormatter;
        private double AxisStep;
        private double AxisUnit;
        private int NrShowingValues;

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

        public ChartFactory(ChartValues<MeasureModel> values, Func<double, string> dtFormatter,
            double step, double unit, int nrValues)
        {
            ChartValues = values;
            DateTimeFormatter = dtFormatter;
            AxisStep = step;
            AxisUnit = unit;
            NrShowingValues = nrValues;
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
            //AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            //AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

        public void AddValue(DateTime now, double trend)
        {
            ChartValues.Add(new MeasureModel
            {
                DateTime = now,
                Value = trend
            });

            SetAxisLimits(now);

            //lets only use the last 80 values
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
    }
}
