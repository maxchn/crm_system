using CrmSystem.DAL.Entities;
using CrmSystem.DAL.Interfaces;
using CrmSystem.Server.Models;
using CrmSystem.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using File = CrmSystem.DAL.Entities.File;

namespace CrmSystem.Server.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]")]
    public class CloudController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private const double MAX_CLOUD_SIZE = 104857600;

        public CloudController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetFilesList(string path, int companyId, int type)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var company = await _unitOfWork.Company.FindById(companyId);

            try
            {
                if (user is null)
                    throw new ArgumentException("User with the specified ID not found!!!");

                if (company is null)
                    throw new ArgumentException("Company with the specified ID not found!!!");

                List<File> files = new List<File>();

                if (IsFoundPersonalFolder(user.Id, company.CompanyId.ToString()))
                {
                    path = HttpUtility.UrlDecode(path);

                    Regex regex = new Regex(@"\\");
                    path = regex.Replace(path, "", 1);

                    string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());
                    string dirPath = string.IsNullOrEmpty(path) ? basePath : Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString(), path);

                    if (Directory.Exists(dirPath))
                    {
                        if (type == 1 || type == 3)
                        {
                            foreach (var directory in Directory.GetDirectories(dirPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                                if (directoryInfo.Exists)
                                {
                                    var directoryInfoObject = new File
                                    {
                                        Type = FileType.Directory,
                                        Name = directoryInfo.Name,
                                        Path = directoryInfo.FullName.Replace(basePath, "")
                                    };

                                    files.Add(directoryInfoObject);
                                }
                            }
                        }

                        if (type == 2 || type == 3)
                        {
                            foreach (var file in Directory.GetFiles(dirPath))
                            {
                                FileInfo fileInfo = new FileInfo(file);

                                if (fileInfo.Exists)
                                {
                                    FilePublicLink link = (await _unitOfWork.FilePublicLink.Find(l => l.Path.Equals(
                                        fileInfo.FullName.Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), ""))))
                                        ?.FirstOrDefault();

                                    var fileInfoObject = new File
                                    {
                                        Type = FileType.File,
                                        Name = fileInfo.Name,
                                        Path = fileInfo.FullName.Replace(basePath, ""),
                                        PublicLink = link is null ? null : link.ShortLink.Short
                                    };

                                    files.Add(fileInfoObject);
                                }
                            }
                        }
                    }
                }

                return Ok(files);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder(int companyId, string path, string nameOfNewFolder)
        {
            if (string.IsNullOrEmpty(path))
                return BadRequest("Путь не может быть пустым!");

            if (string.IsNullOrEmpty(nameOfNewFolder))
                return BadRequest("Название папки не может быть пустым!");

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var company = await _unitOfWork.Company.FindById(companyId);

            if (user != null && company != null)
            {
                if (!IsFoundPersonalFolder(user.Id, company.CompanyId.ToString()))
                    CreatePersonalFolder(user.Id, company.CompanyId.ToString());

                path = HttpUtility.UrlDecode(path);
                nameOfNewFolder = HttpUtility.UrlDecode(nameOfNewFolder);

                Regex regex = new Regex(@"\\");
                path = regex.Replace(path, "", 1);

                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());
                string dirPath = string.IsNullOrEmpty(path) ? basePath : Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString(), path);

                if (Directory.Exists(dirPath))
                {
                    try
                    {
                        string newFolderPath = Path.Combine(dirPath, nameOfNewFolder);

                        if (Directory.Exists(newFolderPath))
                            return BadRequest("В текущем каталоге уже существует папка с указанным именем!");

                        Directory.CreateDirectory(newFolderPath);

                        DirectoryInfo directoryInfo = new DirectoryInfo(newFolderPath);

                        if (directoryInfo.Exists)
                        {
                            var directoryInfoObject = new
                            {
                                FileType = FileType.Directory,
                                Name = directoryInfo.Name,
                                Path = directoryInfo.FullName.Replace(basePath, "")
                            };

                            return Ok(directoryInfoObject);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost, DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> UploadFile(IFormFile file, string path, int companyId)
        {
            try
            {
                if (file is null)
                    throw new ArgumentException("Файл не может быть пустым!!!");

                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException("Путь не может быть пустым!!!");

                var company = await _unitOfWork.Company.FindById(companyId);
                if (company is null)
                    throw new ArgumentException("Компания не может быть индентифицированна!");

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new ArgumentException("Пользователь не может быть индентифицирован!");

                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());

                DirectoryInfo di = new DirectoryInfo(basePath);

                var currentSize = CalculateDirectorySize(di);

                if (currentSize + file.Length > MAX_CLOUD_SIZE)
                    throw new ArgumentException("Размер файла превышает объем свободного места на облачном хранилище!!!");

                path = HttpUtility.UrlDecode(path);
                Regex regex = new Regex(@"\\");
                path = regex.Replace(path, "", 1);

                string dirPath = string.IsNullOrEmpty(path) ? basePath : Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString(), path);

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                object result = null;
                string filePath = Path.Combine(dirPath, file.FileName);

                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        result = new
                        {
                            Status = false,
                            FileType = FileType.File,
                            Name = file.FileName,
                            Path = string.Empty,
                            Message = "Файл уже существует!"
                        };
                    }
                    else
                    {
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        FileInfo fileInfo = new FileInfo(filePath);

                        result = new
                        {
                            Status = true,
                            FileType = FileType.File,
                            Name = fileInfo.Name,
                            Path = fileInfo.FullName.Replace(basePath, ""),
                            Message = string.Empty
                        };
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    result = new
                    {
                        Status = false,
                        FileType = FileType.File,
                        Name = file.FileName,
                        Path = string.Empty,
                        Message = "При загрузке произошла ошибка!"
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFiles([FromBody]DeleteFilesModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new ArgumentException("User with the specified ID not found!!!");

                var company = await _unitOfWork.Company.FindById(model.CompanyId);
                if (company is null)
                    throw new ArgumentException("Company with the specified ID not found!!!");

                Regex regex = new Regex(@"\\");
                StringBuilder builder = new StringBuilder();
                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());

                List<object> resultList = new List<object>();
                foreach (var filePath in model.Paths)
                {
                    builder.Clear();
                    builder.Append(Path.Combine(basePath, regex.Replace(filePath, "", 1)));

                    try
                    {
                        if (System.IO.File.Exists(builder.ToString()))
                        {
                            // если данный файл является прикрепленным файлом 
                            // то не удаляем его иначе удаляем
                            var attachedFilePath = String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), filePath);
                            AttachedFile file = (await _unitOfWork.AttachedFile.Find(t => t.Path.Equals(attachedFilePath) && t.Owner.Id.Equals(user.Id)))?.FirstOrDefault();

                            if (file is null)
                            {
                                await _unitOfWork.BeginTransactionAsync();

                                try
                                {
                                    var path = builder.ToString().Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), "");
                                    var publicLink = (await _unitOfWork.FilePublicLink.Find(f => f.Path.Equals(path)))?.FirstOrDefault();

                                    if (publicLink != null)
                                    {
                                        var shortLink = (await _unitOfWork.ShortLink.Find(l => l.Full.Equals($"{_configuration["BaseDomain"]}share?code={publicLink.Link}")))?.FirstOrDefault();

                                        if (shortLink != null)
                                        {
                                            await _unitOfWork.ShortLink.Delete(shortLink.ShortLinkId);
                                        }

                                        await _unitOfWork.FilePublicLink.Delete(shortLink.ShortLinkId);
                                    }

                                    System.IO.File.Delete(builder.ToString());

                                    resultList.Add(new
                                    {
                                        path = filePath,
                                        isDeleted = true
                                    });

                                    await _unitOfWork.SaveChangesAsync();
                                    _unitOfWork.Commit();
                                }
                                catch (Exception ex)
                                {
                                    _unitOfWork.Rollback();
                                    throw ex;
                                }
                            }
                            else
                            {
                                resultList.Add(new
                                {
                                    path = Path.GetFileName(builder.ToString()),
                                    isDeleted = false,
                                    message = $"Вы не можете удалить {Path.GetFileName(builder.ToString())} так как он является прикрепленным файлом к задаче!!!"
                                });
                            }
                        }
                        else if (Directory.Exists(builder.ToString()))
                        {
                            bool isRemove = !await RemoveDirectoryWithControl(builder.ToString(), user.Id);

                            if (isRemove)
                            {
                                resultList.Add(new
                                {
                                    path = filePath,
                                    isDeleted = true
                                });
                            }
                            else
                            {
                                resultList.Add(new
                                {
                                    path = filePath,
                                    isDeleted = false,
                                    message = "Директория не может быть удалена так как она содержит прикрепленные файлы!!!"
                                });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        resultList.Add(new
                        {
                            path = Path.GetFileName(builder.ToString()),
                            isDeleted = false,
                            message = ""
                        });
                    }
                }

                return Ok(resultList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RenameFile(int companyId, string oldPath, string newName)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new ArgumentException("User with the specified ID not found!!!");

                var company = await _unitOfWork.Company.FindById(companyId);
                if (company is null)
                    throw new ArgumentException("Company with the specified ID not found!!!");

                if (string.IsNullOrEmpty(oldPath))
                    throw new ArgumentException("Старое имя файла не может быть пустым!");

                if (string.IsNullOrEmpty(newName))
                    throw new ArgumentException("Новое имя файла не может быть пустым!");

                Regex regex = new Regex(@"\\");

                oldPath = HttpUtility.UrlDecode(oldPath);
                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());
                string path = Path.Combine(basePath, regex.Replace(oldPath, "", 1));
                newName = HttpUtility.UrlDecode(newName);

                try
                {
                    if (System.IO.File.Exists(path))
                    {

                        string ext;
                        if (!string.IsNullOrEmpty((ext = Path.GetExtension(newName))))
                            newName = newName.Replace(ext, "");

                        ext = Path.GetExtension(path);
                        string newPath = Path.Combine(basePath, path.Replace(Path.GetFileName(path), ""), newName + ext);
                        System.IO.File.Move(path, newPath);

                        FileInfo fileInfo = new FileInfo(newPath);

                        if (fileInfo.Exists)
                        {
                            await _unitOfWork.BeginTransactionAsync();

                            try
                            {
                                var publicLinks = await _unitOfWork.FilePublicLink.Find(l => l.Path.Equals(String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), oldPath)));

                                if (publicLinks != null)
                                {
                                    foreach (var link in publicLinks)
                                    {
                                        link.Path = String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), fileInfo.FullName.Replace(basePath, ""));
                                    }

                                    await _unitOfWork.SaveChangesAsync();
                                }

                                var attachedFiles = await _unitOfWork.AttachedFile.Find(f => f.Path.Equals(String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), oldPath)));

                                if (attachedFiles != null)
                                {
                                    foreach (var file in attachedFiles)
                                    {
                                        file.Path = String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), fileInfo.FullName.Replace(basePath, ""));
                                    }
                                }

                                _unitOfWork.Commit();
                            }
                            catch (Exception)
                            {
                                _unitOfWork.Rollback();
                            }

                            var result = new
                            {
                                NewName = fileInfo.Name,
                                NewPath = fileInfo.FullName.Replace(basePath, ""),
                                Status = true,
                                oldPath = oldPath,
                            };

                            return Ok(result);
                        }
                    }
                    else if (Directory.Exists(path))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(path);
                        string newPath = Path.Combine(basePath, path.Replace(directoryInfo.Name, ""), newName);

                        if (!Directory.Exists(newPath))
                        {
                            Directory.Move(path, newPath);

                            directoryInfo = new DirectoryInfo(newPath);

                            if (directoryInfo.Exists)
                            {
                                await _unitOfWork.BeginTransactionAsync();

                                try
                                {
                                    var directoryOldPath = String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), oldPath);
                                    var publicLinks = await _unitOfWork.FilePublicLink.Find(l => l.Path.Contains(directoryOldPath));

                                    if (publicLinks != null)
                                    {
                                        foreach (var link in publicLinks)
                                        {
                                            link.Path = link.Path.Replace(directoryOldPath, String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), directoryInfo.FullName.Replace(basePath, "")));
                                        }

                                        await _unitOfWork.SaveChangesAsync();
                                    }

                                    var attachedFiles = await _unitOfWork.AttachedFile.Find(f => f.Path.Contains(directoryOldPath));

                                    if (attachedFiles != null)
                                    {
                                        foreach (var file in attachedFiles)
                                        {
                                            file.Path = file.Path.Replace(directoryOldPath, String.Format(@"\{0}\{1}{2}", user.Id, company.CompanyId.ToString(), directoryInfo.FullName.Replace(basePath, "")));
                                        }

                                        await _unitOfWork.SaveChangesAsync();
                                    }

                                    _unitOfWork.Commit();
                                }
                                catch (Exception)
                                {
                                    _unitOfWork.Rollback();
                                }


                                var result = new
                                {
                                    NewName = directoryInfo.Name,
                                    NewPath = directoryInfo.FullName.Replace(basePath, ""),
                                    Status = true,
                                    oldPath = oldPath
                                };

                                return Ok(result);
                            }
                        }
                        else
                            throw new Exception("Директория с указанным именем уже существует!");
                    }
                    else
                        throw new Exception("Файл или директория с указанным именем не найдено!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Move(int companyId, string oldPath, string newPath)
        {
            try
            {
                if (companyId <= 0)
                    return BadRequest("Компания не может быть индентифицированна!");

                if (string.IsNullOrEmpty(oldPath))
                    return BadRequest("Старый путь не может быть пустым!");

                if (string.IsNullOrEmpty(newPath))
                    return BadRequest("Новый путь не может быть пустым!");

                var user = await _userManager.GetUserAsync(HttpContext.User);
                var company = await _unitOfWork.Company.FindById(companyId);

                if (user != null && company != null)
                {
                    Regex regex = new Regex(@"\\");
                    string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());

                    oldPath = HttpUtility.UrlDecode(oldPath);
                    newPath = regex.Replace(HttpUtility.UrlDecode(newPath), "", 1);

                    string path = Path.Combine(basePath, regex.Replace(oldPath, "", 1));

                    if (System.IO.File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);

                        if (fileInfo.Exists)
                        {
                            string newFilePath = Path.Combine(basePath, newPath, fileInfo.Name);

                            if (!System.IO.File.Exists(newFilePath))
                                System.IO.File.Move(path, newFilePath);
                            else
                            {
                                int index = 0;
                                StringBuilder builder = new StringBuilder();

                                do
                                {
                                    if (index > 1000)
                                        break;

                                    builder.Clear();
                                    builder.Append(Path.Combine(basePath, newPath, fileInfo.Name.Replace(Path.GetExtension(fileInfo.Name), string.Empty) + $" ({++index})" + Path.GetExtension(fileInfo.Name)));
                                } while (System.IO.File.Exists(builder.ToString()));

                                System.IO.File.Move(path, builder.ToString());
                            }

                            return Ok(oldPath);
                        }
                        else
                            return BadRequest("Указанный файл не найден!");
                    }
                    else if (Directory.Exists(path))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(path);

                        if (directoryInfo.Exists)
                        {
                            string newFolderPath = Path.Combine(basePath, newPath, directoryInfo.Name);
                            if (!Directory.Exists(newFolderPath))
                                Directory.Move(path, newFolderPath);
                            else
                            {
                                int index = 0;
                                StringBuilder builder = new StringBuilder();

                                do
                                {
                                    if (index > 1000)
                                        break;

                                    builder.Clear();
                                    builder.Append(Path.Combine(basePath, newPath, directoryInfo.Name + $" ({++index})"));
                                } while (Directory.Exists(builder.ToString()));

                                Directory.Move(path, builder.ToString());
                            }

                            return Ok(oldPath);
                        }
                        else
                            return BadRequest("Указанная директория не найдена!");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string filePath, int companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("Путь элемента не может быть пустым!");

                var company = await _unitOfWork.Company.FindById(companyId);
                if (company is null)
                    throw new Exception("Компания не может быть индентифицированна!");

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new Exception("Пользователь не может быть индентифицирован!");

                filePath = HttpUtility.UrlDecode(filePath);

                Regex regex = new Regex(@"\\");

                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());
                string path = Path.Combine(basePath, regex.Replace(filePath, "", 1));

                if (System.IO.File.Exists(path))
                {
                    FileContentResult result = await FileUtils.GetFileContent(path, Path.GetFileName(path), HttpContext);
                    return result;
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo info = new DirectoryInfo(path);

                    if (info.Exists)
                    {
                        string compressedFile = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:TempFolderName"], info.Name + Guid.NewGuid() + ".zip");

                        try
                        {
                            ZipFile.CreateFromDirectory(path, compressedFile);

                            FileContentResult result = await FileUtils.GetFileContent(compressedFile, info.Name + ".zip", HttpContext);
                            return result;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (System.IO.File.Exists(compressedFile))
                                System.IO.File.Delete(compressedFile);
                        }
                    }
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFiles([FromBody]DownloadFilesModel model)
        {
            try
            {
                if (model.FilesPaths.Count == 0)
                    throw new Exception("Должен быть указан как минимум один элемент!");

                var company = await _unitOfWork.Company.FindById(model.CompanyId);
                if (company is null)
                    throw new Exception("Компания не может быть индентифицированна!");

                var user = await _userManager.GetUserAsync(HttpContext.User);
                user = await _unitOfWork.User.FindById(user.Id);

                if (user is null)
                    throw new Exception("Пользователь не может быть индентифицирован!");

                var foundCompany = user.CompanyEmployees.FirstOrDefault(c => c.CompanyId.Equals(company.CompanyId));
                if (foundCompany != null)
                {
                    Regex regex = new Regex(@"\\");
                    string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id,
                                                company.CompanyId.ToString());

                    string compressedFile = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:TempFolderName"], String.Format("{0}_{1}.zip", DateTime.Now.ToString("d-M-yyyy"), Guid.NewGuid().ToString().Replace("-", "")));

                    using (FileStream zipToOpen = new FileStream(compressedFile, FileMode.Create))
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                        {
                            foreach (var file in model.FilesPaths)
                            {
                                string path = Path.Combine(basePath, regex.Replace(HttpUtility.UrlDecode(file), "", 1));

                                if (System.IO.File.Exists(path))
                                {
                                    archive.CreateEntryFromFile(path, Path.GetFileName(path));
                                }
                                else if (Directory.Exists(path))
                                {
                                    AddFolderToArchive(archive, path, path);
                                }
                            }
                        }
                    }

                    try
                    {
                        FileContentResult result = await FileUtils.GetFileContent(compressedFile, Path.GetFileName(compressedFile), HttpContext);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (System.IO.File.Exists(compressedFile))
                            System.IO.File.Delete(compressedFile);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublicLink(string filePath, int companyId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new Exception("Пользователь не может быть индентифицирован!!!");

                var company = await _unitOfWork.Company.FindById(companyId);
                if (company is null)
                    throw new Exception("Компания не может быть индентифицированна!!!");

                Regex regex = new Regex(@"\\");
                StringBuilder builder = new StringBuilder();
                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());

                builder.Append(Path.Combine(basePath, regex.Replace(HttpUtility.UrlDecode(filePath), "", 1)));

                if (System.IO.File.Exists(builder.ToString()))
                {
                    // Создаем публичную ссылку на указанный файл
                    var publicLink = new FilePublicLink();
                    publicLink.Path = builder.ToString().Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), "");
                    publicLink.Owner = user;

                    // Проверяем что на указанный файл не существует публичной ссылки
                    FilePublicLink link = (await _unitOfWork.FilePublicLink.Find(l => l.Path.Equals(publicLink.Path) && l.Owner.Id.Equals(user.Id)))?.FirstOrDefault();

                    if (link is null)
                    {
                        int lenght = -1;
                        do
                        {
                            // Генерируем публичную ссылку
                            publicLink.Link = LinkGenerator.GenerateLongLink();

                            // Проверяем что сгенерированная ссылку не присутствует в БД
                            lenght = (await _unitOfWork.FilePublicLink.Find(p => p.Link.Equals(publicLink.Link))).Count();
                        }
                        while (lenght != 0);

                        await _unitOfWork.BeginTransactionAsync();

                        try
                        {
                            // Добавляем публичную ссылку на файл в БД
                            await _unitOfWork.FilePublicLink.Insert(publicLink);

                            int countShortLink = -1;
                            do
                            {
                                // Генерируем сокращенную ссылку
                                builder.Clear();
                                builder.Append(LinkGenerator.GenerateShortLink($"{_configuration["BaseDomain"]}u/"));
                                countShortLink = (await _unitOfWork.ShortLink.Find(l => l.Short.Equals(builder.ToString()))).Count();
                            }
                            while (countShortLink != 0);

                            var shortLink = new ShortLink
                            {
                                Short = builder.ToString(),
                                Full = $"{_configuration["BaseDomain"]}share?code={publicLink.Link}",
                                OwnerId = user.Id
                            };

                            await _unitOfWork.ShortLink.Insert(shortLink);
                            publicLink.ShortLink = shortLink;

                            await _unitOfWork.SaveChangesAsync();
                            _unitOfWork.Commit();

                            var result = new
                            {
                                status = true,
                                path = filePath,
                                link = shortLink.Short
                            };

                            return Ok(result);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            _unitOfWork.Rollback();
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        throw new Exception("У данного файла уже есть публичная ссылка!!!");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveLink(string link)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new Exception("Пользователь не может быть индентифицирован!!!");

                FilePublicLink publickLink = (await _unitOfWork.FilePublicLink.Find(l => l.ShortLink.Short.Equals(link) && l.Owner.Id.Equals(user.Id)))?.FirstOrDefault();

                if (publickLink is null)
                {
                    throw new Exception("Публичная ссылка с указанным кодом не найденна!!!");
                }
                else
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        await _unitOfWork.ShortLink.Delete(publickLink.ShortLink.ShortLinkId);
                        await _unitOfWork.FilePublicLink.Delete(publickLink.FilePublicLinkId);
                        await _unitOfWork.SaveChangesAsync();
                        _unitOfWork.Commit();

                        return Ok(new { status = true, link = link });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        _unitOfWork.Rollback();
                        throw new Exception(ex.Message);
                    }


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        private void AddFolderToArchive(ZipArchive archive, string path, string basePath)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (var file in dir.GetFiles())
            {
                if (file.Exists)
                {
                    Uri uri1 = new Uri(basePath);
                    Uri uri2 = new Uri(file.FullName);
                    string tempPath = uri1.MakeRelativeUri(uri2).ToString();
                    archive.CreateEntryFromFile(file.FullName, tempPath);
                }
            }

            foreach (var directory in dir.GetDirectories())
            {
                if (directory.Exists)
                {
                    AddFolderToArchive(archive, directory.FullName, basePath);
                }
            }
        }

        //private void RemoveDirectory(string path)
        //{
        //    try
        //    {
        //        foreach (var directory in Directory.GetDirectories(path))
        //        {
        //            if (Directory.Exists(directory))
        //                RemoveDirectory(directory);
        //        }

        //        foreach (var file in Directory.GetFiles(path))
        //        {
        //            try
        //            {
        //                if (System.IO.File.Exists(file))
        //                {
        //                    System.IO.File.Delete(file);
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                // ignored
        //            }
        //        }

        //        Directory.Delete(path);
        //    }
        //    catch (Exception)
        //    {
        //        // ignored
        //    }
        //}

        private async Task<bool> RemoveDirectoryWithControl(string path, string userId)
        {
            bool isFoundAttachedFile = false;

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                StringBuilder builder = new StringBuilder();

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (Directory.Exists(directory.FullName))
                    {
                        isFoundAttachedFile = await RemoveDirectoryWithControl(directory.FullName, userId) == false && isFoundAttachedFile == false ? false : true;
                    }
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    builder.Clear();

                    try
                    {
                        if (System.IO.File.Exists(file.FullName))
                        {
                            builder.Append(file.FullName.Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), ""));
                            AttachedFile attachedFile = (await _unitOfWork.AttachedFile.Find(t => t.Path.Equals(builder.ToString())))?.FirstOrDefault();

                            if (attachedFile is null)
                            {
                                try
                                {
                                    var filePath = builder.ToString().Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), "");
                                    var publicLink = (await _unitOfWork.FilePublicLink.Find(f => f.Path.Equals(filePath)))?.FirstOrDefault();

                                    if (publicLink != null)
                                    {
                                        var shortLink = (await _unitOfWork.ShortLink.Find(l => l.Full.Equals($"{_configuration["BaseDomain"]}share?code={publicLink.Link}")))?.FirstOrDefault();

                                        if (shortLink != null)
                                        {
                                            await _unitOfWork.ShortLink.Delete(shortLink.ShortLinkId);
                                        }

                                        await _unitOfWork.FilePublicLink.Delete(shortLink.ShortLinkId);
                                    }

                                    System.IO.File.Delete(file.FullName);

                                    await _unitOfWork.SaveChangesAsync();
                                    _unitOfWork.Commit();
                                }
                                catch (Exception)
                                {
                                    _unitOfWork.Rollback();
                                }
                            }
                            else
                                isFoundAttachedFile = true;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (!isFoundAttachedFile)
                    Directory.Delete(path);
            }
            catch (Exception)
            {
                // ignored
            }

            return isFoundAttachedFile;
        }

        private bool IsFoundPersonalFolder(string userId, string company)
        {
            string directoryPath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], userId, company);
            return Directory.Exists(directoryPath);
        }

        private bool CreatePersonalFolder(string userId, string company)
        {
            try
            {
                string directoryPath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], userId, company);

                if (!IsFoundPersonalFolder(userId, company))
                {
                    Directory.CreateDirectory(directoryPath);
                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }

        [HttpPost, DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> UpdloadTaskAttachedFiles(IFormFile file, int taskId, int companyId)
        {
            try
            {
                if (file is null)
                    throw new ArgumentException("Файл не может быть пустым!!!");

                var company = await _unitOfWork.Company.FindById(companyId);
                if (company is null)
                    throw new ArgumentException("Company with the specified ID not found!!!");

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new ArgumentException("User with the specified ID not found!!!");

                var task = await _unitOfWork.Task.FindById(taskId);
                if (task is null)
                    throw new ArgumentException("Task with the specified ID not found!!!");

                string basePath = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());

                DirectoryInfo di = new DirectoryInfo(basePath);

                var currentSize = CalculateDirectorySize(di);

                if (currentSize + file.Length > MAX_CLOUD_SIZE)
                    throw new ArgumentException("Размер файла превышает объем свободного места на облачном хранилище!!!");

                string attachedFilesFolderName = "Прикрепленные файлы";
                string dirPath = Path.Combine(basePath, attachedFilesFolderName);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                StringBuilder builder = new StringBuilder();
                builder.Append(Path.Combine(dirPath, file.FileName));

                if (System.IO.File.Exists(builder.ToString()))
                {
                    int index = 1;
                    do
                    {
                        builder.Clear();
                        builder.Append(Path.Combine(dirPath, file.FileName.Replace(Path.GetExtension(file.FileName), string.Empty) + $" ({++index})" + Path.GetExtension(file.FileName)));
                    } while (System.IO.File.Exists(builder.ToString()));
                }

                using (FileStream stream = new FileStream(builder.ToString(), FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileInfo = new FileInfo(builder.ToString());

                if (fileInfo.Exists)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        var attachedFile = new AttachedFile
                        {
                            Name = Path.GetFileName(builder.ToString()),
                            Path = builder.ToString().Replace(Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"]), ""),
                            Owner = user
                        };
                        await _unitOfWork.AttachedFile.Insert(attachedFile);
                        await _unitOfWork.SaveChangesAsync();

                        var taskAttachedFile = new TaskAttachedFile
                        {
                            AttachedFileId = attachedFile.AttachedFileId,
                            TaskId = task.TaskId
                        };

                        await _unitOfWork.TaskAttachedFile.Insert(taskAttachedFile);
                        await _unitOfWork.SaveChangesAsync();
                        _unitOfWork.Commit();

                        return Ok(true);
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetCloudSizeInfo(int companyId)
        {
            try
            {
                var company = await _unitOfWork.Company.FindById(companyId);
                if (company is null)
                    throw new Exception("Компания не может быть индентифицированна!");

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user is null)
                    throw new Exception("Пользователь не может быть индентифицирован!");

                string baseDirectory = Path.Combine(_configuration["Cloud:BasePath"], _configuration["Cloud:CloudFolderName"], user.Id, company.CompanyId.ToString());
                DirectoryInfo di = new DirectoryInfo(baseDirectory);

                var info = new
                {
                    size = CalculateDirectorySize(di),
                    maxSize = MAX_CLOUD_SIZE,
                };

                return Ok(info);
            }
            catch (DirectoryNotFoundException)
            {
                var info = new
                {
                    size = 0,
                    maxSize = MAX_CLOUD_SIZE,
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private long CalculateDirectorySize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => CalculateDirectorySize(di));
        }
    }
}