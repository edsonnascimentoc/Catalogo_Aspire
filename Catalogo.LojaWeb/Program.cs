using Catalogo.LojaWeb.Components;
using Catalogo.LojaWeb.Services;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ProdutoService>();
builder.Services.AddHttpClient<ProdutoService>(c =>
{
    var url = builder.Configuration["ProdutoEndpoint"] 
                 ?? throw new InvalidOperationException("ProdutoEndpoint não definido");

    c.BaseAddress = new(url);
})
    .AddHttpMessageHandler<DownStreamApiTokenHandler>();

    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    });

builder
    .Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddKeycloakWebApp(
        builder.Configuration.GetSection(KeycloakAuthenticationOptions.Section),
        configureOpenIdConnectOptions: options =>
        {
            options.SaveTokens = true;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.RequireHttpsMetadata = false;
            options.ClientId = builder.Configuration["Keycloak:ClientId"];
            options.ClientSecret = builder.Configuration["Keycloak:ClientSecret"];
            options.Events = new OpenIdConnectEvents
            {
                OnSignedOutCallbackRedirect = context =>
                {
                    context.Response.Redirect("/Home/Public");
                    context.HandleResponse();

                    return Task.CompletedTask;
                }

            };
        });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
