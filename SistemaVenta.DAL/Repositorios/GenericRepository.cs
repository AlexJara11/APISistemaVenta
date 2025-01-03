using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Repositorios
{
    public class GenericRepository<TModelo> : IGenericRepository<TModelo> where TModelo : class
    {
        private readonly DbventaContext _dbvcontext;

        public GenericRepository(DbventaContext dbcontext)
        {
            _dbvcontext = dbcontext;
        }
        public async Task<TModelo> Obtener(Expression<Func<TModelo, bool>> filtro)
        {
            try
            {
                TModelo modelo = await _dbvcontext.Set<TModelo>().FirstOrDefaultAsync(filtro);
                return modelo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<TModelo> Crear(TModelo modelo)
        {
            try
            {
                _dbvcontext.Set<TModelo>().Add(modelo);
                await _dbvcontext.SaveChangesAsync();
                return modelo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Editar(TModelo modelo)
        {
            try
            {
                _dbvcontext.Set<TModelo>().Update(modelo);
                await _dbvcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(TModelo modelo)
        {
            try
            {
                _dbvcontext.Set<TModelo>().Remove(modelo);
                await _dbvcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IQueryable<TModelo>> Consultar(Expression<Func<TModelo, bool>> filtro = null)
        {
            try
            {
                IQueryable<TModelo> queryModelo = filtro == null ? _dbvcontext.Set<TModelo>() 
                                                                 : _dbvcontext.Set<TModelo>().Where(filtro);
                return queryModelo;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
