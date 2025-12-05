var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DatabaseHelper>();

// Đăng ký Session và HttpContextAccessor
// Cấu hình thời gian timeout cho session nếu cần (mặc định là 20 phút)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 1. UseStaticFiles nên được gọi để phục vụ file tĩnh (css, js, images)
app.UseStaticFiles();

app.UseRouting();

// 2. Authentication phải đứng trước Authorization
app.UseAuthentication();
app.UseAuthorization();

// 3. QUAN TRỌNG: UseSession phải đặt TRƯỚC khi MapControllerRoute
app.UseSession();

// Nếu bạn dùng .NET 9 trở lên thì giữ lại dòng này, nếu không có thể bỏ
// app.MapStaticAssets(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");
// .WithStaticAssets(); // Chỉ dùng nếu dùng MapStaticAssets

app.Run();