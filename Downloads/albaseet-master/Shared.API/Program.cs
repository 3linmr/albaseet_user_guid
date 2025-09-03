using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Repository;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Shared.API.Extensions;
using Shared.Service.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using System.Text.Encodings.Web;
using Shared.Helper.Data;
using Shared.Helper.Database;
using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using Shared.Helper.Startup;
using Microsoft.OpenApi.Models;
using JsonDiffPatchDotNet;
using Shared.Helper.Extensions;
using Shared.Helper.Services.Tenant;
using Shared.Helper.Middleware;
using Shared.Repository.Tenant;

var builder = WebApplication.CreateBuilder(args);
var defaultConnectionString = builder.Configuration.GetConnectionString("albaseetDefault");
ConfigurationManager configuration = builder.Configuration;

var useKestrel = configuration.GetValue<bool>("Kestrel:UseKestrel");
var kestrelPort = configuration.GetValue<int>("Kestrel:KestrelPort");
var identityUrl = configuration.GetValue<string>("Application:Identity");
var sharedAPI = configuration.GetValue<string>("Application:SharedAPI");
var allowSwagger = configuration.GetValue<bool>("Swagger:AllowSwagger");
var swaggerClient = configuration.GetValue<string>("Swagger:Client") ?? "";



if (useKestrel)
{
    builder.WebHost.UseKestrel(so =>
    {
        so.Limits.MaxConcurrentConnections = int.MaxValue;
        so.Limits.MaxConcurrentUpgradedConnections = int.MaxValue;
        so.Limits.MaxRequestBodySize = int.MaxValue;
        so.ListenAnyIP(kestrelPort);
    });
}

#region AddingServices

//Add Db Connection
var serverVersion = new MySqlServerVersion(new Version(8, 0, 37));

//builder.Services.AddDbContext<ApplicationDbContext>(options => options
//    .UseMySql(connectionString, serverVersion, b => b.MigrationsAssembly("Shared.Repository")));
//builder.Services.AddDbContext<ApplicationDbContext>(ServiceLifetime.Transient);

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
	options.UseMySql(defaultConnectionString, serverVersion, b => b.MigrationsAssembly("Shared.Repository"));
});
builder.Services.AddDbContext<ApplicationDbContext>(ServiceLifetime.Scoped);

//builder.Services.AddDbContext<ApplicationDbContext>(ServiceLifetime.Scoped);




// Add Application Settings
builder.Services.Configure<ApplicationSettingDto>(builder.Configuration.GetSection("Application")).AddSingleton(sp => sp.GetRequiredService<IOptions<ApplicationSettingDto>>().Value);


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.OAuth2,
		Flows = new OpenApiOAuthFlows
		{
			AuthorizationCode = new OpenApiOAuthFlow
			{
				AuthorizationUrl = new Uri($"{identityUrl}/connect/authorize"),
				TokenUrl = new Uri($"{identityUrl}/connect/token"),
				Scopes = new Dictionary<string, string> {
					{ swaggerClient, swaggerClient }
				}
			}
		}
	});
	options.OperationFilter<AuthorizeCheckOperationFilter>();
    options.OperationFilter<AcceptLanguageFilter>();
    options.OperationFilter<CompanyFilter>();
});

// Register the TenantProvider as a scoped service.
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Register HttpClient using IHttpClientFactory.
builder.Services.AddHttpClient();


//Configure IdentityServer Here
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = identityUrl;
        options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
        options.RequireHttpsMetadata = false;
    });
IdentityModelEventSource.ShowPII = true; //UncommentToShowWhatBehindPPI

var env = builder.Environment;
if (!env.IsDevelopment())
{
	// Register the tenant migration hosted service.
	builder.Services.AddHostedService<TenantMigrationHostedService>();
}


builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(StartupExtensions.GetIdleTimeout());
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", x => { x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Content-Disposition"); }));
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Default;
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});
builder.Services.AddAuthorization();


//AutoMapper
var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = 200000;
    options.ValueLengthLimit = 1024 * 1024 * 100;
});

builder.Services.AddSingleton(configuration);

builder.Services.AddDependencyInjection(configuration); //Add Dependency Injection Services

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.Secure = CookieSecurePolicy.SameAsRequest;
    options.OnAppendCookie = cookieContext =>
        StartupExtensions.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
    options.OnDeleteCookie = cookieContext =>
        StartupExtensions.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
});

#endregion







#region UsingServices

var app = builder.Build();
IdentityHelper.IdentityHelperConfigure(app.Services.GetRequiredService<IConfiguration>(), app.Services.GetRequiredService<IHttpContextAccessor>());
ApiHelper.HelperAPIConfigure(app.Services.GetRequiredService<IConfiguration>());
// Configure the HTTP request pipeline.
if (allowSwagger)
{
	app.UseSwagger();
	//app.UseSwaggerUI(options =>
	//{
	//	options.DocExpansion(DocExpansion.None);
	//});

	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint($"{sharedAPI}/swagger/v1/swagger.json", "Shared APIs");
        c.EnableTryItOutByDefault();
        c.ConfigObject.AdditionalItems.Add("requestSnippetsEnabled", true);
        c.ConfigObject.AdditionalItems.Add("requestSnippets", new { defaultExpanded = false });

        c.OAuthClientId(swaggerClient);
		c.OAuthAppName(swaggerClient);
		c.OAuthUsePkce();
		c.DocExpansion(DocExpansion.None);
		c.DocumentTitle = "Shared APIs";
	});
}

//DatabaseExtensions.UpdateDatabase(app);

app.UseRouting();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();
app.UseSession();
//app.UseHttpsRedirection();
app.UseCookiePolicy();


app.Run();

#endregion

