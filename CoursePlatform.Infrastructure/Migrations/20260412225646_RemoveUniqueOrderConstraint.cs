using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueOrderConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections",
                columns: new[] { "CourseId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons",
                columns: new[] { "SectionId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections",
                columns: new[] { "CourseId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SectionId_Order",
                table: "Lessons",
                columns: new[] { "SectionId", "Order" },
                unique: true);
        }
    }
}
