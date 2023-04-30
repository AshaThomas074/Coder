using Coder.Data;
using Coder.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Data;
using System.IO;
using System.Security.Claims;

namespace Coder.Controllers
{
    public class BulkUploadController : Controller
    {
        IConfiguration configuration;
        IWebHostEnvironment hostEnvironment;
        IExcelDataReader reader;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser> _userStore;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        private readonly CoderDBContext _coderDBContext;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public BulkUploadController(IConfiguration configuration,
            IWebHostEnvironment hostEnvironment,
            UserManager<ApplicationUser> userManager,
            CoderDBContext coderDBContext,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IUserStore<ApplicationUser> userStore,
            IPasswordHasher<ApplicationUser> passwordHasher,
            SignInManager<ApplicationUser> signInManager)
        {
            this.configuration = configuration;
            this.hostEnvironment = hostEnvironment;
            this._userManager = userManager;
            _coderDBContext = coderDBContext;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _userStore = userStore;
            _passwordHasher = passwordHasher;
        }
        [HttpGet]
        public IActionResult Index(BulkUploadModel bulkupload = null)
        {
            if (TempData["model"] != null)
            {
                //bulkupload = JsonConvert.DeserializeObject<BulkUploadModel>TempData["model"];
                bulkupload = JsonConvert.DeserializeObject<BulkUploadModel>(TempData["model"].ToString());
                TempData["model"] = null;
            }
            return View(bulkupload);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                // Check the File is received

                if (file == null)
                    throw new Exception("File is Not Received...");

                List<ApplicationUser> students = new List<ApplicationUser>();

                // Create the Directory if it is not exist
                string dirPath = Path.Combine(hostEnvironment.WebRootPath, "uploadedfiles");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // Make sure that only Excel file is used 
                string dataFileName = Path.GetFileNameWithoutExtension(file.FileName);

                string extension = Path.GetExtension(file.FileName);

                string[] allowedExtensions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtensions.Contains(extension))
                    throw new Exception("Sorry! This file is not allowed,make sure that file having extension as either.xls or.xlsx is uploaded.");

                // Make a Copy of the Posted File from the Received HTTP Request
                string newname = "bulkupload_" + userId + "_" + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + "" + extension;
                string saveToPath = Path.Combine(dirPath, newname);

                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                students = ReadExcelFile(saveToPath);

                BulkUploadModel bulkupload = new BulkUploadModel();
                bulkupload.FileName = saveToPath;
                bulkupload.Users = students;
                return Index(bulkupload);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel()
                {

                });
            }
        }


        [HttpGet("download")]
        public async Task<IActionResult> download()
        {

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", "bulkupload.xlsx");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        [HttpPost]
        public IActionResult SaveAllTheStudents(string FileName)
        {
            var students = ReadExcelFile(FileName);
            ResponseModel result=SaveStudents(students);
            BulkUploadModel model = new();
            model.Response = result;
            TempData["model"] = JsonConvert.SerializeObject(model);
            return RedirectToAction("Index");
        }

        private List<ApplicationUser> ReadExcelFile(string FileName)
        {
            List<ApplicationUser> students = new List<ApplicationUser>();
            // USe this to handle Encodeing differences in .NET Core
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // read the excel file
            using (var stream = new FileStream(FileName, FileMode.Open))
            {
                var extension = Path.GetExtension(FileName);
                if (extension == ".xls")
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                else
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                DataSet ds = new DataSet();
                ds = reader.AsDataSet();
                reader.Close();


                if (ds != null && ds.Tables.Count > 0)
                {
                    // Read the the Table
                    DataTable data = ds.Tables[0];
                    for (int i = 1; i < data.Rows.Count; i++)
                    {
                        students.Add(new ApplicationUser
                        {
                            FirstName = data.Rows[i][0].ToString(),
                            LastName = data.Rows[i][1].ToString(),
                            Email = data.Rows[i][2].ToString(),
                            UserExternalId = data.Rows[i][3].ToString(),
                            PasswordHash = data.Rows[i][4].ToString(),
                            StudentBatchName = data.Rows[i][5].ToString()
                        });
                    }
                }
            }
            return students;
        }
        private ResponseModel SaveStudents(List<ApplicationUser> students)
        {
            int count = 0;
            try
            {
                var userManager = new UserStore<ApplicationUser>(_coderDBContext);
                var emails = students.Select(x => x.Email).ToList();
                var users = _coderDBContext.Users.Where(u => emails.Contains(u.UserName)).ToList();

                var roleid = _coderDBContext.Roles.Where(x => x.Name == "Student").FirstOrDefault().Id;
                
                foreach (var userName in emails)
                {
                    var user = users.Where(u => u.UserName == userName).FirstOrDefault();

                    if (user == null)
                    {                        
                        var data = students.Where(x => x.Email == userName).FirstOrDefault();
                        if (data != null)
                        {
                            var stud = CreateUser();

                            stud.FirstName = data.FirstName;
                            stud.LastName = data.LastName;
                            stud.UserExternalId = data.UserExternalId;
                            stud.StudentBatchId = _coderDBContext.StudentBatch.Where(x => x.StudentBatchName == data.StudentBatchName).FirstOrDefault().StudentBatchId;
                            stud.CreatedOn = DateTime.Now;
                            stud.UpdatedOn = DateTime.Now;
                            stud.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);//get the Id of loggined user
                            stud.UserName = userName;
                            stud.NormalizedUserName = userName.ToUpper();
                            stud.Email = userName;
                            stud.NormalizedEmail = userName.ToUpper();
                            stud.EmailConfirmed = false;
                            stud.PhoneNumberConfirmed = false;
                            stud.TwoFactorEnabled = false;
                            stud.LockoutEnabled = false;
                            stud.PasswordHash = _passwordHasher.HashPassword(stud, data.PasswordHash);
                            stud.SecurityStamp = Guid.NewGuid().ToString().ToLower();
                            _coderDBContext.Users.Add(stud);
                            _coderDBContext.SaveChanges();

                            var studId = stud.Id;
                            IdentityUserRole<string> role = new IdentityUserRole<string>();
                            role.RoleId = roleid;
                            role.UserId = studId;
                            _coderDBContext.UserRoles.Add(role);
                            _coderDBContext.SaveChanges();
                            ++count;
                        }
                    }
                }
                ResponseModel model = new();
                return model = new()
                {
                    IsSuccess = true,
                    Message = count+" out of "+students.Count().ToString()+" registered."
                };
            }
            catch (Exception ex)
            {
                ResponseModel model = new();
                return model = new()
                {
                    IsSuccess = false,
                    Message = count + " out of " + students.Count().ToString() + " registered. Something went wrong."
                };
            }
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }

      
}
