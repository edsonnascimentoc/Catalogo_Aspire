using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var sqldb = sql.AddDatabase("sqldb");

var keycloakDb = sql.AddDatabase("keycloakdb");

var keycloak = builder
    .AddContainer("keycloak", "quay.io/keycloak/keycloak")
    .WithEnvironment("KEYCLOAK_ADMIN", "admin")
    .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME", "localhost")
    .WithEnvironment("KC_HOSTNAME_PORT", "60132")
    .WithEnvironment("KC_PROXY_HEADERS", "xforwarded")
    .WithReference(keycloakDb)
    .WithVolume(
        "keycloak_data",
        "/opt/keycloak/data")
    .WithBindMount(
        "./KeyCloakConfiguration",
        "/opt/keycloak/data/import")
    .WithArgs(
        "start-dev",
        "--import-realm",
        "--hostname-strict=false")
    .WithHttpEndpoint(
        port: 60132,
        targetPort: 8080,
        name: "http");

var apiService = builder
    .AddProject<Projects.Catalogo_ApiProdutos>("catalogo-apiprodutos")
    .WaitFor(sql)
    .WaitFor(sqldb)
    .WaitFor(keycloak)
    .WithEnvironment("KEYCLOAK__ClientId", "workspaces-client")
    .WithEnvironment("KEYCLOAK__ClientSecret", "zev");

builder
    .AddProject<Projects.Catalogo_LojaWeb>("catalogo-lojaweb")
    .WithReference(apiService)
    .WaitFor(keycloak)
    .WithEnvironment("KEYCLOAK__ClientId", "workspaces-client")
    .WithEnvironment("KEYCLOAK__ClientSecret", "zev");

builder.Build().Run();