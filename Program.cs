using KnowIT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<KnowledgeDbContext>()
    .AddDefaultTokenProviders();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Create a logger factory and logger instance explicitly
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();

// Function to retrieve the connection string
string GetConnectionString()
{
    // Check for the connection string in environment variables
    string connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");

    if (string.IsNullOrEmpty(connectionString))
    {
        logger.LogInformation("Environment variable MYSQL_CONNECTION_STRING is not set. Falling back to AWS Secrets Manager...");
        return GetConnectionStringFromSecretsManager(logger);
    }
    else
    {
        logger.LogInformation("Using connection string from environment variable.");
        return connectionString;
    }
}

// Function to retrieve connection string from AWS Secrets Manager
string GetConnectionStringFromSecretsManager(ILogger logger)
{
    try
    {
        logger.LogInformation("Attempting to retrieve secrets and metadata from AWS Secrets Manager...");

        var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.USEast1);

        var describeRequest = new DescribeSecretRequest
        {
            SecretId = "rds!db-6cb2165d-f548-4d91-b918-d082b307a660"
        };

        var describeResponse = client.DescribeSecretAsync(describeRequest).Result;

        var tags = describeResponse.Tags;
        string server = tags.FirstOrDefault(tag => tag.Key == "Server")?.Value;
        string database = tags.FirstOrDefault(tag => tag.Key == "Database")?.Value;

        if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
        {
            throw new Exception("Required tags (Server or Database) are missing in the secret metadata.");
        }

        var secretRequest = new GetSecretValueRequest
        {
            SecretId = "rds!db-6cb2165d-f548-4d91-b918-d082b307a660"
        };

        var secretResponse = client.GetSecretValueAsync(secretRequest).Result;

        var secretJson = secretResponse.SecretString;
        var secretData = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretJson);

        if (!secretData.TryGetValue("username", out var username) || !secretData.TryGetValue("password", out var password))
        {
            throw new Exception("The secret does not contain required fields (username or password).");
        }

        logger.LogInformation("Successfully retrieved secrets and metadata. Constructing the connection string...");
        return $"Server={server};Database={database};User={username};Password={password};";
    }
    catch (Exception ex)
    {
        logger.LogError($"Error retrieving secrets or metadata: {ex.Message}");
        throw;
    }
}

// Function to retrieve admin credentials from AWS Secrets Manager
string GetAdminCredentialsFromSecretsManager(ILogger logger, string secretId)
{
    try
    {
        logger.LogInformation($"Attempting to retrieve admin credentials from AWS Secrets Manager: {secretId}");

        var client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.USEast1);
        var secretRequest = new GetSecretValueRequest { SecretId = secretId };
        var secretResponse = client.GetSecretValueAsync(secretRequest).Result;

        logger.LogInformation("Successfully retrieved admin credentials.");
        return secretResponse.SecretString;
    }
    catch (Exception ex)
    {
        logger.LogError($"Error retrieving admin credentials: {ex.Message}");
        throw;
    }
}

// Add DbContext with the dynamically resolved connection string
builder.Services.AddDbContext<KnowledgeDbContext>(options =>
    options.UseMySql(
        GetConnectionString(),
        new MySqlServerVersion(new Version(8, 0, 35)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        scopedLogger.LogInformation("Attempting to open database connection...");

        var dbContext = scope.ServiceProvider.GetRequiredService<KnowledgeDbContext>();
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();

        scopedLogger.LogInformation("Database connection successfully opened and ensured created.");
    }
    catch (Exception ex)
    {
        scopedLogger.LogError($"Error connecting to the database: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Seed the admin role and user during application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var scopedLogger = services.GetRequiredService<ILogger<Program>>();

    async Task SeedAdminAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        string secretJson = GetAdminCredentialsFromSecretsManager(logger, "KnowIT_Admin_Login");
        var secretData = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretJson);

        string adminEmail = secretData["adminEmail"];
        string adminPassword = secretData["adminPassword"];

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
            else
            {
                logger.LogError("Failed to create admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception("Admin user creation failed.");
            }
        }
    }

    await SeedAdminAsync(services, scopedLogger);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
