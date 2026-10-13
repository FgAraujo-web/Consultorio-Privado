using Consultorio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ConsultorioDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Consultorio")));
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting();
app.MapControllerRoute(name:"default",pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();
