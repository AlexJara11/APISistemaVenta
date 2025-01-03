using AutoMapper;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Servicios
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;

        public DashBoardService(IVentaRepository ventaRepository, IGenericRepository<Producto> productoRepository, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _mapper = mapper;
        }
        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVenta, int restarCantidadDias)
        {
            DateTime? ultimaFecha = tablaVenta.OrderByDescending(venta => venta.FechaRegistro).Select(v => v.FechaRegistro).First();
            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);
            return tablaVenta.Where(venta => venta.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }
        private async Task<int> TotalVentasUltimaSemana()
        {
            int total = 0;
            IQueryable<Venta> _ventasQuery = await _ventaRepository.Consultar();
            if (_ventasQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventasQuery, -7);
                total = tablaVenta.Count();
            }
            return total;
        }
        private async Task<string> TotalIngresosUltimaSemana()
        {
            decimal resultado = 0;
            IQueryable<Venta> _ventasQuery = await _ventaRepository.Consultar();
            if (_ventasQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventasQuery, -7);
                resultado = tablaVenta.Select(x => x.Total).Sum(v => v.Value);
            }
            return Convert.ToString(resultado, new CultureInfo("es-PE"));
        }
        private async Task<int> TotalProductos()
        {
            IQueryable<Producto> _productosQuery = await _productoRepository.Consultar();
            int total = _productosQuery.Count();
            return total;
        }
        private async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            IQueryable<Venta> _ventasQuery = await _ventaRepository.Consultar();
            if (_ventasQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventasQuery, -7);
                resultado = tablaVenta.GroupBy(v => v.FechaRegistro.Value.Date)
                                      .OrderBy(v => v.Key)
                                      .Select(v => new { Fecha = v.Key.ToString("dd/MM/yyyy"), Total = v.Count() })
                                      .ToDictionary(keySelector: x => x.Fecha, elementSelector: x => x.Total);
            }
            return resultado;
        }
        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDashBoard = new DashBoardDTO();
            try
            {
                vmDashBoard.TotalVentas = await TotalVentasUltimaSemana();
                vmDashBoard.TotalIngresos = await TotalIngresosUltimaSemana();
                vmDashBoard.TotalProductos = await TotalProductos();
                List<VentaSemanaDTO> listaVentaSemana = new List<VentaSemanaDTO>();
                foreach(KeyValuePair<string, int> item in await VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VentaSemanaDTO { Fecha = item.Key, Total = item.Value });
                }
                vmDashBoard.VentasUltimaSemana = listaVentaSemana;
            }
            catch (Exception)
            {
                throw;
            }
            return vmDashBoard;
        }
    }
}
