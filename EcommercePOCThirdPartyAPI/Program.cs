using EcommercePOCThirdPartyAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
//builder.Services.AddRazorPages();
builder.Services.AddDbContext<ProjectEcommerceContext>(opt =>
 opt.UseSqlServer());
builder.Services.AddControllers()

  .AddJsonOptions(options =>
  {
      // Preserve object references during JSON serialization
      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      /*With this configuration, when you return objects that have circular references or shared objects between different parts of your response, 
      the JSON serializer will handle them appropriately by preserving the object references, 
      which can help reduce the size of the JSON response and prevent infinite loops during serialization.*/
  });
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            //This configure the swagger with below used Titles and helps us to locate Developer and Github Repository
            Version = "Demo",
            Title = "Ecommerce API",
            Description = "An ASP.NET Core Web API 6.0 for managing an ECommerce Project",
            //TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Developer's LinkedIn Contact",
                Url = new Uri("https://www.linkedin.com/in/zahid-farooq-dar/")
            },
            License = new OpenApiLicense
            {
                Name = "Projet's Github Link",
                Url = new Uri("https://github.com/ZahidFarooqDar/RenoProject1/tree/main/ProjectHero")
            }
        });

        // Define a security scheme for bearer tokens
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        // Assign the security requirements to the operations
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    });

// Create a service scope to access the RoleManager
builder.Services.AddAutoMapper(typeof(Program));
/*builder.Services.TryAddScoped<IAccountRepository, AccountRepository>();
builder.Services.TryAddScoped<ISellerRepository, SellerRepository>();*/
/*builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SellerPolicy", policy => policy.RequireRole("SELLER"));
    options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("CUSTOMER"));
});
builder.Services.AddAuthentication(options =>
{
    // Set the default authentication scheme to JwtBearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option =>
{
    option.RequireHttpsMetadata = false;
    // Indicate that the token should be saved in the authentication properties
    option.SaveToken = true;

    // Disable HTTPS metadata validation (only for development, not recommended in production)
    *//*    RequireHttpsMetadata is set to false, indicating that HTTPS metadata validation is disabled.
          This is typically done for development purposes and should be enabled in a production environment.*//*


    // Configure token validation parameters
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        // Validate the issuer signing key
        ValidateIssuerSigningKey = true, // Uncomment if you want to validate the issuer signing key
        // Validate the issuer (typically the server that created the token)
        // Set the issuer signing key used to validate the token's signature
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidateIssuer = true,

        // Validate the audience (typically the intended recipient of the token)
        ValidateAudience = false,

        // Define the valid issuer and audience from configuration

        // ValidAudience = builder.Configuration["JWT:ValidAudience"],



        // Validate the token's lifetime (expiration date)
        //ValidateLifetime = true, // Uncomment if you want to validate token lifetime  
    };
});*/
//CORS 
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();



app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("corspolicy");
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
//app.MapControllers();

app.Run();
