using CrmSystem.DAL.Contexts;
using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.Server.Extensions;
using CrmSystem.Server.Models;
using CrmSystem.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CrmSystem.Server.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly NpgsqlDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private IUnitOfWork _unitOfWork;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationRoleManager roleManager,
            NpgsqlDbContext dbContext,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Login,
                                                                      model.Password,
                                                                      model.RememberMe,
                                                                      lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(u => u.Email.Equals(model.Login) || u.PhoneNumber.Equals(model.Password));
                    return Ok(GenerateJwtToken(model.Login, appUser));
                }
            }

            return BadRequest("Invalid Login Or Password");
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model, string token = null)
        {
            IdentityResult result;

            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = !(token is null),
                };

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    result = await _userManager.CreateAsync(newUser, model.Password);
                    if (result.Succeeded)
                    {
                        //await _roleManager.CreateRoleAsync("User");
                        //await _userManager.AddToRoleAsync(newUser, "User");


                        // Если регистрация не по приглашению
                        // то создаем новую компанию
                        if (token is null)
                        {
                            var newCompany = new Company();
                            newCompany.Name = Guid.NewGuid().ToString().Replace("-", "");
                            newCompany.UrlName = newCompany.Name;
                            newCompany.Owner = newUser;

                            await _unitOfWork.Company.Insert(newCompany);
                            await _unitOfWork.SaveChangesAsync();

                            await _unitOfWork.Company.AddEmployee(newCompany.CompanyId, newUser);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        // Иначе прикрепляем регистрируемого пользователя к конкретной компании
                        else
                        {
                            // Ищем приглашение
                            var invitation = (await _unitOfWork.EmployeeInvitation.Find(i => i.Token.Equals(token) && i.Email.Equals(model.Email))).FirstOrDefault();

                            // Если приглашение найдено
                            if (invitation != null)
                            {
                                // Добавляем сотрудника к компании
                                await _unitOfWork.Company.AddEmployee(invitation.CompanyId, newUser);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                return BadRequest("Приглашение не было найдено!!!");
                            }
                        }

                        _unitOfWork.Commit();

                        return Ok();
                    }
                    else
                    {
                        _unitOfWork.Commit();
                    }
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    return BadRequest(ex.Message);
                }

                StringBuilder builder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    builder.AppendLine($"{error.Code}: {error.Description}");
                }

                return BadRequest(builder.ToString());
            }

            return BadRequest("Invalid input parameters");
        }

        /// <summary>
        /// Регистрация пользователя по приглашению на 
        /// основании указанных базовых данных
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sender"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExtendedRegistration([FromBody]ExtendedRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var request = (await _unitOfWork.RegistrationRequest.Find(r => r.Code.Equals(model.Ref)))?.FirstOrDefault();

                    if (request is null)
                        throw new Exception("Неверный код");

                    var department = (await _unitOfWork.Department.Find(d => d.Name.ToLower().Equals(request.Department.ToLower())))?.FirstOrDefault();

                    if (department is null)
                    {
                        var newDepartment = new Department
                        {
                            Name = request.Department,

                        };

                        await _unitOfWork.Department.Insert(newDepartment);
                        await _unitOfWork.SaveChangesAsync();
                        department = newDepartment;
                    }

                    var position = (await _unitOfWork.Position.Find(p =>
                        p.Name.ToLower().Equals(request.Position.ToLower())))?.FirstOrDefault();

                    if (position is null)
                    {
                        var newPosition = new Position
                        {
                            Name = request.Position
                        };

                        await _unitOfWork.Position.Insert(newPosition);
                        await _unitOfWork.SaveChangesAsync();
                        position = newPosition;
                    }


                    var newEmployee = new ApplicationUser
                    {
                        UserName = request.Email,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Patronymic = request.Patronymic,
                        Department = department,
                        Position = position
                    };

                    newEmployee.CompanyEmployees = new List<CompanyEmployee>
                        {
                            new CompanyEmployee
                            {
                                CompanyId = request.CompanyId,
                                User = newEmployee
                            }
                        };

                    // Создаем нового пользователя
                    var result = await _userManager.CreateAsync(newEmployee, model.Password);

                    if (result.Succeeded)
                    {
                        await _unitOfWork.RegistrationRequest.Delete(request.RegistrationRequestId);
                        await _unitOfWork.SaveChangesAsync();

                        // Создаем уведомление о присоединении нового сотрудника к компании
                        CompanyNotification newCompanyNotification = new CompanyNotification
                        {
                            Author = newEmployee,
                            CompanyId = request.CompanyId,
                            NewEmployee = newEmployee,
                            Type = CompanyNotificationType.EmployeeJoin,
                            DateTime = DateTime.Now
                        };

                        await _unitOfWork.CompanyNotification.Insert(newCompanyNotification);

                        _unitOfWork.Commit();
                        return Ok();
                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        return BadRequest(result.Errors);
                    }
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    return BadRequest(ex.Message);
                }
            }

            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            return BadRequest(messages);
        }

        /// <summary>
        /// Создание приглашения на регистрацию в системе
        /// и его отправка на указанный email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendInvitation(string email, int companyId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (user is null)
                    throw new Exception("Пользователь не может быть идентифицирован!!!");

                var company = await _unitOfWork.Company.FindById(companyId);

                if (company is null)
                    throw new Exception("Компания не может быть идентифицирована!!!");

                var searchUser = (await _unitOfWork.User.Find(u => u.Email.ToLower().Equals(email.ToLower())))?.FirstOrDefault();

                if (searchUser != null)
                    throw new Exception("Пользователь с указанным email уже зарегистрирован в системе!!!");

                StringBuilder builder = new StringBuilder();
                bool tokenIsUnique = false;

                while (!tokenIsUnique)
                {
                    // Генерируем новый токен
                    builder.Append($"{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}");

                    // Ищем приглашение с таким же токеном
                    var invitation = await _unitOfWork.EmployeeInvitation.Find(i => i.Token.Equals(builder.ToString()));

                    // если не найдено значит сгенерированный токен уникальный 
                    // иначе генерируем новый токен
                    if (invitation is null || invitation.Count() == 0)
                    {
                        tokenIsUnique = true;
                        break;
                    }

                    builder.Clear();
                }

                var newEmployeeInvintion = new EmployeeInvitation
                {
                    CompanyId = company.CompanyId,
                    Email = email,
                    Token = builder.ToString()
                };

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    await _unitOfWork.EmployeeInvitation.Insert(newEmployeeInvintion);
                    await _unitOfWork.SaveChangesAsync();
                    _unitOfWork.Commit();

                    var callbackUrl = String.Format("{0}join?token={1}&email={2}", _configuration["WebClientUrl"], newEmployeeInvintion.Token, newEmployeeInvintion.Email);
                    await _emailSender.SendInvitationAsync(newEmployeeInvintion.Email, callbackUrl);

                    return Ok();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));

                return BadRequest(messages);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            if (user is null)
            {
                return BadRequest("Пользователь не может быть индентифицирован!");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            var builder = new StringBuilder();
            foreach (var error in result.Errors)
            {
                builder.Append(error.Description);
            }

            return BadRequest(builder.ToString());
        }

        /// <summary>
        /// Получение подробных данных текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDetailsInfo()
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var fullUser = await _unitOfWork.User.FindById(user.Id);

                if (fullUser != null)
                    return Ok(user);
                else
                    return BadRequest("User not identified!!!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userRoles = await _userManager.GetRolesAsync(user);

                StringBuilder builder = new StringBuilder();
                foreach (string role in userRoles)
                {
                    builder.Append(role + ",");
                }

                return Ok(Regex.Replace(builder.ToString(), ".$", ""));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка аватара пользователя
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);

                    // Получаем путь к старому аватару чтобы в последствии удалить его
                    string oldAvatarPath = user.AvatarPath is null ? "" :
                        Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:AvatarsFolderName"], user.AvatarPath);

                    string filename = Guid.NewGuid() + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string pathForNewAvatar = Path.Combine(_configuration["Cloud:BasePath"],
                        _configuration["Cloud:AvatarsFolderName"], filename);

                    using (FileStream stream = new FileStream(pathForNewAvatar, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _unitOfWork.User.UpdateAvatar(user.Id, filename);
                        await _unitOfWork.SaveChangesAsync();
                        _unitOfWork.Commit();

                        if (System.IO.File.Exists(oldAvatarPath))
                            System.IO.File.Delete(oldAvatarPath);

                        return Ok();
                    }
                    catch (Exception)
                    {
                        _unitOfWork.Rollback();

                        if (System.IO.File.Exists(pathForNewAvatar))
                            System.IO.File.Delete(pathForNewAvatar);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest("Avatar is null");
        }

        /// <summary>
        /// Скачивание аватара пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadAvatar(string id)
        {
            var user = await _unitOfWork.User.FindById(id);
            string errorMessage = String.Empty;

            if (user != null)
            {
                if (user.AvatarPath != null)
                {
                    string path = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:AvatarsFolderName"], user.AvatarPath);

                    if (System.IO.File.Exists(path))
                    {
                        new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
                        return new PhysicalFileResult(path, contentType);
                    }
                    else
                    {
                        return NotFound("Аватар не найден!");
                    }
                }
                else
                {
                    errorMessage = "У пользователя не задан аватар!";
                }
            }
            else
            {
                errorMessage = "Пользователь не найден!";
            }

            return BadRequest(errorMessage);
        }

        /// <summary>
        /// Восстановление пароля
        /// </summary>
        /// <param name="email"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email, string callbackUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return BadRequest();
                }

                // Оправляем письмо с инструкцией по восстановлению пароля
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedCode = HttpUtility.UrlEncode(code);
                callbackUrl = callbackUrl.Replace("code_value", encodedCode);
                await _emailSender.SendEmailAsync(user.Email, "Сброс пароля", "Пожалуйста, сбросьте пароль, нажав <a href=\"" + callbackUrl + "\">здесь</a>");

                return Ok("Пожалуйста, проверьте свою электронную почту, чтобы сбросить пароль.");
            }

            return BadRequest();
        }

        /// <summary>
        /// Генерация JWT токена
        /// </summary>
        /// <param name="email"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private object GenerateJwtToken(string email, ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtIssuer"],
                audience: _configuration["JwtIssuer"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenResult = new
            {
                access_token_type = "Bearer",
                access_token = tokenHandler.WriteToken(token),
                expires = expires
            };

            return tokenResult;
        }
    }
}