   M S B A M L   `   `   `  ����U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf  5   	   H DLiveChartsWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:LiveChartsWPFLiveChartsWPF 5   	      LiveChartsWPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	   -localclr-namespace:LiveChartsWPF     5   	   -   5          � Title$  
MainWindow��5	   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)   .+�5   
   �� -   5        $��	ComboBox1��5   N   
����  �$��1��6)   $��25��66   $	��170��6B   $��200,0,460,400q�6_   �� 5   
   -   5        $��
BtnConnect��5      
�������$��1��6:   $��25��6G   $	��170��6S   $��460,0,200,400q�6t   .��6�   	Connect5      #   LiveCharts.Wpf.CartesianChart  5   
   $��1��6   &    LiveCharts.Wpf.Charts.Base.Chart   AnimationsSpeed$ 0:0:0.2h�6*      	Hoverable .  $��	0,60,0,20q�6m      DataTooltip 6V   Z�    Series 5         LiveCharts.Wpf.LineSeries  -   5        $��Series��5   !      LineSmoothness$ 1Q�5   !      LiveCharts.Wpf.Series   StrokeThickness$ 6Q�5   !      Stroke �#F34336	   Fill
	 ���� 
   Values
 5   /   �� *ChartValues+   PointGeometry 5   !   Z� 5   !      AxisY 5         LiveCharts.Wpf.Axis     Title$ Voltage (mV)��6      MinValue$ 0Q�5          MaxValue$
 4095Q�5!      5#         AxisX 5&        $ Time (seconds)��5'         LabelFormatter 5&      �� *DateTimeFormatter+ 5(      �� *	AxisMax+ 5)      �� *	AxisMin+   Unit 5*      �� *
AxisUnit+   	Separator 5+         LiveCharts.Wpf.Separator  5,         Step 6(   �� *
AxisStep+5.      50      52      53      