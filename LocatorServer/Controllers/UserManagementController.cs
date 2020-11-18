using LocatorServer.Data;
using LocatorServer.Models;
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
        private readonly UserManager<LocatorUser> UserManager;
        private readonly SignInManager<LocatorUser> SignInManager;
        private readonly ApplicationDbContext DbContext;
        public UserManagementController(ApplicationDbContext context, UserManager<LocatorUser> userManager, SignInManager<LocatorUser> signInManager)
        {
            UserManager = userManager;
            DbContext = context;
            SignInManager = signInManager;
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
            ViewBag.AuthRole = roles.Contains("authorized");
            
            return View(user);
        }

        // POST: UserManagementController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(string id, [Bind("Id", "UserName", "RealName", "Email", "EmailConfirmed")]LocatorUser user, bool AdminRole, bool AuthRole)
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
                    existing.RealName = user.RealName;

                    await UserManager.UpdateAsync(existing);
                    //
                    // If roles change to take away access, invalidate their login and force them to log in again
                    // this should happen infrequently, so not that much of a problem ¯\_(ツ)_/¯
                    //
                    if (AdminRole && !roles.Contains("admin"))
                    {
                        //add admin
                        await UserManager.AddToRoleAsync(existing, "admin");
                        await UserManager.UpdateSecurityStampAsync(existing);
                    }
                    else if (!AdminRole && roles.Contains("admin"))
                    {
                        //remove admin
                        await UserManager.RemoveFromRoleAsync(existing, "admin");
                        await UserManager.UpdateSecurityStampAsync(existing);
                    }

                    if (AuthRole && !roles.Contains("authorized"))
                    {
                        //add auth
                        await UserManager.AddToRoleAsync(existing, "authorized");
                        await UserManager.UpdateSecurityStampAsync(existing);
                    }
                    else if (!AuthRole && roles.Contains("authorized"))
                    {
                        //remove auth
                        await UserManager.RemoveFromRoleAsync(existing, "authorized");
                        await UserManager.UpdateSecurityStampAsync(existing);
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
