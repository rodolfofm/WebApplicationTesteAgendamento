using WebApplicationTesteAgendamento;
using WebApplicationTesteAgendamento.Request;
using WebApplicationTesteAgendamento.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SchedulerService>();

var app = builder.Build();

var schedulerService = app.Services.GetRequiredService<SchedulerService>();
schedulerService.StartAsync().Wait();

app.Lifetime.ApplicationStopping.Register(() =>
{
    schedulerService.StopAsync().Wait();
});

//await schedulerService.ScheduleJobAsync(new JobScheduleRequest
//{
//    JobId = "banco 1",
//    JobType =  new BancoJob().GetType().FullName,
//    CronExpression = "0/5 * * * * ?"
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
