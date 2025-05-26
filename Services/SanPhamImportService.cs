using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLySuaChua_BaoHanh.Models;
using System.Globalization;

namespace QuanLySuaChua_BaoHanh.Services
{
    public class SanPhamImportService
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public SanPhamImportService(
            BHSC_DbContext context, 
            IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
            
            // Set the LicenseContext for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ImportResult> ImportSanPhamAsync(Stream fileStream, string fileName = "")
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
                    
                    if (columns.Length < 9)
                    {
                        result.Errors.Add($"Dòng {row}: Không đủ 9 cột (Mã sản phẩm, Khách hàng, Danh mục, Mã bảo hành, Tên sản phẩm, Ngày mua, Thời gian bảo hành, Ngày hết hạn, Mô tả)");
                        continue;
                    }

                    // Check if this is header row
                    if (row == 1 && !hasHeader && 
                        (columns[0]?.ToLower().Contains("mã sản phẩm") == true || 
                         columns[1]?.ToLower().Contains("khách hàng") == true ||
                         columns[2]?.ToLower().Contains("danh mục") == true ||
                         columns[4]?.ToLower().Contains("tên sản phẩm") == true))
                    {
                        hasHeader = true;
                        continue;
                    }

                    await ProcessSanPhamRowAsync(columns, row, result);
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
                if (colCount < 9)
                {
                    result.Errors.Add("File không đúng định dạng. Cần có ít nhất 9 cột: Mã sản phẩm, Khách hàng, Danh mục, Mã bảo hành, Tên sản phẩm, Ngày mua, Thời gian bảo hành, Ngày hết hạn, Mô tả.");
                    return;
                }

                // Check header (optional)
                var header1 = worksheet.Cells[1, 1].Value?.ToString();
                var header2 = worksheet.Cells[1, 2].Value?.ToString();
                var header5 = worksheet.Cells[1, 5].Value?.ToString();
                int startRow = (header1 != null && header2 != null && header5 != null &&
                               (header1.ToLower().Contains("mã sản phẩm") || 
                                header2.ToLower().Contains("khách hàng") ||
                                header5.ToLower().Contains("tên sản phẩm"))) ? 2 : 1;

                // Process rows
                for (int row = startRow; row <= rowCount; row++)
                {
                    var columns = new string[9];
                    for (int col = 1; col <= 9; col++)
                    {
                        columns[col - 1] = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                    }

                    await ProcessSanPhamRowAsync(columns, row, result);
                }
            }
        }

        private async Task ProcessSanPhamRowAsync(string[] columns, int row, ImportResult result)
        {
            try
            {
                var maSanPham = columns[0]?.Trim();
                var khachHang = columns[1]?.Trim();
                var danhMuc = columns[2]?.Trim();
                var maBaoHanh = columns[3]?.Trim();
                var tenSanPham = columns[4]?.Trim();
                var ngayMuaStr = columns[5]?.Trim();
                var thoiGianBaoHanhStr = columns[6]?.Trim();
                var ngayHetHanStr = columns[7]?.Trim();
                var moTa = columns[8]?.Trim();

                // Validate required fields
                if (string.IsNullOrEmpty(tenSanPham))
                {
                    result.Errors.Add($"Dòng {row}: Tên sản phẩm không được để trống");
                    return;
                }

                if (string.IsNullOrEmpty(khachHang))
                {
                    result.Errors.Add($"Dòng {row}: Khách hàng không được để trống");
                    return;
                }

                if (string.IsNullOrEmpty(danhMuc))
                {
                    result.Errors.Add($"Dòng {row}: Danh mục không được để trống");
                    return;
                }

                // Find KhachHang
                var khachHangEntity = await FindKhachHangAsync(khachHang);
                if (khachHangEntity == null)
                {
                    result.Errors.Add($"Dòng {row}: Không tìm thấy khách hàng '{khachHang}'");
                    return;
                }

                // Find DanhMuc
                var danhMucEntity = await FindDanhMucAsync(danhMuc);
                if (danhMucEntity == null)
                {
                    result.Errors.Add($"Dòng {row}: Không tìm thấy danh mục '{danhMuc}'");
                    return;
                }                // Parse dates
                DateOnly ngayMua = DateOnly.FromDateTime(DateTime.Now); // Default to today
                DateOnly? ngayHetHan = null;
                int? thoiGianBaoHanh = null;

                if (!string.IsNullOrEmpty(ngayMuaStr))
                {
                    if (DateOnly.TryParse(ngayMuaStr, out var parsedNgayMua))
                    {
                        ngayMua = parsedNgayMua;
                    }
                    else
                    {
                        // Try parsing as DateTime first, then convert to DateOnly
                        if (DateTime.TryParse(ngayMuaStr, out var dtNgayMua))
                        {
                            ngayMua = DateOnly.FromDateTime(dtNgayMua);
                        }
                        else
                        {
                            result.Warnings.Add($"Dòng {row}: Không thể parse ngày mua '{ngayMuaStr}', sử dụng ngày hiện tại");
                            ngayMua = DateOnly.FromDateTime(DateTime.Now);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(thoiGianBaoHanhStr))
                {
                    if (int.TryParse(thoiGianBaoHanhStr, out var parsedThoiGian))
                    {
                        thoiGianBaoHanh = parsedThoiGian;
                    }
                    else
                    {
                        result.Warnings.Add($"Dòng {row}: Không thể parse thời gian bảo hành '{thoiGianBaoHanhStr}'");
                    }
                }

                if (!string.IsNullOrEmpty(ngayHetHanStr))
                {
                    if (DateOnly.TryParse(ngayHetHanStr, out var parsedNgayHetHan))
                    {
                        ngayHetHan = parsedNgayHetHan;
                    }
                    else
                    {
                        // Try parsing as DateTime first, then convert to DateOnly
                        if (DateTime.TryParse(ngayHetHanStr, out var dtNgayHetHan))
                        {
                            ngayHetHan = DateOnly.FromDateTime(dtNgayHetHan);
                        }
                        else
                        {
                            result.Warnings.Add($"Dòng {row}: Không thể parse ngày hết hạn '{ngayHetHanStr}'");
                        }
                    }
                }

                // Auto-calculate NgayHetHan if not provided but we have NgayMua and ThoiGianBaoHanh
                if (ngayHetHan == null && thoiGianBaoHanh.HasValue)
                {
                    ngayHetHan = ngayMua.AddMonths(thoiGianBaoHanh.Value);
                }

                // If we still don't have NgayHetHan, set a default (1 year from NgayMua)
                if (ngayHetHan == null)
                {
                    ngayHetHan = ngayMua.AddMonths(12);
                    result.Warnings.Add($"Dòng {row}: Không có ngày hết hạn, sử dụng mặc định 12 tháng từ ngày mua");
                }

                // Generate SanPhamId if not provided
                if (string.IsNullOrEmpty(maSanPham))
                {
                    maSanPham = await _idGenerator.GenerateSanPhamIdAsync();
                }

                // Check if SanPham already exists
                var existingSanPham = await _context.SanPhams
                    .FirstOrDefaultAsync(sp => sp.SanPhamId == maSanPham);

                if (existingSanPham != null)
                {
                    result.SkippedCount++;
                    result.Warnings.Add($"Dòng {row}: Sản phẩm với mã '{maSanPham}' đã tồn tại");
                    return;
                }                // Create new SanPham
                var sanPham = new SanPham
                {
                    SanPhamId = maSanPham,
                    KhachHangId = khachHangEntity.Id,
                    DanhMucId = danhMucEntity.DanhMucId,
                    MaBh = maBaoHanh,
                    TenSanPham = tenSanPham,
                    NgayMua = ngayMua,
                    ThoiGianBaoHanh = thoiGianBaoHanh ?? 12, // Default 12 months if not provided
                    NgayHetHanBh = ngayHetHan.Value,
                    MoTa = moTa
                };

                _context.SanPhams.Add(sanPham);
                await _context.SaveChangesAsync();

                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Lỗi khi xử lý dòng {row}: {ex.Message}");
            }
        }

        // Helper methods to find entities
        private async Task<NguoiDung?> FindKhachHangAsync(string identifier)
        {
            // Try to find by ID first, then by name
            var khachHang = await _context.NguoiDungs
                .Where(nd => nd.VaiTro == "KhachHang")
                .FirstOrDefaultAsync(nd => nd.Id == identifier);

            if (khachHang == null)
            {
                // Try to find by name
                khachHang = await _context.NguoiDungs
                    .Where(nd => nd.VaiTro == "KhachHang")
                    .FirstOrDefaultAsync(nd => nd.HoTen.ToLower().Contains(identifier.ToLower()));
            }

            return khachHang;
        }

        private async Task<DanhMuc?> FindDanhMucAsync(string identifier)
        {
            // Try to find by ID first, then by name
            var danhMuc = await _context.DanhMucs
                .FirstOrDefaultAsync(dm => dm.DanhMucId == identifier);

            if (danhMuc == null)
            {
                // Try to find by name
                danhMuc = await _context.DanhMucs
                    .FirstOrDefaultAsync(dm => dm.TenDanhMuc.ToLower().Contains(identifier.ToLower()));
            }

            return danhMuc;
        }
    }
}
