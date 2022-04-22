using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Profile.Migrations
{
    public partial class newdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Consumers",
                keyColumn: "Id",
                keyValue: new Guid("1e15d57c-26e3-4e78-94f9-8649b3302555"));

            migrationBuilder.DeleteData(
                table: "SourceParameter",
                keyColumns: new[] { "JsonPath", "SourceId", "Type" },
                keyValues: new object[] { "Message.data.amount", 1, 4 });

            migrationBuilder.DeleteData(
                table: "SourceParameter",
                keyColumns: new[] { "JsonPath", "SourceId", "Type" },
                keyValues: new object[] { "Message.data.amount", 101, 3 });

            migrationBuilder.DeleteData(
                table: "SourceParameter",
                keyColumns: new[] { "JsonPath", "SourceId", "Type" },
                keyValues: new object[] { "Message.data.amount", 101, 4 });

            migrationBuilder.DeleteData(
                table: "SourceParameter",
                keyColumns: new[] { "JsonPath", "SourceId", "Type" },
                keyValues: new object[] { "Message.data.amount", 102, 4 });

            migrationBuilder.DeleteData(
                table: "SourceServices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SourceServices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 10101);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "Id",
                keyValue: new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                columns: new[] { "Client", "SourceId", "User" },
                values: new object[] { 0L, 1, 0L });

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "Id",
                keyValue: new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"),
                columns: new[] { "Filter", "User" },
                values: new object[] { "", 0L });

            migrationBuilder.UpdateData(
                table: "SourceServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceUrl",
                value: "http://notification-enrichment.test-notification-enrichment.svc:5000/GetTelephoneNumber");

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApiKey", "DisplayType", "KafkaUrl", "Secret", "SmsServiceReference", "Title_EN", "Title_TR", "Topic" },
                values: new object[] { "", 1, "cluster-kafka-tls-bootstrap-test-kafka4cdc.apps.nonprod.ebt.bank:443", "", "9cab7fdc-76a4-44be-b6fa-101f13729875", "CashBackEN", "CashBackTR", "CAMPAIGN_CASHBACK_ACCOUNTING_INFO" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "Id",
                keyValue: new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"),
                columns: new[] { "Filter", "User" },
                values: new object[] { "Message.data.amount >= 500000", 123456L });

            migrationBuilder.InsertData(
                table: "Consumers",
                columns: new[] { "Id", "Client", "DeviceKey", "Email", "Filter", "IsEmailEnabled", "IsPushEnabled", "IsSmsEnabled", "SourceId", "User", "Phone_CountryCode", "Phone_Number", "Phone_Prefix" },
                values: new object[] { new Guid("1e15d57c-26e3-4e78-94f9-8649b3302555"), 123456L, null, null, "Message.data.amount >= 500 && Message.data.iban ==\"TR1234567\"", false, false, true, 1, 123456L, 90, 3855206, 530 });

            migrationBuilder.InsertData(
                table: "SourceParameter",
                columns: new[] { "JsonPath", "SourceId", "Type", "Title_EN", "Title_TR" },
                values: new object[] { "Message.data.amount", 1, 4, "Amount", "Tutar" });

            migrationBuilder.UpdateData(
                table: "SourceServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceUrl",
                value: "localhost:/getcustomerId");

            migrationBuilder.InsertData(
                table: "SourceServices",
                columns: new[] { "Id", "ServiceUrl", "SourceId" },
                values: new object[,]
                {
                    { 2, "localhost:/get2customerId", 1 },
                    { 3, "localhost:/get3customerId", 101 }
                });

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApiKey", "DisplayType", "KafkaUrl", "Secret", "SmsServiceReference", "Title_EN", "Title_TR", "Topic" },
                values: new object[] { "a1b2c33d4e5f6g7h8i9jakblc", 5, "test", "11561681-8ba5-4b46-bed0-905ae1769bc6", "notify_sms_incoming_eft", "Incoming EFT", "Gelen EFT", "http://localhost:8082/topics/cdc_eft/incoming_eft" });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "ApiKey", "ClientIdJsonPath", "DisplayType", "EmailServiceReference", "KafkaUrl", "ParentId", "PushServiceReference", "Secret", "SmsServiceReference", "Title_EN", "Title_TR", "Topic" },
                values: new object[,]
                {
                    { 101, "a1b2c33d4e5f6g7h8i9jakblc", null, 4, "notify_email_incoming_fast", "test", 1, "notify_push_incoming_fast", "11561681-8ba5-4b46-bed0-905ae1769bc6", "notify_sms_incoming_fast", "Incoming FAST", "Gelen FAST", "http://localhost:8082/topics/cdc_eft/incoming_fast" },
                    { 102, "a1b2c33d4e5f6g7h8i9jakblc", null, 4, "notify_email_incoming_qr", "test", 1, "notify_push_incoming_qr", "11561681-8ba5-4b46-bed0-905ae1769bc6", "notify_sms_incoming_qr", "Incoming QR", "Gelen QR", "http://localhost:8082/topics/cdc_eft/incoming_qr" }
                });

            migrationBuilder.UpdateData(
                table: "Consumers",
                keyColumn: "Id",
                keyValue: new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                columns: new[] { "Client", "SourceId", "User" },
                values: new object[] { 123456L, 102, 123456L });

            migrationBuilder.InsertData(
                table: "SourceParameter",
                columns: new[] { "JsonPath", "SourceId", "Type", "Title_EN", "Title_TR" },
                values: new object[,]
                {
                    { "Message.data.amount", 101, 3, "Amount", "Tutar" },
                    { "Message.data.amount", 101, 4, "Amount", "Tutar" },
                    { "Message.data.amount", 102, 4, "Amount", "Tutar" }
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "ApiKey", "ClientIdJsonPath", "DisplayType", "EmailServiceReference", "KafkaUrl", "ParentId", "PushServiceReference", "Secret", "SmsServiceReference", "Title_EN", "Title_TR", "Topic" },
                values: new object[] { 10101, "a1b2c33d4e5f6g7h8i9jakblc", null, 2, "notify_email_incoming_fast", "test", 101, "notify_push_incoming_fast", "11561681-8ba5-4b46-bed0-905ae1769bc6", "notify_sms_incoming_fast", "Not Delivered FAST Messages", "Ulasmayan FAST", "http://localhost:8082/topics/cdc_eft/incoming_fast_not_delivered" });
        }
    }
}
