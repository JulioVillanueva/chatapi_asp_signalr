using demo_chatHub_azureSignalR;
using demo_chatHub_azureSignalR.Data;
using demo_chatHub_azureSignalR.model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    //options =>
    //{
    //    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    //    {
    //        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    //        Name = "Authorization",
    //        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    //    });
    //    options.OperationFilter<SecurityRequirementsOperationFilter>();
    //}
    );

builder.Services.AddSignalR(o =>
{
  o.EnableDetailedErrors = true;
})
    .AddAzureSignalR("Endpoint=https://sigr-chatdemo-dev-uksouth-001.service.signalr.net;AccessKey=J1HtR9kIV+OrDR99XkDsVAyoo0SK7QNRwolgNnE1GSA=;Version=1.0;");;


builder.Services.AddAuthorization();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDb"));
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddIdentityCore<IdentityUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddApiEndpoints();// this should be the same as AddIdentityApiEndpoints directly 




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}



app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
app.MapPost("/public-chat/broadcast", async Task<Results<Ok, BadRequest>> ([FromBody] Message message, IHubContext<ChatHub, IChatClient> context) =>
{
    await context.Clients.All.ReceiveMessage(message.message);
    return TypedResults.Ok();
}
);



//app.MapPost("/private-chat/broadcast", async Task<Results<Ok, BadRequest>> ([FromBody] Message message, IHubContext<ChatHub, IChatClient> context) =>
//{
//    await context.Clients.All.ReceiveMessage(message.message);
//    return TypedResults.Ok();
//})
//    .RequireAuthorization();

app.MapGroup("iam")
    .MapIdentityApi<IdentityUser>();

app.MapGroup("public-chat")
    .MapHub<ChatHub>("chat-hub");

//app.MapGroup("Private-chat")
//    .RequireAuthorization()
//    .MapHub<ChatHub>("chat-hub");

app.Run();


