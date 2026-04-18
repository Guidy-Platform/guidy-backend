using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CoursePlatform.Infrastructure.Services;

public class PdfCertificateService : ICertificateService
{
    public Task<byte[]> GeneratePdfAsync(
        Certificate certificate,
        CancellationToken ct = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                // A4 Landscape
                page.Size(PageSizes.A4.Landscape());
                page.Margin(0);
                page.DefaultTextStyle(t => t.FontFamily("Arial"));

                page.Content().Element(BuildCertificate(certificate));
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    private static Action<IContainer> BuildCertificate(Certificate cert)
        => container =>
        {
            container
                .Background("#FAFAF7")
                .Padding(50)
                .Column(col =>
                {
                    col.Spacing(0);

                    // ─── Border ───────────────────────────────────────
                    col.Item()
                        .Border(3)
                        .BorderColor("#C9A84C")
                        .Padding(30)
                        .Column(inner =>
                        {
                            inner.Spacing(16);

                            // Platform Name
                            inner.Item()
                                .AlignCenter()
                                .Text("LearnHub Platform")
                                .FontSize(14)
                                .FontColor("#888888")
                                .LetterSpacing(3);

                            // Certificate Title
                            inner.Item()
                                .AlignCenter()
                                .Text("Certificate of Completion")
                                .FontSize(36)
                                .Bold()
                                .FontColor("#2C3E50");

                            // Divider
                            inner.Item()
                                .AlignCenter()
                                .Width(200)
                                .Height(2)
                                .Background("#C9A84C");

                            // Presented To
                            inner.Item()
                                .AlignCenter()
                                .PaddingTop(10)
                                .Text("This is to certify that")
                                .FontSize(14)
                                .FontColor("#666666");

                            // Student Name
                            inner.Item()
                                .AlignCenter()
                                .Text(cert.StudentName)
                                .FontSize(42)
                                .Bold()
                                .FontColor("#C9A84C");

                            // Has completed
                            inner.Item()
                                .AlignCenter()
                                .Text("has successfully completed the course")
                                .FontSize(14)
                                .FontColor("#666666");

                            // Course Name
                            inner.Item()
                                .AlignCenter()
                                .Text(cert.CourseName)
                                .FontSize(24)
                                .Bold()
                                .FontColor("#2C3E50");

                            // Divider
                            inner.Item()
                                .AlignCenter()
                                .Width(200)
                                .Height(1)
                                .Background("#DDDDDD");

                            // Bottom Row: Date + Instructor + Verify Code
                            inner.Item()
                                .PaddingTop(10)
                                .Row(row =>
                                {
                                    // Date
                                    row.RelativeItem()
                                        .AlignLeft()
                                        .Column(c =>
                                        {
                                            c.Item().Text("Date Issued")
                                                .FontSize(10)
                                                .FontColor("#999999");
                                            c.Item()
                                                .Text(cert.IssuedAt
                                                    .ToString("MMMM dd, yyyy"))
                                                .FontSize(13)
                                                .Bold()
                                                .FontColor("#2C3E50");
                                        });

                                    // Instructor
                                    row.RelativeItem()
                                        .AlignCenter()
                                        .Column(c =>
                                        {
                                            c.Item()
                                                .BorderBottom(1)
                                                .BorderColor("#2C3E50")
                                                .PaddingBottom(4)
                                                .Text(cert.InstructorName)
                                                .FontSize(13)
                                                .Bold()
                                                .FontColor("#2C3E50");
                                            c.Item()
                                                .AlignCenter()
                                                .Text("Instructor")
                                                .FontSize(10)
                                                .FontColor("#999999");
                                        });

                                    // Verify Code
                                    row.RelativeItem()
                                        .AlignRight()
                                        .Column(c =>
                                        {
                                            c.Item()
                                                .AlignRight()
                                                .Text("Verification Code")
                                                .FontSize(10)
                                                .FontColor("#999999");
                                            c.Item()
                                                .AlignRight()
                                                .Text(cert.VerifyCode)
                                                .FontSize(11)
                                                .Bold()
                                                .FontColor("#C9A84C");
                                        });
                                });
                        });
                });
        };
}