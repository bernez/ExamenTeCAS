using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamenTeCAS.Data;
using ExamenTeCAS.Models;
using Microsoft.AspNetCore.Identity;

namespace ExamenTeCAS.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Clientes()
        {
            var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
            IEnumerable<Usuario> usuariosList = _context.Usuario.Where(x => clientes.Select(y => y.Id).Contains(x.UID)); 
            return View(usuariosList);

        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Agregar
        public IActionResult Agregar()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar([Bind("Id,Nombre,Apaterno,Amaterno,Correo,Telefono,Identificacion")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
             
                    var user = new IdentityUser { UserName = $"{usuario.Nombre} {usuario.Apaterno}", Email = usuario.Correo, PhoneNumber = usuario.Telefono };

                    var result = await _userManager.CreateAsync(user, "PruebaTecas");
               
                    if (result.Succeeded)
                    {
                            await _userManager.AddToRoleAsync(user, "Cliente");

                        usuario.FechaIngreso = DateTime.Now.ToString("dd/MM/yyyy");
                        usuario.UID = user.Id;

                        _context.Add(usuario);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Clientes));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            if (error.Code == "DuplicateUserName")
                            {
                                error.Description = "Ya se encuentra registrado un usuario con el correo '" + user.Email + "'";
                            }
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception)
                {

                    ModelState.AddModelError(string.Empty, "Error al crear el usuario");
                }
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id,Nombre,Apaterno,Amaterno,Correo,Telefono,Identificacion,FechaIngreso,UID")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Clientes));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            var cuentas = await _context.Cuenta.Where(x => x.Usuario == usuario).ToListAsync();

            Parallel.ForEach(cuentas, async item =>
            {
                var movs = await _context.Movimiento.Where(x => x.Cuenta == item).ToListAsync();
                _context.Movimiento.RemoveRange(movs);
                await _context.SaveChangesAsync();
            });

            _context.Cuenta.RemoveRange(cuentas);
            await _context.SaveChangesAsync();
            
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Clientes));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}
