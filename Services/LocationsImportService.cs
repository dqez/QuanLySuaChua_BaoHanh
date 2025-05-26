using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Services
{
    public class LocationsImportService
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public LocationsImportService(
            BHSC_DbContext context, 
            IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
            
            // Set the LicenseContext for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }        public async Task<ImportResult> ImportAllLocationsAsync(Stream fileStream, string fileName = "")
        {
            var result = new ImportResult();

            try
            {
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                
                if (fileExtension == ".csv")
                {
                    await ProcessCsvFileAsync(fileStream, result);
                }
                else if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    await ProcessExcelFileAsync(fileStream, result);
                }
                else
                {
                    result.Errors.Add("Định dạng file không được hỗ trợ. Chỉ chấp nhận file Excel (.xlsx, .xls) hoặc CSV (.csv)");
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi khi nhập dữ liệu: {ex.Message}");
            }

            return result;
        }

        private async Task ProcessCsvFileAsync(Stream fileStream, ImportResult result)
        {
            using (var reader = new StreamReader(fileStream))
            {
                string line;
                int row = 0;
                bool hasHeader = false;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    row++;
                    
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var columns = line.Split(',');
                    
                    if (columns.Length < 3)
                    {
                        result.Errors.Add($"Dòng {row}: Không đủ 3 cột (Thành phố, Quận, Phường)");
                        continue;
                    }

                    var tenThanhPho = columns[0]?.Trim();
                    var tenQuan = columns[1]?.Trim();
                    var tenPhuong = columns[2]?.Trim();

                    // Check if this is header row
                    if (row == 1 && !hasHeader && 
                        (tenThanhPho?.ToLower().Contains("thành phố") == true || 
                         tenThanhPho?.ToLower().Contains("tỉnh") == true ||
                         tenQuan?.ToLower().Contains("quận") == true ||
                         tenPhuong?.ToLower().Contains("phường") == true))
                    {
                        hasHeader = true;
                        continue;
                    }

                    if (string.IsNullOrEmpty(tenThanhPho) || string.IsNullOrEmpty(tenQuan) || string.IsNullOrEmpty(tenPhuong))
                        continue;

                    await ProcessLocationRowAsync(tenThanhPho, tenQuan, tenPhuong, row, result);
                }
            }
        }

        private async Task ProcessExcelFileAsync(Stream fileStream, ImportResult result)
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
                    return;
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

                    await ProcessLocationRowAsync(tenThanhPho, tenQuan, tenPhuong, row, result);
                }
            }
        }

        private async Task ProcessLocationRowAsync(string tenThanhPho, string tenQuan, string tenPhuong, int row, ImportResult result)
        {
            try
            {
                // Step 1: Check/Create ThanhPho
                var thanhPho = await FindThanhPhoByNameAsync(tenThanhPho);
                if (thanhPho == null)
                {
                    thanhPho = await CreateThanhPhoEntityAsync(tenThanhPho);
                    result.Warnings.Add($"Đã tạo mới thành phố: {tenThanhPho}");
                }

                // Step 2: Check/Create Quan
                var quan = await FindQuanByNameAsync(tenQuan, thanhPho.ThanhPhoId);
                if (quan == null)
                {
                    quan = await CreateQuanEntityAsync(tenQuan, thanhPho.ThanhPhoId);
                    result.Warnings.Add($"Đã tạo mới quận: {tenQuan} trong thành phố {tenThanhPho}");
                }

                // Step 3: Check/Create Phuong
                var phuong = await FindPhuongByNameAsync(tenPhuong, quan.QuanId);
                if (phuong == null)
                {
                    phuong = await CreatePhuongEntityAsync(tenPhuong, quan.QuanId);
                    result.SuccessCount++;
                }
                else
                {
                    result.SkippedCount++;
                    result.Warnings.Add($"Phường '{tenPhuong}' đã tồn tại trong quận '{tenQuan}', thành phố '{tenThanhPho}'.");
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi khi xử lý dòng {row}: {tenThanhPho}, {tenQuan}, {tenPhuong} - {ex.Message}");
            }
        }

        // Helper methods for ThanhPho operations
        private async Task<ThanhPho?> FindThanhPhoByNameAsync(string tenThanhPho)
        {
            return await _context.ThanhPhos
                .FirstOrDefaultAsync(tp => tp.TenThanhPho.ToLower() == tenThanhPho.ToLower());
        }

        private async Task<ThanhPho> CreateThanhPhoEntityAsync(string tenThanhPho)
        {
            var thanhPho = new ThanhPho
            {
                ThanhPhoId = await _idGenerator.GenerateThanhPhoIdAsync(),
                TenThanhPho = tenThanhPho
            };

            _context.ThanhPhos.Add(thanhPho);
            await _context.SaveChangesAsync();
            return thanhPho;
        }

        // Helper methods for Quan operations
        private async Task<Quan?> FindQuanByNameAsync(string tenQuan, string thanhPhoId)
        {
            return await _context.Quans
                .FirstOrDefaultAsync(q => q.TenQuan.ToLower() == tenQuan.ToLower() 
                                    && q.ThanhPhoId == thanhPhoId);
        }

        private async Task<Quan> CreateQuanEntityAsync(string tenQuan, string thanhPhoId)
        {
            var quan = new Quan
            {
                QuanId = await _idGenerator.GenerateQuanIdAsync(),
                TenQuan = tenQuan,
                ThanhPhoId = thanhPhoId
            };

            _context.Quans.Add(quan);
            await _context.SaveChangesAsync();
            return quan;
        }

        // Helper methods for Phuong operations
        private async Task<Phuong?> FindPhuongByNameAsync(string tenPhuong, string quanId)
        {
            return await _context.Phuongs
                .FirstOrDefaultAsync(p => p.TenPhuong.ToLower() == tenPhuong.ToLower() 
                                    && p.QuanId == quanId);
        }

        private async Task<Phuong> CreatePhuongEntityAsync(string tenPhuong, string quanId)
        {
            var phuong = new Phuong
            {
                PhuongId = await _idGenerator.GeneratePhuongIdAsync(),
                TenPhuong = tenPhuong,
                QuanId = quanId
            };

            _context.Phuongs.Add(phuong);
            await _context.SaveChangesAsync();
            return phuong;
        }
    }
}
