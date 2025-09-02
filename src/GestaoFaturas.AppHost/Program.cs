var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("gestaoFaturas");

// Add the API project
var api = builder.AddProject<Projects.GestaoFaturas_Api>("api")
    .WithReference(database);

builder.Build().Run();
