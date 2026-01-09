using UsersMembers.Ioc;

var builder = WebApplication.CreateBuilder(args);

var awsRegion = builder.Configuration["AWS:Region"];

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddUsersMembersDependencies(awsRegion);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
