namespace CoursePlatform.Domain.Enums;

public enum NotificationType
{
    // Student notifications
    OrderCompleted = 1,
    CourseEnrolled = 2,
    CertificateIssued = 3,

    // Instructor notifications
    CoursePublished = 4,
    CourseRejected = 5,
    ReviewReceived = 6,
    NewEnrollment = 7,

    // General
    WelcomeMessage = 8,
    SystemMessage = 9
}