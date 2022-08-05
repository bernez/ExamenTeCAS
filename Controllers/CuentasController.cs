using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamenTeCAS.Data;
using ExamenTeCAS.Models;

namespace ExamenTeCAS.Controllers
{
    public class CuentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cuentas
        public async Task<IActionResult> Cuentas(int Id)
        {
            var user = await _context.Usuario.FindAsync(Id);
            List<Cuenta> cuentas = await _context.Cuenta.Where(x => x.Usuario == user).ToListAsync();
            ViewBag.Usuario = user;
            return View(cuentas);
        }


        // GET: Cuentas/Create
        public IActionResult Agregar(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        // POST: Cuentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(Cuenta Cuenta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Cuenta.Id > 0)
                        Cuenta.Id = 0;
              
                var usr = await _context.Usuario.FirstOrDefaultAsync(x => x.Id == Cuenta.IdUsuario);

                if (usr is null)
                {
                    ModelState.AddModelError(string.Empty, "Se ha detectado un error, pruebe más tarde");
                        ViewBag.Id = Cuenta.IdUsuario;

                        return View();
                }

                Cuenta.Fecha = DateTime.Now.ToString("dd/MM/yyyy");
                Cuenta.Hora = DateTime.Now.ToShortTimeString();
                Cuenta.Usuario = usr;
                _context.Add(Cuenta);

                await _context.SaveChangesAsync();

                }
                catch (Exception ex )
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.Id = Cuenta.IdUsuario;
                    return View();
                }
                return RedirectToAction(nameof(Cuentas), new { Id = Cuenta.IdUsuario });
            }
            return View(Cuenta);
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuenta.Include(x => x.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // GET: Cuentas/Edit/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cuenta = await _context.Cuenta.Include(x => x.Usuario).FirstOrDefaultAsync(m => m.Id == id);
            if (Cuenta == null)
            {
                return NotFound();
            }
            return View(Cuenta);
        }

        // POST: Cuentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id,Nombre,Numero,Saldo,IdUsuario")] Cuenta Cuenta)
        {
            if (id != Cuenta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Cuenta.Usuario = await _context.Usuario.FindAsync(Cuenta.IdUsuario);
                    _context.Update(Cuenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentaExists(Cuenta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Cuentas) ,new { Id = Cuenta.IdUsuario});
            }
            return View(Cuenta);
        }

        // GET: Cuentas/Delete/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Cuenta = await _context.Cuenta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Cuenta == null)
            {
                return NotFound();
            }

            return View(Cuenta);
        }

        // POST: Cuentas/Delete/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Cuenta = await _context.Cuenta.FindAsync(id);
            _context.Cuenta.Remove(Cuenta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool CuentaExists(int id)
        {
            return _context.Cuenta.Any(e => e.Id == id);
        }


        public async Task<IActionResult> Movimientos(int Id)
        {
            var cuenta = await _context.Cuenta.Include( x=> x.Usuario).FirstOrDefaultAsync(y => y.Id == Id);
            var FechaAnt = DateTime.Now.AddMonths(-3).Date;

            IQueryable<Movimiento> movimientos = _context.Movimiento.Where(x => x.Cuenta == cuenta).OrderByDescending(x => x.Id);
            ViewBag.Cuenta = cuenta;
            ViewBag.Usuario = cuenta.Usuario;
            return View(movimientos);
        }


        [HttpPost]
        public async Task<IActionResult> GuardaMovimiento(Movimiento mov)
        {
            var cuenta = await _context.Cuenta.Include(x => x.Usuario).FirstOrDefaultAsync(y => y.Id == mov.IdCuenta);

            mov.Fecha = DateTime.Now.ToString("dd/MM/yyyy");
            mov.Hora = DateTime.Now.ToShortTimeString();
            mov.Cuenta = cuenta;

            _context.Movimiento.Add(mov);

            if(mov.Tipo == 'D')
            {
                cuenta.Saldo += mov.Monto;
            }else if(mov.Tipo == 'R')
            {
                cuenta.Saldo -= mov.Monto;
            }
            else
            {
                return BadRequest(new { mensaje = "No se recibió el tipo correcto" });
                
            }
            _context.Cuenta.Update(cuenta);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
