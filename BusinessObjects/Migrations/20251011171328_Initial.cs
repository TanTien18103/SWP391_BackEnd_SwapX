using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    accountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OtpCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OtpExpiredTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__F267253E9C6E442D", x => x.accountID);
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    PackageID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Battery_type = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    expiredDate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Package__322035EC6A45A890", x => x.PackageID);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    stationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    battery_number = table.Column<int>(type: "int", nullable: true),
                    location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    rating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    stationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Station__F0A7F3E0EE3A1E80", x => x.stationID);
                });

            migrationBuilder.CreateTable(
                name: "EVDriver",
                columns: table => new
                {
                    CustomerID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    accountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EVDriver__A4AE64B868918DE6", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK__EVDriver__accoun__5DCAEF64",
                        column: x => x.accountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                });

            migrationBuilder.CreateTable(
                name: "Battery",
                columns: table => new
                {
                    BatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    Battery_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    stationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    specification = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    batteryQuality = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    batteryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Battery__5710803E287D4B4A", x => x.BatteryID);
                    table.ForeignKey(
                        name: "FK__Battery__station__5441852A",
                        column: x => x.stationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "BSS_Staff",
                columns: table => new
                {
                    StaffID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BSS_Staf__96D4AAF7F2826E50", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK__BSS_Staff__Accou__5BE2A6F2",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__BSS_Staff__Stati__5CD6CB2B",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    FormID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Form__FB05B7BD4218BBBD", x => x.FormID);
                    table.ForeignKey(
                        name: "FK__Form__AccountID__66603565",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__Form__StationID__6754599E",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    RatingID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    rating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rating__FCCDF85CD48F902B", x => x.RatingID);
                    table.ForeignKey(
                        name: "FK__Rating__AccountI__6B24EA82",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__Rating__StationI__6C190EBB",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReportID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    image = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Report__D5BD48E504DC6B1F", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK__Report__AccountI__6D0D32F4",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__Report__StationI__6E01572D",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "BatteryReport",
                columns: table => new
                {
                    BatteryReportID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BatteryR__0A361DA7C09CCE11", x => x.BatteryReportID);
                    table.ForeignKey(
                        name: "FK__BatteryRe__Accou__59063A47",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__BatteryRe__Batte__59FA5E80",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__BatteryRe__Stati__5AEE82B9",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    service_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BAF8CE09CA2", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK__Orders__AccountI__68487DD7",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__Orders__BatteryI__693CA210",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                });

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    SlotID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cordinate_x = table.Column<int>(type: "int", nullable: true),
                    cordinate_y = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Slot__0A124A4F88119764", x => x.SlotID);
                    table.ForeignKey(
                        name: "FK__Slot__BatteryID__6EF57B66",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__Slot__StationID__6FE99F9F",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    VIN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    vehicle_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vehicle_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PackageID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CustomerID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PackageExpiredate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vehicle__C5DF234D17ECC187", x => x.VIN);
                    table.ForeignKey(
                        name: "FK__Vehicle__Battery__72C60C4A",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__Vehicle__Custome__01142BA1",
                        column: x => x.CustomerID,
                        principalTable: "EVDriver",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK__Vehicle__Package__73BA3083",
                        column: x => x.PackageID,
                        principalTable: "Package",
                        principalColumn: "PackageID");
                });

            migrationBuilder.CreateTable(
                name: "StationSchedule",
                columns: table => new
                {
                    StationScheduleID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FormID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StationS__12BF61BEC9CF80C5", x => x.StationScheduleID);
                    table.ForeignKey(
                        name: "FK__StationSc__FormI__70DDC3D8",
                        column: x => x.FormID,
                        principalTable: "Form",
                        principalColumn: "FormID");
                    table.ForeignKey(
                        name: "FK__StationSc__Stati__71D1E811",
                        column: x => x.StationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                });

            migrationBuilder.CreateTable(
                name: "ExchangeBattery",
                columns: table => new
                {
                    ExchangeBatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OldBatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewBatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StaffAccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScheduleID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    stationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Exchange__B321FE92B7665EFC", x => x.ExchangeBatteryID);
                    table.ForeignKey(
                        name: "FK__ExchangeB__NewBa__5FB337D6",
                        column: x => x.NewBatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__ExchangeB__OldBa__60A75C0F",
                        column: x => x.OldBatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__ExchangeB__Order__619B8048",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__ExchangeB__Sched__628FA481",
                        column: x => x.ScheduleID,
                        principalTable: "StationSchedule",
                        principalColumn: "StationScheduleID");
                    table.ForeignKey(
                        name: "FK__ExchangeB__Staff__6383C8BA",
                        column: x => x.StaffAccountID,
                        principalTable: "Account",
                        principalColumn: "accountID");
                    table.ForeignKey(
                        name: "FK__ExchangeB__stati__6477ECF3",
                        column: x => x.stationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                    table.ForeignKey(
                        name: "FK__ExchangeBat__VIN__656C112C",
                        column: x => x.VIN,
                        principalTable: "Vehicle",
                        principalColumn: "VIN");
                });

            migrationBuilder.CreateTable(
                name: "BatteryHistory",
                columns: table => new
                {
                    BatteryHistoryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExchangeBatteryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VIN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    stationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnergyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BatteryH__4E7644D24A446D7E", x => x.BatteryHistoryID);
                    table.ForeignKey(
                        name: "FK__BatteryHi__Batte__5535A963",
                        column: x => x.BatteryID,
                        principalTable: "Battery",
                        principalColumn: "BatteryID");
                    table.ForeignKey(
                        name: "FK__BatteryHi__Excha__5629CD9C",
                        column: x => x.ExchangeBatteryID,
                        principalTable: "ExchangeBattery",
                        principalColumn: "ExchangeBatteryID");
                    table.ForeignKey(
                        name: "FK__BatteryHi__stati__571DF1D5",
                        column: x => x.stationID,
                        principalTable: "Station",
                        principalColumn: "stationID");
                    table.ForeignKey(
                        name: "FK__BatteryHist__VIN__5812160E",
                        column: x => x.VIN,
                        principalTable: "Vehicle",
                        principalColumn: "VIN");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Battery_stationID",
                table: "Battery",
                column: "stationID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_BatteryID",
                table: "BatteryHistory",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_ExchangeBatteryID",
                table: "BatteryHistory",
                column: "ExchangeBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_stationID",
                table: "BatteryHistory",
                column: "stationID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryHistory_VIN",
                table: "BatteryHistory",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReport_AccountID",
                table: "BatteryReport",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReport_BatteryID",
                table: "BatteryReport",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryReport_StationID",
                table: "BatteryReport",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_BSS_Staff_AccountID",
                table: "BSS_Staff",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_BSS_Staff_StationID",
                table: "BSS_Staff",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_EVDriver_accountID",
                table: "EVDriver",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_NewBatteryID",
                table: "ExchangeBattery",
                column: "NewBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_OldBatteryID",
                table: "ExchangeBattery",
                column: "OldBatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_OrderID",
                table: "ExchangeBattery",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_ScheduleID",
                table: "ExchangeBattery",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_StaffAccountID",
                table: "ExchangeBattery",
                column: "StaffAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_stationID",
                table: "ExchangeBattery",
                column: "stationID");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeBattery_VIN",
                table: "ExchangeBattery",
                column: "VIN");

            migrationBuilder.CreateIndex(
                name: "IX_Form_AccountID",
                table: "Form",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Form_StationID",
                table: "Form",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AccountID",
                table: "Orders",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BatteryID",
                table: "Orders",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_AccountID",
                table: "Rating",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_StationID",
                table: "Rating",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_AccountID",
                table: "Report",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_StationID",
                table: "Report",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_BatteryID",
                table: "Slot",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_StationID",
                table: "Slot",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_StationSchedule_FormID",
                table: "StationSchedule",
                column: "FormID");

            migrationBuilder.CreateIndex(
                name: "IX_StationSchedule_StationID",
                table: "StationSchedule",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_BatteryID",
                table: "Vehicle",
                column: "BatteryID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_CustomerID",
                table: "Vehicle",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_PackageID",
                table: "Vehicle",
                column: "PackageID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatteryHistory");

            migrationBuilder.DropTable(
                name: "BatteryReport");

            migrationBuilder.DropTable(
                name: "BSS_Staff");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "ExchangeBattery");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "StationSchedule");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Form");

            migrationBuilder.DropTable(
                name: "Battery");

            migrationBuilder.DropTable(
                name: "EVDriver");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
