using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepository;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleVentaRepository, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _mapper = mapper;
        }
        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepository.Registrar(_mapper.Map<Venta>(modelo));
                if (ventaGenerada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo crear la venta");
                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepository.Consultar();
            var listaResultado = new List<Venta>();
            try
            {
                if (buscarPor == "fecha")
                {
                    DateTime fec_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                    DateTime fec_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));
                    listaResultado = await query.Where(venta => venta.FechaRegistro.Value.Date >= fec_Inicio && 
                                                                venta.FechaRegistro.Value.Date <= fec_Fin)
                                                .Include(dv => dv.DetalleVenta)
                                                .ThenInclude(p => p.IdProductoNavigation)
                                                .ToListAsync();
                }
                else
                {
                    listaResultado = await query.Where(venta => venta.NumeroDocumento == numeroVenta)
                                                .Include(dv => dv.DetalleVenta)
                                                .ThenInclude(p => p.IdProductoNavigation)
                                                .ToListAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _mapper.Map<List<VentaDTO>>(listaResultado);
        }

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepository.Consultar();
            var listaResultado = new List<DetalleVenta>();
            try
            {
                DateTime fec_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                DateTime fec_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));
                listaResultado = await query.Include(p => p.IdProductoNavigation)
                                            .Include(v => v.IdVentaNavigation)
                                            .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fec_Inicio &&
                                                         dv.IdVentaNavigation.FechaRegistro.Value.Date <= fec_Fin)
                                            .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return _mapper.Map<List<ReporteDTO>>(listaResultado);
        }

    }
}
