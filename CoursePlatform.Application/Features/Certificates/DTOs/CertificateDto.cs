namespace CoursePlatform.Application.Features.Certificates.DTOs;

public class CertificateDto
{
    public int Id { get; set; }
    public string VerifyCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string VerifyUrl { get; set; } = string.Empty;
}