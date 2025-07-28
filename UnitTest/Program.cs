
using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.Services;

namespace UnitTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IProdutoService, ProdutoService>();
            builder.Services.AddScoped<IEstoqueService, EstoqueService>();

			builder.Services.AddDbContext<EstoqueDB>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
        }
    }
}
