using ClosedXML.Excel;
using System.IO;
using System.Collections.Generic;
using System;
using Contract.DTOs;

public static class ExcelHelper
{
    public static byte[] ExportPaymentLogs(List<PaymentLogDto> logs)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("PaymentLogs");
        ws.Cell(1, 1).Value = "#";
        ws.Cell(1, 2).Value = "User Email";
        ws.Cell(1, 3).Value = "Package";
        ws.Cell(1, 4).Value = "Amount";
        ws.Cell(1, 5).Value = "Status";
        ws.Cell(1, 6).Value = "Date";

        int row = 2, stt = 1;
        foreach (var log in logs)
        {
            ws.Cell(row, 1).Value = stt++;
            ws.Cell(row, 2).Value = log.UserEmail;
            ws.Cell(row, 3).Value = log.PackageName;
            ws.Cell(row, 4).Value = log.PaymentAmount;
            ws.Cell(row, 5).Value = log.PaymentStatus;
            ws.Cell(row, 6).Value = log.PaymentDate?.ToString("dd/MM/yyyy HH:mm");
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static byte[] ExportUsers(List<UserSimpleDto> users)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Users");
        ws.Cell(1, 1).Value = "#";
        ws.Cell(1, 2).Value = "Email";
        ws.Cell(1, 3).Value = "Name";
        ws.Cell(1, 4).Value = "Role";
        ws.Cell(1, 5).Value = "IsActive";
        ws.Cell(1, 6).Value = "CreatedAt";
        ws.Cell(1, 7).Value = "VIP Expiration";
        ws.Cell(1, 8).Value = "VIP Status";

        int row = 2, stt = 1;
        foreach (var u in users)
        {
            ws.Cell(row, 1).Value = stt++;
            ws.Cell(row, 2).Value = u.Email;
            ws.Cell(row, 3).Value = u.Name;
            ws.Cell(row, 4).Value = u.Role;
            ws.Cell(row, 5).Value = u.IsActive == true ? "Active" : "Inactive";
            ws.Cell(row, 6).Value = u.CreatedAt?.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(row, 7).Value = u.VipExpirationDate?.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(row, 8).Value = u.VipExpirationDate.HasValue && u.VipExpirationDate > DateTime.UtcNow ? "VIP" : "Normal";
            row++;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static byte[] ExportRevenue(List<PaymentLogDto> logs, string title)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Revenue");
        ws.Cell(1, 1).Value = title;
        ws.Range(1, 1, 1, 5).Merge().Style.Font.Bold = true;

        ws.Cell(2, 1).Value = "#";
        ws.Cell(2, 2).Value = "User Email";
        ws.Cell(2, 3).Value = "Amount";
        ws.Cell(2, 4).Value = "Status";
        ws.Cell(2, 5).Value = "Date";

        int row = 3, stt = 1;
        decimal total = 0;
        foreach (var log in logs)
        {
            ws.Cell(row, 1).Value = stt++;
            ws.Cell(row, 2).Value = log.UserEmail;
            ws.Cell(row, 3).Value = log.PaymentAmount;
            ws.Cell(row, 4).Value = log.PaymentStatus;
            ws.Cell(row, 5).Value = log.PaymentDate?.ToString("dd/MM/yyyy HH:mm");
            if (log.PaymentStatus == "Completed") total += log.PaymentAmount ?? 0;
            row++;
        }
        ws.Cell(row + 1, 2).Value = "Tổng doanh thu (Completed):";
        ws.Cell(row + 1, 3).Value = total;
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
