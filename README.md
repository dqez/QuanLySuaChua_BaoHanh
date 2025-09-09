# HỆ THỐNG QUẢN LÝ SỬA CHỮA VÀ BẢO HÀNH THIẾT BỊ ĐIỆN LẠNH HÃNG PANASONIC.

## Giới thiệu:
Xây dựng hệ thống quản lý sửa chữa và bảo hành thiết bị điện lạnh Panasonic, giúp tự động hóa quy trình, tối ưu hiệu suất làm việc và nâng cao trải nghiệm khách hàng.

## Công nghệ sử dụng:
  - **Framework:** ASP.NET Core 8.0 (.NET 8)
  - **Architecture:** Razor Pages with Areas
  - **Database:** SQL Server with Entity Framework Core
  - **Authentication: ASP.NET Core Identity
  - **Frontend:** Bootstrap 5, jQuery, Bootstrap Icons
  - **Payment Integration:** VnPay Payment Gateway
  - **Excel Processing:** NPOI/EPPlus libraries
  - **PDF Generation:** DinkToPdf (wkhtmltopdf wrapper)

## Tính năng chính:
   -	Quản trị viên: Cho phép cập nhật thông tin người dùng, phân quyền tài khoản, cập nhật danh mục sản phẩm và sản phẩm, phân công nhiệm vụ cho kỹ thuật viên. Quản trị viên có thể xem báo cáo, thống kê về doanh thu, số lượng khách hàng.
   -	Nhân viên kho linh kiện: Theo dõi số lượng tồn kho và cập nhật dữ liệu kho linh kiện, tạo phiếu xuất linh kiện theo ngày (vẫn phân biệt theo từng đơn sửa chữa), phiếu nhập linh kiện
   -	Nhân viên chăm sóc khách hàng: Tiếp nhận và xử lý yêu cầu sửa chữa/bảo hành từ khách hàng. Tìm kiếm và tra cứu thông tin khách hàng, lịch sử sửa chữa. Chat trực tiếp với khách hàng để tư vấn và giải đáp thắc mắc. Theo dõi và cập nhật tình trạng đơn sửa chữa/bảo hành.
   -	Nhân viên kỹ thuật: Theo dõi các đơn sửa chữa/bảo hành được phân công, cập nhật danh sách đơn sửa chữa/bảo hành, cập nhật lỗi và linh kiện cho đơn sửa chữa/bảo hành, cập nhật trạng thái sửa chữa (đang sửa chữa, chờ linh kiện, hoàn thành).
   -	Khách hàng: Đăng ký yêu cầu sửa chữa/bảo hành. Theo dõi tiến độ sửa chữa và nhận thông báo cập nhật. Kiểm tra thời hạn bảo hành của thiết bị. Xem lịch sử sửa chữa/bảo hành của thiết bị. Thanh toán dịch vụ. Đánh giá chất lượng dịch vụ và nhân viên sửa chữa. Tra cứu thông tin dịch vụ, linh kiện, sản phẩm.

##  Workflow

### Luồng trạng thái
  ```cshap
    public enum TrangThaiPhieu
    {
        ChoXacNhan,
        DaXacNhan,
        DaPhanCong,
        ChoKiemTra,
        DangSuaChua,
        DaSuaXong,
        DaThanhToan,
        DangVanChuyen,
        HoanThanh,
        DaHuy
    }
  ```
  
### Quy trình nghiệp vụ:
  1. Khách hàng gửi yêu cầu sửa chữa/bảo hành
  2. Nhân viên tư vấn kiểm tra và xác nhận đơn sửa chữa/bảo hành
  3. Quản trị viên phân công cho kỹ thuật viên
  4. Kỹ thuật viên kiểm tra và đưa ra báo giá
  5. Khách hàng duyệt hoặc từ chối báo giá
  6. Kỹ thuật viên tiến hành cập nhật linh kiện cần sử dụng
  7. Nhân viên kho tạo phiếu xuất linh kiện và xuất linh kiện cho kỹ thuật viên
  8. Kỹ thuật viên tiến hành sửa chữa và cập nhật trạng thái sửa chữa
  9. Khách hàng thanh toán qua VnPay
  10. Quản trị viên gửi sản phẩm đã sửa chữa
  11. Khách hàng xác nhận đã nhận hàng


## Cài đặt & Chạy project
### Yêu cầu
  - Visual Studio 2022 (hoặc Rider)
  - .NET 8 SDK
  - SQL Server (2019 trở lên)

### Các bước
1. Clone repo:
  ```
  git clone https://github.com/dqez/QuanLySuaChua_BaoHanh.git
  cd QuanLySuaChua_BaoHanh
  ```

2. Mở solution bằng Visual Studio 2022.

3. Cập nhật appsettings.json:
```
"ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=QuanLySuaChuaBaoHanh;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

4. Chạy migration để tạo database:
```
dotnet ef database update
```

5. Build & run:
```
dotnet run
```

6. Truy cập tại https://localhost:5001 hoặc http://localhost:5000.

### Tài khoản mặc định
```
Username: admin1
Password: Admin@123
```
