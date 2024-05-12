using OfferVerse.DAL;
using OfferVerse.DAL.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration.GetConnectionString("default");

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUserDAL>(ud => new UserDAL(connectionString));
builder.Services.AddTransient<ICommentaryDAL>(cd => new CommentaryDAL(connectionString));
builder.Services.AddTransient<IServiceDemandedDAL>(sd => new ServiceDemandedDAL(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/*
services.AddSession();
services.AddMvc().AddSessionStateTempDataProvider(); // Pour utiliser le fournisseur de TempData basé sur la session
*/

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
