using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAPI.Migrations
{
    public partial class renameUserIdPropertyFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_UserId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Cities_CityId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_AspNetUsers_UserId",
                table: "Responses");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Responses",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_UserId",
                table: "Responses",
                newName: "IX_Responses_TenantId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Properties",
                newName: "LandlordId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_UserId",
                table: "Properties",
                newName: "IX_Properties_LandlordId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                newName: "IX_Comments_TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_TenantId",
                table: "Comments",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_LandlordId",
                table: "Properties",
                column: "LandlordId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Cities_CityId",
                table: "Properties",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_AspNetUsers_TenantId",
                table: "Responses",
                column: "TenantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_TenantId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_LandlordId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Cities_CityId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_AspNetUsers_TenantId",
                table: "Responses");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Responses",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_TenantId",
                table: "Responses",
                newName: "IX_Responses_UserId");

            migrationBuilder.RenameColumn(
                name: "LandlordId",
                table: "Properties",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_LandlordId",
                table: "Properties",
                newName: "IX_Properties_UserId");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Comments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TenantId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_UserId",
                table: "Properties",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Cities_CityId",
                table: "Properties",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_AspNetUsers_UserId",
                table: "Responses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
