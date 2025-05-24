using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace QuanLySuaChua_BaoHanh.Services
{
    public interface IExportService
    {
        byte[] ExportToExcel<T>(List<T> data, string sheetName, Dictionary<string, string> columnMappings, string title = null);
        byte[] ExportToPdf(string htmlContent, string title = null);
    }

    public class ExportService : IExportService
    {
        private readonly IConverter _pdfConverter;

        public ExportService(IConverter pdfConverter)
        {
            _pdfConverter = pdfConverter;
        }

        public byte[] ExportToExcel<T>(List<T> data, string sheetName, Dictionary<string, string> columnMappings, string title = null)
        {
            // Configure EPPlus to use non-commercial license (if applicable, depends on your licensing)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Add title if provided
                int startRow = 1;
                if (!string.IsNullOrEmpty(title))
                {
                    worksheet.Cells[startRow, 1].Value = title;
                    worksheet.Cells[startRow, 1, startRow, columnMappings.Count].Merge = true;
                    worksheet.Cells[startRow, 1].Style.Font.Bold = true;
                    worksheet.Cells[startRow, 1].Style.Font.Size = 14;
                    worksheet.Cells[startRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    startRow += 2; // Leave a blank row after title
                }

                // Add headers
                int col = 1;
                foreach (var header in columnMappings.Values)
                {
                    worksheet.Cells[startRow, col].Value = header;
                    worksheet.Cells[startRow, col].Style.Font.Bold = true;
                    worksheet.Cells[startRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow, col].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[startRow, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[startRow, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[startRow, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[startRow, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    col++;
                }

                // Add data
                int row = startRow + 1;
                foreach (var item in data)
                {
                    col = 1;
                    foreach (var prop in columnMappings.Keys)
                    {
                        var propInfo = typeof(T).GetProperty(prop);
                        if (propInfo != null)
                        {
                            var value = propInfo.GetValue(item);
                            worksheet.Cells[row, col].Value = value;
                            
                            // Format for currency values (detect decimal properties)
                            if (propInfo.PropertyType == typeof(decimal) || propInfo.PropertyType == typeof(decimal?))
                            {
                                worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
                            }
                            // Format for date values
                            else if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                            {
                                worksheet.Cells[row, col].Style.Numberformat.Format = "dd/MM/yyyy";
                            }

                            // Add border
                            worksheet.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        col++;
                    }
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        public byte[] ExportToPdf(string htmlContent, string title = null)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 },
                DocumentTitle = title ?? "Export Document"
            };

            var headerStyle = @"
                <style>
                    h1 { 
                        font-size: 20px; 
                        font-weight: bold; 
                        text-align: center; 
                        margin-bottom: 20px; 
                        font-family: 'Arial', sans-serif;
                    }
                    body { 
                        font-family: 'Arial', sans-serif; 
                        font-size: 12px; 
                    }
                    table { 
                        border-collapse: collapse; 
                        width: 100%; 
                        margin-bottom: 20px;
                    }
                    th { 
                        background-color: #f2f2f2; 
                        text-align: left; 
                        padding: 8px; 
                        border: 1px solid #ddd; 
                        font-weight: bold;
                    }
                    td { 
                        border: 1px solid #ddd; 
                        padding: 8px; 
                    }
                    tr:nth-child(even) { 
                        background-color: #f9f9f9; 
                    }
                    .footer { 
                        text-align: right; 
                        font-size: 10px; 
                        margin-top: 30px; 
                    }
                </style>";

            var titleHtml = !string.IsNullOrEmpty(title) ? $"<h1>{title}</h1>" : "";
            var footer = $"<div class='footer'>Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}</div>";

            var fullHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    {headerStyle}
                </head>
                <body>
                    {titleHtml}
                    {htmlContent}
                    {footer}
                </body>
                </html>";

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = fullHtml,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8"
                },
                HeaderSettings = { FontSize = 12, Right = "Trang [page] / [toPage]", Line = true },
                FooterSettings = { FontSize = 10, Line = true, Center = "Trung tâm sửa chữa" }
            };

            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _pdfConverter.Convert(document);
        }
    }
}