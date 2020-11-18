using LocatorServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly ApplicationDbContext DbContext;
        public UserManagementController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            UserManager = userManager;
            DbContext = context;
        }

        // GET: UserManagementController
        public ActionResult Index()
        {
            var rolestore = new Microsoft.AspNetCore.Identity.EntityFrameworkCore.RoleStore<IdentityRole>(DbContext);
            var roleManager = new RoleManager<IdentityRole>(rolestore, null, null, null, null);
            var roles = roleManager.Roles.ToList();
            ViewBag.roles = roles;
            return View(UserManager.Users.ToList());
        }

        // GET: UserManagementController/Details/5
        public async Task<ActionResult> DetailsAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: UserManagementController/Edit/5
        public async Task<ActionResult> EditAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var user = await UserManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            var roles = await UserManager.GetRolesAsync(user);
            ViewBag.AdminRole = roles.Contains("admin");
            
            return View(user);
        }

        // POST: UserManagementController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(string id, [Bind("Id", "UserName", "Email", "EmailConfirmed")]IdentityUser user, bool AdminRole)
        {
            if(id != user.Id)
            {
                return NotFound();
            }
            var existing = await UserManager.FindByIdAsync(user.Id);
            if(existing == null)
            {
                return NotFound();
            }

            var roles = await UserManager.GetRolesAsync(user);

            if (ModelState.IsValid)
            {
                try
                {
                    existing.UserName = user.UserName;
                    existing.Email = user.Email;
                    existing.EmailConfirmed = user.EmailConfirmed;

                    await UserManager.UpdateAsync(existing);

                    if (AdminRole && !roles.Contains("admin"))
                    {
                        //add admin
                        await UserManager.AddToRoleAsync(existing, "admin");
                    }
                    else if (roles.Contains("admin"))
                    {
                        //remove admin
                        await UserManager.RemoveFromRoleAsync(existing, "admin");
                    }
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: UserManagementController/Delete/5
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UserManagementController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            var logins = await UserManager.GetLoginsAsync(user);
            var roles = await UserManager.GetRolesAsync(user);

            await UserManager.UpdateSecurityStampAsync(user);

            foreach (var login in logins)
            {
                await UserManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }

            foreach(var role in roles)
            {
                await UserManager.RemoveFromRoleAsync(user, role);
            }

            await UserManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
