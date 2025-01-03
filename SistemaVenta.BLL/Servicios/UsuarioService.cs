using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }
        public async Task<List<UsuarioDTO>> Lista()
        {
            try
            {
                var queryUsuario = await _usuarioRepository.Consultar();
                var listaUsuarios = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();
                return _mapper.Map<List<UsuarioDTO>>(listaUsuarios);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuario = await _usuarioRepository.Consultar(usuario => usuario.Correo == correo && usuario.Clave == clave);
                if (queryUsuario.FirstOrDefault() == null)
                    throw new TaskCanceledException("Usuario no encontrado");

                var usuario = queryUsuario.Include(rol => rol.IdRolNavigation).First();
                return _mapper.Map<SesionDTO>(usuario);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var usuarioCreado =  await _usuarioRepository.Crear(_mapper.Map<Usuario>(modelo));
                if(usuarioCreado == null)
                    throw new TaskCanceledException("No se pudo crear el usuario"); 
                var query = await _usuarioRepository.Consultar(usuario => usuario.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();
                return _mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //TODO: Revisar
        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                var usuarioModelo = _mapper.Map<Usuario>(modelo);
                var usuarioEncontrado = await _usuarioRepository.Obtener(usuario => usuario.IdUsuario == usuarioModelo.IdUsuario);
                if(usuarioEncontrado == null)
                    throw new TaskCanceledException("Usuario no encontrado");
                //_mapper.Map(modelo, usuarioEncontrado);

                //if (!await _usuarioRepository.Editar(usuarioEncontrado))
                //    throw new TaskCanceledException("No se pudo editar el usuario");
                //return true;
                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;
                bool respuesta = await _usuarioRepository.Editar(usuarioEncontrado);
                if (!await _usuarioRepository.Editar(usuarioEncontrado))
                    throw new TaskCanceledException("No se pudo editar el usuario");
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var usuarioEncontrado = await _usuarioRepository.Obtener(usuario => usuario.IdUsuario == id);
                if (usuarioEncontrado == null)
                    throw new TaskCanceledException("Usuario no encontrado");
                if (!await _usuarioRepository.Eliminar(usuarioEncontrado))
                    throw new TaskCanceledException("No se pudo eliminar el usuario");
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
