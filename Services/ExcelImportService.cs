using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLySuaChua_BaoHanh.Models;
using System.Text.RegularExpressions;

namespace QuanLySuaChua_BaoHanh.Services
{
    public class ExcelImportService
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public ExcelImportService(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
            // Set the LicenseContext for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ImportResult> ImportThanhPhoAsync(Stream fileStream)
        {
            var result = new ImportResult();

            try
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // Validate the file structure
                    if (colCount < 1)
                    {
                        result.Errors.Add("File không đúng định dạng. Cần có ít nhất một cột cho tên thành phố.");
                        return result;
                    }

                    // Check header (optional)
                    var header = worksheet.Cells[1, 1].Value?.ToString();
                    int startRow = (header != null && header.ToLower().Contains("tên thành phố")) ? 2 : 1;

                    // Process rows
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        var tenThanhPho = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        
                        if (string.IsNullOrEmpty(tenThanhPho))
                            continue;

                        // Check if the ThanhPho already exists
                        var existingThanhPho = await _context.ThanhPhos
                            .FirstOrDefaultAsync(tp => tp.TenThanhPho.ToLower() == tenThanhPho.ToLower());

                        if (existingThanhPho == null)
                        {
                            var thanhPho = new ThanhPho
                            {
                                ThanhPhoId = await _idGenerator.GenerateThanhPhoIdAsync(),
                                TenThanhPho = tenThanhPho
                            };

                            _context.ThanhPhos.Add(thanhPho);
                            result.SuccessCount++;
                        }
                        else
                        {
                            result.SkippedCount++;
                            result.Warnings.Add($"Thành phố '{tenThanhPho}' đã tồn tại.");
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi khi nhập dữ liệu: {ex.Message}");
            }

            return result;
        }

        public async Task<ImportResult> ImportQuanAsync(Stream fileStream)
        {
            var result = new ImportResult();

            try
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // Validate the file structure
                    if (colCount < 2)
                    {
                        result.Errors.Add("File không đúng định dạng. Cần có ít nhất hai cột: Tên thành phố và Tên quận.");
                        return result;
                    }

                    // Check header (optional)
                    var header1 = worksheet.Cells[1, 1].Value?.ToString();
                    var header2 = worksheet.Cells[1, 2].Value?.ToString();
                    int startRow = (header1 != null && header2 != null &&
                                   (header1.ToLower().Contains("thành phố") || header1.ToLower().Contains("tỉnh")) &&
                                    header2.ToLower().Contains("quận")) ? 2 : 1;

                    // Process rows
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        var tenThanhPho = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var tenQuan = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        
                        if (string.IsNullOrEmpty(tenThanhPho) || string.IsNullOrEmpty(tenQuan))
                            continue;

                        // Find the ThanhPho
                        var thanhPho = await _context.ThanhPhos
                            .FirstOrDefaultAsync(tp => tp.TenThanhPho.ToLower() == tenThanhPho.ToLower());

                        if (thanhPho == null)
                        {
                            result.Warnings.Add($"Thành phố '{tenThanhPho}' không tồn tại. Tạo mới thành phố.");
                            thanhPho = new ThanhPho
                            {
                                ThanhPhoId = await _idGenerator.GenerateThanhPhoIdAsync(),
                                TenThanhPho = tenThanhPho
                            };
                            _context.ThanhPhos.Add(thanhPho);
                            await _context.SaveChangesAsync(); // Save to get the ID
                        }

                        // Check if the Quan already exists in this ThanhPho
                        var existingQuan = await _context.Quans
                            .FirstOrDefaultAsync(q => q.TenQuan.ToLower() == tenQuan.ToLower() && 
                                                     q.ThanhPhoId == thanhPho.ThanhPhoId);

                        if (existingQuan == null)
                        {
                            var quan = new Quan
                            {
                                QuanId = await _idGenerator.GenerateQuanIdAsync(),
                                TenQuan = tenQuan,
                                ThanhPhoId = thanhPho.ThanhPhoId
                            };

                            _context.Quans.Add(quan);
                            result.SuccessCount++;
                        }
                        else
                        {
                            result.SkippedCount++;
                            result.Warnings.Add($"Quận '{tenQuan}' đã tồn tại trong thành phố '{tenThanhPho}'.");
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi khi nhập dữ liệu: {ex.Message}");
            }

            return result;
        }

        public async Task<ImportResult> ImportPhuongAsync(Stream fileStream)
        {
            var result = new ImportResult();

            try
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // Validate the file structure
                    if (colCount < 3)
                    {
                        result.Errors.Add("File không đúng định dạng. Cần có ít nhất ba cột: Tên thành phố, Tên quận và Tên phường.");
                        return result;
                    }

                    // Check header (optional)
                    var header1 = worksheet.Cells[1, 1].Value?.ToString();
                    var header2 = worksheet.Cells[1, 2].Value?.ToString();
                    var header3 = worksheet.Cells[1, 3].Value?.ToString();
                    int startRow = (header1 != null && header2 != null && header3 != null &&
                                   (header1.ToLower().Contains("thành phố") || header1.ToLower().Contains("tỉnh")) &&
                                    header2.ToLower().Contains("quận") &&
                                    header3.ToLower().Contains("phường")) ? 2 : 1;

                    // Process rows
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        var tenThanhPho = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var tenQuan = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        var tenPhuong = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                        
                        if (string.IsNullOrEmpty(tenThanhPho) || string.IsNullOrEmpty(tenQuan) || string.IsNullOrEmpty(tenPhuong))
                            continue;

                        // Find the ThanhPho
                        var thanhPho = await _context.ThanhPhos
                            .FirstOrDefaultAsync(tp => tp.TenThanhPho.ToLower() == tenThanhPho.ToLower());

                        if (thanhPho == null)
                        {
                            result.Warnings.Add($"Thành phố '{tenThanhPho}' không tồn tại. Tạo mới thành phố.");
                            thanhPho = new ThanhPho
                            {
                                ThanhPhoId = await _idGenerator.GenerateThanhPhoIdAsync(),
                                TenThanhPho = tenThanhPho
                            };
                            _context.ThanhPhos.Add(thanhPho);
                            await _context.SaveChangesAsync(); // Save to get the ID
                        }

                        // Find the Quan
                        var quan = await _context.Quans
                            .FirstOrDefaultAsync(q => q.TenQuan.ToLower() == tenQuan.ToLower() && 
                                                     q.ThanhPhoId == thanhPho.ThanhPhoId);

                        if (quan == null)
                        {
                            result.Warnings.Add($"Quận '{tenQuan}' không tồn tại trong thành phố '{tenThanhPho}'. Tạo mới quận.");
                            quan = new Quan
                            {
                                QuanId = await _idGenerator.GenerateQuanIdAsync(),
                                TenQuan = tenQuan,
                                ThanhPhoId = thanhPho.ThanhPhoId
                            };
                            _context.Quans.Add(quan);
                            await _context.SaveChangesAsync(); // Save to get the ID
                        }

                        // Check if the Phuong already exists in this Quan
                        var existingPhuong = await _context.Phuongs
                            .FirstOrDefaultAsync(p => p.TenPhuong.ToLower() == tenPhuong.ToLower() && 
                                                     p.QuanId == quan.QuanId);

                        if (existingPhuong == null)
                        {
                            var phuong = new Phuong
                            {
                                PhuongId = await _idGenerator.GeneratePhuongIdAsync(),
                                TenPhuong = tenPhuong,
                                QuanId = quan.QuanId
                            };

                            _context.Phuongs.Add(phuong);
                            result.SuccessCount++;
                        }
                        else
                        {
                            result.SkippedCount++;
                            result.Warnings.Add($"Phường '{tenPhuong}' đã tồn tại trong quận '{tenQuan}'.");
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi khi nhập dữ liệu: {ex.Message}");
            }

            return result;
        }
    }

    public class ImportResult
    {
        public int SuccessCount { get; set; } = 0;
        public int SkippedCount { get; set; } = 0;
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();

        public bool HasErrors => Errors.Any();
    }
} 