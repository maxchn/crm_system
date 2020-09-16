using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrmSystem.Server.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public HomeController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("CRM System API");
        }

        [HttpGet("/u/{ssn:regex([[a-zA-Z0-9!@#$%&]]+)}")]
        public async Task<IActionResult> FileRedirect()
        {
            var value = HttpContext.Request.Path.Value;
            value = value.Replace("/u/", "");

            try
            {
                var shortLink = (await _unitOfWork.ShortLink.Find(u => u.Short.Contains(value)))?.FirstOrDefault();

                if (shortLink is null)
                {
                    Response.StatusCode = 404;
                    return View("PageNotFound");
                }
                else
                {
                    return Redirect(shortLink.Full);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return BadRequest();
        }

        [HttpGet("/share")]
        public async Task<IActionResult> Share(string code)
        {
            try
            {
                FilePublicLink link = (await _unitOfWork.FilePublicLink.Find(l => l.Link.Equals(code)))?.FirstOrDefault();

                if (link != null)
                {
                    string path = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]) + link.Path;

                    if (System.IO.File.Exists(path))
                    {
                        FileContentResult result = await FileUtils.GetFileContent(path, Path.GetFileName(path), HttpContext);
                        return result;
                    }
                    else
                    {
                        Response.StatusCode = 404;
                        return View("PageNotFound");
                    }
                }
                else
                {
                    Response.StatusCode = 404;
                    return View("PageNotFound");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return View("PageNotFound");
            }
        }

        [HttpGet("/attach")]
        [Authorize]
        public async Task<IActionResult> GetAttachedFile(int id)
        {
            try
            {
                var attachedFile = (await _unitOfWork.AttachedFile.Find(f => f.AttachedFileId == id))?.FirstOrDefault();

                if (attachedFile != null)
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    string path = String.Format("{0}{1}", Path.Combine(_configuration["Cloud:BasePath"],
                        _configuration["Cloud:CloudFolderName"]), attachedFile.Path);

                    if (!System.IO.File.Exists(path))
                    {
                        throw new Exception("Файл не найден!");
                    }

                    FileContentResult result = await FileUtils.GetFileContent(path, Path.GetFileName(path), HttpContext);
                    return result;
                }
                else
                {
                    throw new Exception("Прикрепленный файл по указанному ID не найден!");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return View("PageNotFound");
            }
        }
    }
}