using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Data;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Implementations;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Implementations;
using VetClinicMVC.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<VetClinicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<VetClinicContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Veterinarian", "Receptionist", "Client" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Admin
    var adminEmail = "admin@vetcare.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new IdentityUser 
        { 
            UserName = adminEmail, 
            Email = adminEmail, 
            EmailConfirmed = true 
        };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Client
    var clientEmail = "client@vetcare.com";
    if (await userManager.FindByEmailAsync(clientEmail) == null)
    {
        var clientUser = new IdentityUser 
        { 
            UserName = clientEmail, 
            Email = clientEmail, 
            EmailConfirmed = true 
        };
        await userManager.CreateAsync(clientUser, "Client123!");
        await userManager.AddToRoleAsync(clientUser, "Client");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
