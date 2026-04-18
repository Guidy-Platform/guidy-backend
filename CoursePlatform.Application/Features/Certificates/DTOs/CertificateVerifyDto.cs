namespace CoursePlatform.Application.Features.Certificates.DTOs;

public class CertificateVerifyDto
{
    public bool IsValid { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}