
// using Mapster;
// using Npgsql;
// using DataServiceLayer;

// class start {
// public static DataService DataService { get; } = new DataService();
//     static void starts(string[] args)
//     {


//         var builder = WebApplication.CreateBuilder(args);

//         // Add services to the container.

//         builder.Services.AddSingleton<DataService, DataService>();

//         builder.Services.AddMapster();

//         builder.Services.AddControllers();

//         var app = builder.Build();

//         // Configure the HTTP request pipeline.

//         app.MapControllers();
//         app.Run();
//     }
// }