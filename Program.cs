using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(); // apos add metodos de cache, precisamos desse método e adicionar quais os locais, antes  do Authorization/Authentication
builder.Services.AddResponseCompression(option =>
{
    option.Providers.Add<GzipCompressionProvider>();
    option.MimeTypes = 
        ResponseCompressionDefaults.MimeTypes.Concat(new[] {"application/json"});
        //posso comprimir outros tipode arquivos Ex:img, vid, aud, etc..
});

//builder.Services.AddResponseCaching(); - //adiciona por padrão um cabeçalho de cache
builder.Services.AddControllers();

//cria a chave para decode
var key = Encoding.ASCII.GetBytes(Shop.Settings.Secret);
builder.Services.AddAuthentication( x=>
{
    //definição da autenticação da aplicação
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=>{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//adicionar referencia do DbContext
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));//-> referencia para trabalhar in memory

//PARA DEPLOYE VAMOS TESTAR PRIMEIRO O BANCO EM MEMÓRIA
//adicionar a conexão do banco
//builder.Services.AddDbContext<DataContext>(
//    opt => opt.UseSqlServer(
//        builder.Configuration.GetConnectionString("connectionString")
//    )
//);

//builder.Services.AddScoped<DataContext, DataContext>();-> DIZ QUE PODE REMOVER

builder.Services.AddSwaggerGen( c=> 
{
    c.SwaggerDoc("v1", new OpenApiInfo{ Title="Shop Api", Version ="v1"});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API v1");
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();


app.UseCors(x=> x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.MapControllers();

app.Run();
