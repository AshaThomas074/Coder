using Coder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Coder.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

builder.Services.AddSession();


if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<CoderDBContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("CoderConnProd")), ServiceLifetime.Transient);
}
else
{
    builder.Services.AddDbContext<CoderDBContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("CoderConnProd")), ServiceLifetime.Transient);
}

builder.Services.AddDefaultIdentity<ApplicationUser>()
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<CoderDBContext>();



var app = builder.Build();
/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoderDBContext>();
    db.Database.Migrate();
}*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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
app.UseAuthentication();;
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints => endpoints.MapRazorPages());
//app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
