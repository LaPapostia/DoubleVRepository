    using DataLayer.DbConnection.Factory;
    using DataLayer.DbConnection.Repository;
    using DataLayer.DbConnection;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    /// Add the custom services
    builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
    builder.Services.AddScoped(typeof(IPostgresRepository<>), typeof(PostgresRepository<>));

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();



    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
