using GymManagment.Application.Interfaces;
using GymManagment.Application.Mappings;
using GymManagment.Application.Services;
using GymManagment.Domain.DTO;
using GymManagment.Domain.Models;
using GymManagment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddDbContext<GymDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();








