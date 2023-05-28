using Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace EfcDataAccess;

[ExcludeFromCodeCoverage]
class DatabaseInsertion
{
    private static Context _context;
    public static async Task InsertMyEntitiesAsync(int length)
    {
        for (int i = 1; i <= length; i++)
        {
            CO2 co2 = new CO2() { CO2Id = i, Date = new DateTime(2023, 04, i, 13, i, 00), Value = 1000+i};
            await _context.CO2s.AddAsync(co2);
            Humidity humidity = new Humidity()
                { HumidityId = i, Date = new DateTime(2023, 04, i, 13, i, 00), Value = 20 + i };
            await _context.Humidities.AddAsync(humidity);
            Temperature temperature = new Temperature()
                { TemperatureId = i, Date = new DateTime(2023, 04, i, 13, i, 00), Value = 20 + i/5 };
            await _context.Temperatures.AddAsync(temperature);
        }
        //Intervals
        Interval intervalMonday = new Interval() { Id = 1, StartTime = new TimeSpan(10, 00, 00), EndTime = new TimeSpan(12, 00, 00), DayOfWeek = DayOfWeek.Monday};
        Interval intervalWednesday = new Interval() { Id = 2, StartTime = new TimeSpan(8, 00, 00), EndTime = new TimeSpan(9, 00, 00), DayOfWeek = DayOfWeek.Wednesday};
        Interval intervalFriday = new Interval() { Id = 3, StartTime = new TimeSpan(17, 00, 00), EndTime = new TimeSpan(18, 00, 00), DayOfWeek = DayOfWeek.Friday};
        List<Interval> intervals = new List<Interval>();
        intervals.Add(intervalMonday);
        intervals.Add(intervalWednesday);
        intervals.Add(intervalFriday);
        await _context.Intervals.AddRangeAsync(intervals);
        
        
        //Thresholds
        Threshold threshold1 = new Threshold() { Id = 1, Type = "temperature", MinValue = 20, MaxValue = 40 };
        Threshold threshold2 = new Threshold() { Id = 2, Type = "humidity", MinValue = 20, MaxValue = 100 };
        Threshold threshold3 = new Threshold() { Id = 3, Type = "co2", MinValue = 1000, MaxValue = 1200 };
        List<Threshold> thresholds = new List<Threshold>();
        thresholds.Add(threshold1);
        thresholds.Add(threshold2);
        thresholds.Add(threshold3);
        await _context.Thresholds.AddRangeAsync(thresholds);

        //Preset
        Preset preset = new Preset() { Id = 1, Thresholds = thresholds, IsCurrent = true, Name = "Preset 1"};
        await _context.Presets.AddAsync(preset);

        //Email
        NotificationEmail notificationEmail = new NotificationEmail() { Email = "greenhousesep4@gmail.com" };
        await _context.NotificationEmails.AddAsync(notificationEmail);

        //ValveState
        ValveState valveState = new ValveState() { Toggle = false };
        await _context.ValveState.AddAsync(valveState);
    }

    public static async Task Main(string[] args)
    {
        // Create a new instance of the context and pass it to the DatabaseInsertion constructor
        _context = new Context();
        _context.Humidities.RemoveRange(_context.Humidities);
        _context.CO2s.RemoveRange(_context.CO2s);
        _context.Temperatures.RemoveRange(_context.Temperatures);
        _context.Intervals.RemoveRange(_context.Intervals);
        _context.Thresholds.RemoveRange(_context.Thresholds);
        _context.Presets.RemoveRange(_context.Presets);
        _context.NotificationEmails.RemoveRange(_context.NotificationEmails);
        _context.ValveState.RemoveRange(_context.ValveState);
        // Call the InsertMyEntitiesAsync method
        await InsertMyEntitiesAsync(10);

        // Save changes to the database
        await _context.SaveChangesAsync();
    }
}
