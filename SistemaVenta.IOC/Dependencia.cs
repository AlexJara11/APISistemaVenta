using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.Repositorios;
using SistemaVenta.Utility;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.BLL.Servicios;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencias(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("cadenaSQL"));
            });
            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            service.AddScoped<IVentaRepository, VentaRepository>();
            service.AddAutoMapper(typeof(AutoMapperProfile));
            service.AddScoped<IRolService, RolService>();
            service.AddScoped<IUsuarioService, UsuarioService>();
            service.AddScoped<ICategoriaService, CategoriaService>();
            service.AddScoped<IProductoService, ProductoService>();
            service.AddScoped<IVentaService, VentaService>();
            service.AddScoped<IDashBoardService, DashBoardService>();
            service.AddScoped<IMenuService, MenuService>();
        }
    }
}
