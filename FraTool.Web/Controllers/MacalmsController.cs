using dShared.Model;
using Macalms.Biz;
using Macalms.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraTool.Web.Controllers
{
    [Authorize]
    public class MacalmsController : Controller
    {
        private readonly IConfiguration _configuration;
        private SharedBiz sharedBiz;
        private EmployeeProfileBiz employeeProfileBiz;
        private StudentProfilBiz studentProfilBiz;
        private ExamResultBiz examResultBiz;
        private ScholarshipBiz scholarshipBiz;
        public MacalmsController(IConfiguration configuration)
        {
            _configuration = configuration;
            sharedBiz = new SharedBiz(_configuration);
            employeeProfileBiz = new EmployeeProfileBiz(_configuration);
            studentProfilBiz = new StudentProfilBiz(_configuration);
            examResultBiz = new ExamResultBiz(_configuration);
            scholarshipBiz = new ScholarshipBiz(_configuration);
        }
        #region Employee
        [Authorize]
        public IActionResult Employee()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EmpDepartment()
        {
            try
            {
                var dataSet = await sharedBiz.GetDepartments();
                var data = from c in dataSet
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.DepartmentName)
                        select new
                        {
                            c.RecordId,
                            c.DepartmentName
                        };
                return Json(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> EmpDesignation()
        {
            try
            {
                var dataSet = await sharedBiz.GetDesignations();
                var data = from c in dataSet
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.DesignationName)
                           select new
                           {
                               c.RecordId,
                               c.DesignationName
                           };
                return Json(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> EmpWorkLocation()
        {
            try
            {
                var dataSet = await sharedBiz.GetWorkLocations();
                var data = from c in dataSet
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.LocationName)
                           select new
                           {
                               c.RecordId,
                               c.LocationName
                           };
                return Json(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployeeProfile(EmployeeProfile model)
        {
            try
            {
                model.EntryBy = HttpContext.Session.GetString("UserName");
                var result = await employeeProfileBiz.AddEmployeeProfile(model);
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeProfile(EmployeeProfile model)
        {
            try
            {
                var result = 0;
                if (model.RecordId > 0)
                {
                    model.ModifyBy = HttpContext.Session.GetString("UserName");
                    result = await employeeProfileBiz.UpdateEmployeeProfile(model);
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeProfiles()
        {
            try
            {
                var dataSet = await employeeProfileBiz.GetEmployeeProfiles();
                return Json(data: dataSet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeEditView(long RecordId)
        {
            var data = new EmployeeProfile();
            if (RecordId > 0)
            {
                var employee = await employeeProfileBiz.GetEmployeeProfiles();
                data = employee.Where(c => c.RecordId == RecordId).SingleOrDefault();
            }
            return Json(data: data);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeEmployeeStatus(long RecordId)
        {
            try
            {
                var result = 0;
                if (RecordId > 0)
                {
                    var data = await employeeProfileBiz.GetEmployeeProfiles();
                    var model = data.Where(u => u.RecordId == RecordId).FirstOrDefault();
                    if (model != null)
                    {
                        if (model.IsActive == true)
                        {
                            model.IsActive = false;
                        }
                        else
                        {
                            model.IsActive = true;
                        }
                        model.ModifyBy = HttpContext.Session.GetString("UserName");
                        result = await employeeProfileBiz.ChangeEmployeeStatus(model);
                    }
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Student
        [Authorize]
        public IActionResult Students()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetParents()
        {
            try
            {
                var dataSet = await employeeProfileBiz.GetEmployeeProfiles();
                var data = from e in dataSet
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.EmployeeName)
                           select new
                           {
                               e.RecordId,
                               EmployeeName = e.EmployeeCode + " - " + e.EmployeeName
                           };
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                var dataSet = await studentProfilBiz.GetBanks();
                var data = from b in dataSet
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.BankName)
                           select new
                           {
                               b.RecordId,
                               b.BankName
                           };
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStudentCodeByParentCode(long ParentId)
        {
            try
            {
                var employeeRefCode = (await employeeProfileBiz.GetEmployeeProfiles()).FirstOrDefault(p => p.RecordId == ParentId)?.EmployeeCode;
                var studentCode = await studentProfilBiz.GetStudentCodeByParentCode(employeeRefCode ?? "");
                var data = new
                {
                    StudentCode = studentCode
                };
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddStudentProfile(StudentProfile model)
        {
            try
            {
                int result = 0;
                if(model != null)
                {
                    model.EntryBy = HttpContext.Session.GetString("UserName");
                    result = await studentProfilBiz.AddStudentProfile(model);
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStudentProfile(StudentProfile model)
        {
            try
            {
                var result = 0;
                if (model.RecordId > 0)
                {
                    model.ModifyBy = HttpContext.Session.GetString("UserName");
                    result = await studentProfilBiz.UpdateStudentProfile(model);
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> StudentEditView(long RecordId)
        {
            var data = new StudentProfile();
            if (RecordId > 0)
            {
                var student = await studentProfilBiz.GetStudentProfiles();
                data = student.Where(c => c.RecordId == RecordId).SingleOrDefault();
            }
            return Json(data: data);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStudentStatus(long RecordId)
        {
            int result = 0;            
            var model = (await studentProfilBiz.GetStudentProfiles()).Where(x => x.RecordId == RecordId).FirstOrDefault();
            if (model != null)
            {
                var parent = (await employeeProfileBiz.GetEmployeeProfiles()).Where(x => x.EmployeeCode == model.EmployeeRefCode).FirstOrDefault();
                if(parent!.IsActive == true)
                {
                    model.IsActive = !model.IsActive;
                    model.ModifyBy = HttpContext.Session.GetString("UserName");
                    result = await studentProfilBiz.ChangeStudentStatus(model);
                }
                else
                {
                    result = -99;
                }
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetStudentProfiles()
        {
            try
            {
                var dataSet = await studentProfilBiz.GetStudentProfiles();
                return Json(data: dataSet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Results
        [Authorize]
        public IActionResult Result()
        {
            return View();
        }
        public async Task<IActionResult> GetAssessmentYears()
        {
            try
            {
                var dataSet = await examResultBiz.GetAssessmentYearsAsync();
                var data = from c in dataSet
                        .OrderBy(x => x.YearName)
                           select new
                           {
                               c.RecordId,
                               c.YearName
                           };
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStudents(long ParentId)
        {
            try
            {
                var dataSet = (await studentProfilBiz.GetStudentProfiles()).Where(x=> x.ParentId ==ParentId).ToList();
                var data = from e in dataSet
                        .Where(x => x.IsActive == true)
                        .OrderBy(x => x.StudentCode)
                           select new
                           {
                               e.RecordId,
                               StudentName = e.StudentCode + " - " + e.StudentName
                           };
                return Json(data: data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveStudentResult(ExamResults model)
        {
            try
            {
                int result = 0;
                if(model != null)
                {
                    model.EntryBy = HttpContext.Session.GetString("UserName");
                    result = await examResultBiz.SaveStudentResult(model);
                }
                return Json(data: result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStudentResults()
        {
            try
            {
                var dataSet = await examResultBiz.GetExamResults();
                return Json(data: dataSet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ResultEditView(long RecordId)
        {
            var data = new ExamResults();
            if (RecordId > 0)
            {
                var result = await examResultBiz.GetExamResults();
                data = result.Where(c => c.RecordId == RecordId).SingleOrDefault();
            }
            return Json(data: data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateExamResult(ExamResults model)
        {
            try
            {
                var result = 0;
                if (model.RecordId > 0)
                {
                    model.ModifyBy = HttpContext.Session.GetString("UserName");
                    result = await examResultBiz.UpdateExamResult(model);
                }
                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Schlorship
        [Authorize]
        public IActionResult Scholarship()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetScholarshipData(int AssessmentYear)
        {
            try
            {
                var dataSet = await scholarshipBiz.GetScholarshipData(AssessmentYear);
                return Json(data: dataSet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
