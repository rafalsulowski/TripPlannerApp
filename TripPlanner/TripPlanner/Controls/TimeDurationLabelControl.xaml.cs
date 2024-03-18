using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TripPlanner.Controls;

public partial class TimeDurationLabelControl : ContentView
{
	public TimeDurationLabelControl()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty StartTimeProperty = BindableProperty.Create(nameof(StartTime),
            typeof(DateTime),
            typeof(TimeDurationLabelControl),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (TimeDurationLabelControl)bindable;
            });

    public static readonly BindableProperty StopTimeProperty = BindableProperty.Create(nameof(StopTime),
            typeof(DateTime),
            typeof(TimeDurationLabelControl),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (TimeDurationLabelControl)bindable;

                int hour = (control.StopTime - control.StartTime).Hours;
                int Minute = (control.StopTime - control.StartTime).Minutes;

                if(hour == 0)
                    control.m_Label.Text = $"({Minute}m)";
                else
                    control.m_Label.Text = $"({hour}h {Minute}m)";
            });


    public DateTime StopTime
    {
        get => (DateTime)GetValue(StopTimeProperty);
        set => SetValue(StopTimeProperty, value);
    }

    public DateTime StartTime
    {
        get => (DateTime)GetValue(StartTimeProperty);
        set => SetValue(StartTimeProperty, value);
    }
}