var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

// Registrar projetos manualmente não é necessário para o build local; iniciar o host.

builder.Build().Run();
