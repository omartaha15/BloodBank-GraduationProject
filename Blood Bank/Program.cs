using BloodBank.Core.Entities;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using BloodBank.Infrastructure.Repositories;
using BloodBank.Business.Services;
using BloodBank.Business.Interfaces;
using BloodBank.Business.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BloodBank.Core.Constants;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context
builder.Services.AddDbContext<BloodBankDbContext>( options =>
    options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

// Identity Configuration
builder.Services.AddIdentity<User, IdentityRole>( options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes( 15 );
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
} )
.AddEntityFrameworkStores<BloodBankDbContext>()
.AddDefaultTokenProviders();

// Configure Cookie Settings
builder.Services.ConfigureApplicationCookie( options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays( 7 );
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
} );

// Authorization Policies
builder.Services.AddAuthorization( options =>
{
    options.AddPolicy( "RequireAdminRole", policy =>
        policy.RequireRole( Roles.Admin ) );

    options.AddPolicy( "RequireStaffRole", policy =>
        policy.RequireRole( Roles.Staff, Roles.Admin ) );

    options.AddPolicy( "RequireDonorRole", policy =>
        policy.RequireRole( Roles.Donor ) );

    options.AddPolicy( "RequireHospitalRole", policy =>
        policy.RequireRole( Roles.Hospital ) );
} );




// Repositories
builder.Services.AddScoped( typeof( IGenericRepository<> ), typeof( GenericRepository<> ) );
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDonationRepository, DonationRepository>();
builder.Services.AddScoped<IBloodTestRepository, BloodTestRepository>();
builder.Services.AddScoped<IBloodUnitRepository, BloodUnitRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
builder.Services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IBloodTestService, BloodTestService>();
builder.Services.AddScoped<IBloodUnitService, BloodUnitService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IHospitalService, HospitalService>();
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
//builder.Services.AddScoped<IInventoryService, InventoryService>();

// AutoMapper
builder.Services.AddAutoMapper( typeof( MappingProfile ) );

var app = builder.Build();


using ( var scope = app.Services.CreateScope() )
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BloodBankDbContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.Initialize( services );
    }
    catch ( Exception ex )
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError( ex, "An error occurred while seeding the database." );
    }
}

// Initialize Database and Roles
using ( var scope = app.Services.CreateScope() )
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BloodBankDbContext>();
        context.Database.Migrate();
        await DbInitializer.Initialize( services );
    }
    catch ( Exception ex )
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError( ex, "An error occurred while seeding the database." );
    }
}

// Configure the HTTP request pipeline.
if ( !app.Environment.IsDevelopment() )
{
    app.UseExceptionHandler( "/Home/Error" );
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}" );

app.Run();