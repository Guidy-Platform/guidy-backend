using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoursePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "UserSubscriptions");

            //migrationBuilder.DropTable(
            //    name: "SubscriptionPlans");

            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_CourseId",
                table: "WishlistItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_StudentId",
                table: "WishlistItems",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_StudentId_CourseId",
                table: "WishlistItems",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishlistItems");

            //migrationBuilder.CreateTable(
            //    name: "SubscriptionPlans",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        BillingInterval = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        CertificateAccess = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        StripePriceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        StripeProductId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        UnlimitedCourseAccess = table.Column<bool>(type: "bit", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserSubscriptions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        PlanId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        AutoRenew = table.Column<bool>(type: "bit", nullable: false),
            //        CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        StripeCustomerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        StripeSubscriptionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserSubscriptions_SubscriptionPlans_PlanId",
            //            column: x => x.PlanId,
            //            principalTable: "SubscriptionPlans",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_UserSubscriptions_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserSubscriptions_PlanId",
            //    table: "UserSubscriptions",
            //    column: "PlanId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserSubscriptions_StripeSubscriptionId",
            //    table: "UserSubscriptions",
            //    column: "StripeSubscriptionId",
            //    unique: true,
            //    filter: "[StripeSubscriptionId] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserSubscriptions_UserId_Status",
            //    table: "UserSubscriptions",
            //    columns: new[] { "UserId", "Status" });
        }
    }
}
