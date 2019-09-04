using System;
using System.Collections.Generic;
using System.Linq;
using VideoFilesLibrary.Helpers;
using Microsoft.AspNetCore.Mvc;
using VideoFilesLibrary.DataAccess.Interfaces;
using VideoFilesLibrary.Models;
using System.Threading.Tasks;
using VideoFilesLibrary.DataAccess.Implementation;

namespace VideoFilesLibrary.Controllers {
    public class AccountController : Controller {
        


        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserViewModel valUser) {
            try {
                IAccountData vUserData = new AccountData();
                UserViewModel vUserResponse = vUserData.Verify(valUser.UserName.Trim(), valUser.Password.Trim());

                if (vUserResponse != null) {
                    HttpContext.Session.Set<UserViewModel>("UserSesion", vUserResponse);
                    return Json(new { content = "1" });
                }
            } catch (Exception) {
                HttpContext.Session.Remove("UserSesion");
                return Json(new { content = "3" });
            }
            return Json(new { content = "3" });
        }

        public IActionResult Logout() {
            HttpContext.Session.Remove("UserSesion");
            return Json(new {content = "True" });
        }
    }
}
