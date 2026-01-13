using dCommon;

namespace Macalms.Model
{
    public class ExamResults : DbBase
    {
        public long RecordId { get; set; }
        public long ParentId { get; set; }
        public long StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? ClassStudied { get; set; }
        public string? NameOfTheInstitution { get; set; }
        public string? StudyMedium { get; set; }
        public string? AcademyType { get; set; }
        public string? ExamResult { get; set; }
        public long AssessmentYearId { get; set; }
        public string? AssessmentYear { get; set; }
    }
}
