using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Services
{
    public class IDGenerator
    {
        private readonly BHSC_DbContext _context;

        public IDGenerator(BHSC_DbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateNguoiDungIDAsync(string vaiTro)
        {
            string prefix;
            int maxDigits;

            switch (vaiTro)
            {
                case "QuanTriVien":
                    prefix = "QTV";
                    maxDigits = 3;
                    break;
                case "KyThuatVien":
                    prefix = "KTV";
                    maxDigits = 3;
                    break;
                case "TuVanVien":
                    prefix = "TVV";
                    maxDigits = 3;
                    break;
                case "NhanVienKho":
                    prefix = "NVK";
                    maxDigits = 3;
                    break;
                case "KhachHang":
                    prefix = "KH";
                    maxDigits = 3;  //3: KH001 | 4:KH0001
                    break;
                default:
                    prefix = "USER";
                    maxDigits = 3;
                    break;
            }

            int maxId = 0;
            var roleUsers = await _context.NguoiDungs
                .Where(nd => nd.VaiTro == vaiTro)
                .Select(nd => nd.Id)
                .ToListAsync();

            foreach (var id in roleUsers)
            {
                if (id.StartsWith(prefix) && int.TryParse(id.Substring(prefix.Length), out int idNum))
                {
                    maxId = Math.Max(maxId, idNum);
                }
            }

            maxId++;
            string format = new string('0', maxDigits);
            return $"{prefix}{maxId.ToString(format)}";
        }
        public async Task<string> GenerateThanhPhoIdAsync()
        {
            const string prefix = "TP";
            int maxId = await GetMaxIdForPrefixAsync<ThanhPho>(tp => tp.ThanhPhoId, prefix);
            return $"{prefix}{maxId+1:D3}";
        }


        public async Task<string> GenerateQuanIdAsync()
        {
            const string prefix = "QU";
            int maxId = await GetMaxIdForPrefixAsync<Quan>(q => q.QuanId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }


        public async Task<string> GeneratePhuongIdAsync()
        {
            const string prefix = "PH";
            int maxId = await GetMaxIdForPrefixAsync<Phuong>(p => p.PhuongId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }


        public async Task<string> GenerateDanhMucIdAsync()
        {
            const string prefix = "DM";
            int maxId = await GetMaxIdForPrefixAsync<DanhMuc>(d => d.DanhMucId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }


        public async Task<string> GenerateLinhKienIdAsync()
        {
            const string prefix = "LK";
            int maxId = await GetMaxIdForPrefixAsync<LinhKien>(lk => lk.LinhKienId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GenerateSanPhamIdAsync()
        {
            const string prefix = "SP";
            int maxId = await GetMaxIdForPrefixAsync<SanPham>(sp => sp.SanPhamId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GeneratePhieuSuaChuaIdAsync()
        {
            const string prefix = "PSC";
            int maxId = await GetMaxIdForPrefixAsync<PhieuSuaChua>(p => p.PhieuSuaChuaId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GeneratePhieuNhapIdAsync()
        {
            const string prefix = "PN";
            int maxId = await GetMaxIdForPrefixAsync<PhieuNhap>(p => p.PhieuNhapId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GeneratePhieuXuatIdAsync()
        {
            const string prefix = "PX";
            int maxId = await GetMaxIdForPrefixAsync<PhieuXuat>(p => p.PhieuXuatId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GenerateThongBaoIdAsync()
        {
            const string prefix = "TB";
            int maxId = await GetMaxIdForPrefixAsync<ThongBao>(tb => tb.ThongBaoId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GenerateTinNhanIdAsync()
        {
            const string prefix = "TN";
            int maxId = await GetMaxIdForPrefixAsync<TinNhan>(tn => tn.TinNhanId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }

        public async Task<string> GenerateChiTietSuaChuaIdAsync()
        {
            const string prefix = "CT";
            int maxId = await GetMaxIdForPrefixAsync<ChiTietSuaChua>(ct => ct.ChiTietId, prefix);
            return $"{prefix}{maxId + 1:D3}";
        }


        private async Task<int> GetMaxIdForPrefixAsync<TEntity>(Func<TEntity, string> idSelector, string prefix) where TEntity : class
        {
            var entities = await _context.Set<TEntity>().ToListAsync();
            int maxId = 0;

            foreach (var entity in entities)
            {
                string id = idSelector(entity);
                if (id != null && id.StartsWith(prefix) && int.TryParse(id.Substring(prefix.Length), out int idNum))
                {
                    maxId = Math.Max(maxId, idNum);
                }
            }

            return maxId;
        }

        public async Task<string> GenerateRoleIdAsync(string roleName){
            const string prefix = "ROLE_";
    
            // Kiểm tra xem role đã tồn tại chưa
            var existingRoles = await _context.Roles
                .Where(r => r.Name == roleName)
                .ToListAsync();
                
            if (existingRoles.Any())
            {
                // Nếu role đã tồn tại, trả về ID hiện tại
                return existingRoles.First().Id;
            }
            
            // Tạo ID mới cho role
            string roleId = $"{prefix}{roleName}";
            
            // Đảm bảo ID là duy nhất
            int counter = 1;
            while (await _context.Roles.AnyAsync(r => r.Id == roleId))
            {
                roleId = $"{prefix}{roleName}_{counter++}";
            }
            
            return roleId;
                }

            }
}
