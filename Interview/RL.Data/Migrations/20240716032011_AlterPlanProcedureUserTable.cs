using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RL.Data.Migrations
{
    public partial class AlterPlanProcedureUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlanProcedureUsers_ProcedureId",
                table: "PlanProcedureUsers",
                column: "ProcedureId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanProcedureUsers_Plans_PlanId",
                table: "PlanProcedureUsers",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanProcedureUsers_Procedures_ProcedureId",
                table: "PlanProcedureUsers",
                column: "ProcedureId",
                principalTable: "Procedures",
                principalColumn: "ProcedureId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanProcedureUsers_Plans_PlanId",
                table: "PlanProcedureUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanProcedureUsers_Procedures_ProcedureId",
                table: "PlanProcedureUsers");

            migrationBuilder.DropIndex(
                name: "IX_PlanProcedureUsers_ProcedureId",
                table: "PlanProcedureUsers");
        }
    }
}
