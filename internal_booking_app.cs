// Internal Resource Booking System

// Project setup: ASP.NET Core MVC selected for better separation of concerns and scalability.
// Using Entity Framework Core with SQLite for easier setup.

// Models

public class Resource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<Booking> Bookings { get; set; } = new List<Booking>();
}

public class Booking
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
}

// DbContext

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Resource> Resources { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Resource)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.ResourceId);
    }
}

// Booking Conflict Logic Example (in Booking Controller)

// Example BookingController with Create method
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Create(Booking booking)
    {
        if (booking.EndTime <= booking.StartTime)
        {
            ModelState.AddModelError("EndTime", "End Time must be after Start Time.");
            return View(booking);
        }

        bool conflict = await _context.Bookings.AnyAsync(b =>
            b.ResourceId == booking.ResourceId &&
            booking.StartTime < b.EndTime && booking.EndTime > b.StartTime);

        if (conflict)
        {
            ModelState.AddModelError("StartTime", "This resource is already booked during the requested time.");
            return View(booking);
        }

        _context.Add(booking);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

// For UI: Use Bootstrap for basic styling.
// Forms should have proper types: datetime-local for date fields.
// Navigation links between: Resources List, Add Resource, Bookings List, Add Booking.

// Additional Requirements:
// - Server-side validation via DataAnnotations (e.g., [Required], [Range]).
// - Client-side validation via jQuery Unobtrusive Validation.
// - Error handling with try-catch in CRUD operations.
// - Version control with Git (commit often with clear messages).

// Submission includes:
// - GitHub repo link.
// - README.md with setup steps.
// - ZIP with source code, database, screenshots.
