using EventEase.Models;

namespace EventEase.Services;

public class EventService
{
    private readonly object _lock = new();
    private readonly List<EventItem> _events = new()
    {
        new EventItem
        {
            Id = 1,
            Title = "Spring Kickoff",
            Location = "Main Hall",
            Date = DateTime.Today.AddDays(3),
            Attendees = 42
        },
        new EventItem
        {
            Id = 2,
            Title = "Community Workshop",
            Location = "Room B",
            Date = DateTime.Today.AddDays(7),
            Attendees = 18
        },
        new EventItem
        {
            Id = 3,
            Title = "Volunteer Meetup",
            Location = "Riverside Center",
            Date = DateTime.Today.AddDays(12),
            Attendees = 26
        }
    };

    private int _nextId = 4;

    public event Action? Changed;

    public int GetEventCount()
    {
        lock (_lock)
        {
            return _events.Count;
        }
    }

    public IReadOnlyList<EventItem> GetEvents()
    {
        lock (_lock)
        {
            return _events
                .OrderBy(eventItem => eventItem.Date)
                .ThenBy(eventItem => eventItem.Title)
                .ToList();
        }
    }

    public EventItem AddEvent(EventItem newEvent)
    {
        if (newEvent is null)
        {
            throw new ArgumentNullException(nameof(newEvent));
        }

        if (string.IsNullOrWhiteSpace(newEvent.Title))
        {
            throw new ArgumentException("Title is required.", nameof(newEvent));
        }

        if (string.IsNullOrWhiteSpace(newEvent.Location))
        {
            throw new ArgumentException("Location is required.", nameof(newEvent));
        }

        if (newEvent.Attendees < 0)
        {
            throw new ArgumentException("Attendees cannot be negative.", nameof(newEvent.Attendees));
        }

        Action? changed;
        EventItem eventToAdd;

        lock (_lock)
        {
            eventToAdd = new EventItem
            {
                Id = _nextId++,
                Title = newEvent.Title.Trim(),
                Location = newEvent.Location.Trim(),
                Date = newEvent.Date == default ? DateTime.Today : newEvent.Date.Date,
                Attendees = newEvent.Attendees
            };

            _events.Add(eventToAdd);
            changed = Changed;
        }

        changed?.Invoke();

        return eventToAdd;
    }

    public void UpdateAttendance(int eventId, int attendees)
    {
        if (attendees < 0)
        {
            throw new ArgumentException("Attendees cannot be negative.", nameof(attendees));
        }

        Action? changed;

        lock (_lock)
        {
            var existingEvent = _events.FirstOrDefault(eventItem => eventItem.Id == eventId);

            if (existingEvent is null)
            {
                throw new ArgumentException("The event was not found.", nameof(eventId));
            }

            existingEvent.Attendees = attendees;
            changed = Changed;
        }

        changed?.Invoke();
    }
}