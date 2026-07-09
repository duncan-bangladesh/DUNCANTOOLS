using dShared.Biz;
using dVehicle.Biz;
using dVehicle.Model;
using Microsoft.AspNetCore.Mvc;

namespace FraTool.Web.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IssueToBiz issueToBiz;
        private readonly OwnersBiz ownersBiz;
        private readonly VehicleTypeBiz vehicleTypeBiz;
        private readonly BRTAOfficeBiz bRTAOfficeBiz;
        private readonly DriverBiz driverBiz;
        private readonly VehicleBiz vehicleBiz;
        private readonly IssueLocationBiz issueLocationBiz;
        private readonly VehicleDocumentBiz vehicleDocumentBiz;
        public VehicleController(IConfiguration configuration)
        {
            _configuration = configuration;
            issueToBiz = new IssueToBiz(_configuration);
            ownersBiz = new OwnersBiz(_configuration);
            vehicleTypeBiz = new VehicleTypeBiz(_configuration);
            bRTAOfficeBiz = new BRTAOfficeBiz(_configuration);
            driverBiz = new DriverBiz(_configuration);
            vehicleBiz = new VehicleBiz(_configuration);
            issueLocationBiz = new IssueLocationBiz(_configuration);
            vehicleDocumentBiz = new VehicleDocumentBiz(_configuration);
        }
        #region Vehicle
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllVehicle()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveVehicle(Vehicles model)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    model.EntryBy = HttpContext.Session.GetString("UserName");
                    int result = await vehicleBiz.AddVehicleAsync(model);
                    if (result > 0)
                    {
                        return Ok(new { success = true, message = "Vehicle saved successfully." });
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Failed to save vehicle. Please try again." });
                    }
                }
                else
                {
                    return Unauthorized(new { success = false, message = "User session has expired. Please log in again." });
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the vehicle. Please try again later.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVehicle(Vehicles model)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    model.EntryBy = HttpContext.Session.GetString("UserName");
                    int result = await vehicleBiz.UpdateVehicleAsync(model);
                    if (result > 0)
                    {
                        return Ok(new { success = true, message = "Vehicle updated successfully." });
                    }
                    else
                    {
                        return Ok(new { success = false, message = "Failed to update vehicle. Please try again." });
                    }
                }
                else
                {
                    return Unauthorized(new { success = false, message = "User session has expired. Please log in again." });
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the vehicle. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> VehicleList()
        {
            try
            {
                var data = new List<Vehicles>();
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    var RoleId = Convert.ToInt32(HttpContext.Session.GetString("RoleId"));
                    var dSet = await vehicleBiz.GetVehiclesAsync();
                    if (RoleId == 8)
                    {
                        var CompanyId = Convert.ToInt32(HttpContext.Session.GetString("CompanyId"));
                        data = dSet.Where(x => x.CompanyId == CompanyId).ToList();
                    }
                    else {
                        data = dSet;
                    }
                }
                return Ok(data);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve vehicles. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Vehicle_dd()
        {
            try
            {
                var data = new List<Vehicles>();
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    var RoleId = Convert.ToInt32(HttpContext.Session.GetString("RoleId"));
                    var dSet = await vehicleBiz.GetVehiclesAsync();
                    if (RoleId == 8)
                    {
                        var CompanyId = Convert.ToInt32(HttpContext.Session.GetString("CompanyId"));
                        data = dSet.Where(x => x.CompanyId == CompanyId).ToList();
                    }
                    else
                    {
                        data = dSet;
                    }
                }
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.VehicleName)
                               select new
                               {
                                   c.RecordId,
                                   c.VehicleName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve vehicles. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> VehiclesEditView(long id = 0)
        {
            try
            {
                var data = new Vehicles();
                if (id > 0)
                {
                    var vehicles = await vehicleBiz.GetVehiclesAsync();
                    data = vehicles.Where(x => x.RecordId == id).FirstOrDefault();
                }
                return Ok(data);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve vehicle info.");
            }
        }
        #endregion
        #region Owners
        public IActionResult Owners()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> OwnersList()
        {
            try
            {
                return Ok(await ownersBiz.GetOwnersAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve owners. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Owners_dd()
        {
            try
            {
                var data = await ownersBiz.GetOwnersAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.OwnerName)
                               select new
                               {
                                   c.RecordId,
                                   c.OwnerName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve owners. Please try again later.");
            }
        }
        #endregion
        #region Issue Location
        public IActionResult IssueLocation()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> IssueLocationList()
        {
            try
            {
                return Ok(await issueLocationBiz.GetIssueLocationAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve issue locations. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> IssueLocation_dd()
        {
            try
            {
                var data = await issueLocationBiz.GetIssueLocationAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.LocationName)
                               select new
                               {
                                   c.RecordId,
                                   c.LocationName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve issue locations. Please try again later.");
            }
        }
        #endregion
        #region Issue User
        public IActionResult IssueTo()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> IssueToUser()
        {
            try
            {
                return Ok(await issueToBiz.GetIssueToAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve issue users. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> IssueToUser_dd()
        {
            try
            {
                var data = await issueToBiz.GetIssueToAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.ReceiverName)
                               select new
                               {
                                   c.RecordId,
                                   c.ReceiverName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve issue users. Please try again later.");
            }
        }
        #endregion
        #region Vehicle Type
        public IActionResult VehicleType()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> VehicleTypeList()
        {
            try
            {
                return Ok(await vehicleTypeBiz.GetVehicleTypeAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve vehicle types. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> VehicleType_dd()
        {
            try
            {
                var data = await vehicleTypeBiz.GetVehicleTypeAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.TypeName)
                               select new
                               {
                                   c.RecordId,
                                   c.TypeName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve vehicle types. Please try again later.");
            }
        }
        #endregion
        #region BRTA Office
        public IActionResult BRTAOffice()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> BRTAOfficeList()
        {
            try
            {
                return Ok(await bRTAOfficeBiz.GetBRTAOfficeAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve BRTA offices. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> BRTAOffice_dd()
        {
            try
            {
                var data = await bRTAOfficeBiz.GetBRTAOfficeAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.OfficeName)
                               select new
                               {
                                   c.RecordId,
                                   c.OfficeName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve BRTA offices. Please try again later.");
            }
        }
        #endregion
        #region Driver
        public IActionResult Driver()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DriverList()
        {
            try
            {
                return Ok(await driverBiz.GetDriversAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve drivers. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Driver_dd()
        {
            try
            {
                var data = await driverBiz.GetDriversAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.DriverName)
                               select new
                               {
                                   c.RecordId,
                                   c.DriverName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve drivers. Please try again later.");
            }
        }
        #endregion
        #region Document
        public IActionResult Documents()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> VehicleDocumentList()
        {
            try
            {
                var data = new List<VehicleDocument>();
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    var RoleId = Convert.ToInt32(HttpContext.Session.GetString("RoleId"));
                    var dSet = await vehicleDocumentBiz.GetVehicleDocuments();
                    if (RoleId == 8)
                    {
                        var CompanyId = Convert.ToInt32(HttpContext.Session.GetString("CompanyId"));
                        data = dSet.Where(x => x.CompanyId == CompanyId).ToList();
                    }
                    else
                    {
                        data = dSet;
                    }
                }
                return Ok(data);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve documents. Please try again later.");
            }
        }
        [HttpGet("Vehicle/Download")]
        public IActionResult Download(string path)
        {
            var basePath = _configuration["ApplicationConfig:VehicleAttachmentPath"];
            var fullPath = Path.Combine(basePath!, path.Replace("/", "\\"));
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var contentType = "application/octet-stream";
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, contentType, Path.GetFileName(fullPath));
        }
        [HttpPost]
        public async Task<IActionResult> SaveDocument([FromForm] VehicleDocument model)
        {
            string? fullPath = null;
            string? dbPath = null;
            try
            {
                if (model.DocumentAttachment == null || model.DocumentAttachment.Length == 0)
                    return BadRequest("Please select a file.");

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                var extension = Path.GetExtension(model.DocumentAttachment.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Only PDF and Image files are allowed.");

                string uploadsFolder = _configuration["ApplicationConfig:VehicleAttachmentPath"] ?? throw new InvalidOperationException("Attachment Path is not configured.");
                string safeVehicleName = GetSafeFolderName(model.VehicleName!);
                string vehicleFolder = Path.Combine(uploadsFolder, safeVehicleName);
                Directory.CreateDirectory(vehicleFolder);
                string uploadDateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"{model.VehicleName}_{uploadDateTime}_{model.DocumentTypeName}{extension}";
                fullPath = Path.Combine(vehicleFolder, fileName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.DocumentAttachment.CopyToAsync(stream);
                }
                dbPath = Path.Combine(safeVehicleName, fileName).Replace("\\", "/");

                model.DocumentAttachment = null;
                model.FilePath = dbPath;
                model.EntryBy = HttpContext.Session.GetString("UserName");

                int result = 0;
                result = await vehicleDocumentBiz.SaveVehicleDocument(model);

                if (result <= 0)
                {
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = "DB save failed. File has been rolled back."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Document uploaded successfully.",
                    FileName = fileName,
                    FilePath = dbPath
                });
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while uploading the document.",
                    Error = ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> VehicleDocumentEditView(long id = 0)
        {
            try
            {
                var data = new VehicleDocument();
                if (id > 0)
                {
                    var documents = await vehicleDocumentBiz.GetVehicleDocuments();
                    data = documents.Where(x => x.RecordId == id).FirstOrDefault();
                }
                return Ok(data);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve vehicle documents info.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVehicleDocuments([FromForm] VehicleDocument model)
        {
            string? fullPath = null;
            string? dbPath = null;
            try
            {
                if (HttpContext.Session.GetString("UserName") != null)
                {
                    if ((model.FileUrl != null && model.FileUrl != "") && (model.DocumentAttachment != null && model.DocumentAttachment.Length > 0))
                    {
                        var vehiclePhysicalPath = (await vehicleDocumentBiz.GetVehicleDocuments()).Where(x => x.RecordId == model.RecordId).FirstOrDefault()!.PhysicalPath!;
                        System.IO.File.Delete(vehiclePhysicalPath);

                        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                        var extension = Path.GetExtension(model.DocumentAttachment.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                            return BadRequest("Only PDF and Image files are allowed.");

                        string uploadsFolder = _configuration["ApplicationConfig:VehicleAttachmentPath"] ?? throw new InvalidOperationException("Attachment Path is not configured.");
                        string safeVehicleName = GetSafeFolderName(model.VehicleName!);
                        string vehicleFolder = Path.Combine(uploadsFolder, safeVehicleName);
                        Directory.CreateDirectory(vehicleFolder);
                        string uploadDateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string fileName = $"{model.VehicleName}_{uploadDateTime}_{model.DocumentTypeName}{extension}";
                        fullPath = Path.Combine(vehicleFolder, fileName);

                        await using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await model.DocumentAttachment.CopyToAsync(stream);
                        }
                        dbPath = Path.Combine(safeVehicleName, fileName).Replace("\\", "/");

                        model.DocumentAttachment = null;
                        model.FilePath = dbPath;
                        model.ModifyBy = HttpContext.Session.GetString("UserName");

                        int result = 0;
                        result = await vehicleDocumentBiz.UpdateVehicleDocumentAsync(model);

                        if (result <= 0)
                        {
                            if (System.IO.File.Exists(fullPath))
                                System.IO.File.Delete(fullPath);
                            return StatusCode(500, new
                            {
                                Success = false,
                                Message = "DB save failed. File has been rolled back."
                            });
                        }

                        return Ok(new
                        {
                            Success = true,
                            Message = "Document uploaded successfully.",
                            FileName = fileName,
                            FilePath = dbPath
                        });
                    }
                    else
                    {
                        return BadRequest("Please select a file.");
                    }
                }
                else
                {
                    return Unauthorized(new { success = false, message = "User session has expired. Please log in again." });
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while uploading the document.",
                    Error = ex.Message
                });
            }
        }
        private static string GetSafeFolderName(string folderName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                folderName = folderName.Replace(c, '_');
            }
            return folderName.Trim();
        }
        #endregion
        #region Document Type
        public IActionResult DocumentType()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DocumentTypeList()
        {
            try
            {
                return Ok(await new DocumentTypeBiz(_configuration).GetDocumentTypeAsync());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve document types. Please try again later.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DocumentType_dd()
        {
            try
            {
                var data = await new DocumentTypeBiz(_configuration).GetDocumentTypeAsync();
                var response = from c in data
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.DocumentTypeName)
                               select new
                               {
                                   c.RecordId,
                                   c.DocumentTypeName
                               };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve document types. Please try again later.");
            }
        }
        #endregion
    }
}
