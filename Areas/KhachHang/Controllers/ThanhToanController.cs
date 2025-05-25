#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;
using QuanLySuaChua_BaoHanh.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize(Roles = "KhachHang")]
    public class ThanhToanController : Controller
    {
        private readonly VnPayConfig _config;
        private readonly BHSC_DbContext _dbContext;
        private readonly UserManager<NguoiDung> _userManager;

        public ThanhToanController(IOptions<VnPayConfig> config, BHSC_DbContext dbContext, UserManager<NguoiDung> userManager)
        {
            _config = config.Value;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpGet]
        [Route("khach-hang/chon-phuong-thuc-thanh-toan/{phieuSuaChuaId}")]
        public IActionResult PhuongThucThanhToan(string phieuSuaChuaId)
        {
            if (string.IsNullOrEmpty(phieuSuaChuaId))
            {
                TempData["Error"] = "Phiếu sửa chữa không hợp lệ.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }
            ViewBag.PhieuSuaChuaId = phieuSuaChuaId;
            return View();
        }
        [HttpPost]
        [Route("khach-hang/chon-phuong-thuc-thanh-toan")]
        public async Task<IActionResult> PhuongThucThanhToan(string phuongThuc, string phieuSuaChuaId)
        {
            if (string.IsNullOrEmpty(phuongThuc) || string.IsNullOrEmpty(phieuSuaChuaId))
            {
                TempData["Error"] = "Vui lòng chọn phương thức thanh toán.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }
            var user = await _userManager.GetUserAsync(User);
            PhieuSuaChua phieuSuaChua = _dbContext.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .FirstOrDefault(p => p.PhieuSuaChuaId == phieuSuaChuaId && p.KhachHangId == user.Id);
            if (phieuSuaChua == null)
            {
                TempData["Error"] = "Phiếu sửa chữa không tồn tại hoặc bạn không có quyền truy cập.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }
            if (phuongThuc == "TienMat")
            {
                phieuSuaChua.PhuongThucThanhToan = "TienMat";
                await _dbContext.SaveChangesAsync();
                TempData["Success"] = "Bạn đã chọn thanh toán bằng tiền mặt.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }
            else if (phuongThuc == "ChuyenKhoan")
            {
                phieuSuaChua.PhuongThucThanhToan = "ChuyenKhoan";
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("ThanhToanVNPay", new { phieuId = phieuSuaChuaId });
            }
            else
            {
                TempData["Success"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }
        }
        [HttpGet]
        [Route("khach-hang/thanh-toan/{phieuId}")]
        public async Task<IActionResult> ThanhToanVNPay(string phieuId)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Phiếu sửa chữa không tồn tại.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }
            var user = await _userManager.GetUserAsync(User);

            PhieuSuaChua phieuSuaChua = _dbContext.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .FirstOrDefault(p => p.PhieuSuaChuaId == phieuId && p.KhachHangId == user.Id);

            if (phieuSuaChua == null)
            {
                TempData["Error"] = "Phiếu sửa chữa không tồn tại hoặc bạn không có quyền truy cập.";
                return RedirectToAction("Index", "DonSuaChua", new { area = "KhachHang" });
            }

            string vnp_Returnurl = _config.ReturnUrl;
            string vnp_Url = _config.Url;
            string vnp_TmnCode = _config.TmnCode;
            string vnp_HashSecret = _config.HashSecret;

            // Log hashSecret để debug
            System.Diagnostics.Debug.WriteLine($"HashSecret for request: {vnp_HashSecret}");

            // Tạo đối tượng xử lý thư viện VNPay
            VnPayLibrary vnpay = new VnPayLibrary();

            // Tạo mã giao dịch tham chiếu duy nhất
            string txnRef = $"{phieuSuaChua.PhieuSuaChuaId}_{DateTime.Now.Ticks}";

            // Thêm các tham số yêu cầu của VNPay
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION); // Phiên bản API VNPay
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            long amountInVnd = (long)(phieuSuaChua.TongTien * 100); // Nhân với 100 theo yêu cầu của VNPay
            vnpay.AddRequestData("vnp_Amount", amountInVnd.ToString());

            // Các thông tin khác
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");

            // Thông tin đơn hàng - không có dấu, không có ký tự đặc biệt
            string orderInfo = $"Thanh toan phieu sua chua {phieuSuaChua.PhieuSuaChuaId}";
            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");

            // URL để VNPay callback sau khi thanh toán xong
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

            // Mã giao dịch tham chiếu
            vnpay.AddRequestData("vnp_TxnRef", txnRef);

            // Tạo URL thanh toán
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            // Lưu URL thanh toán vào TempData để debug
            TempData["LastPaymentUrl"] = paymentUrl;

            return Redirect(paymentUrl);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("/KhachHang/ThanhToan/PaymentConfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            var responseModel = PaymentExecute(Request.Query);

            // Trích xuất ID phiếu từ OrderId (định dạng: {phieuId}_{timestamp})
            string phieuId = string.Empty;
            if (!string.IsNullOrEmpty(responseModel.OrderId) && responseModel.OrderId.Contains("_"))
            {
                phieuId = responseModel.OrderId.Split('_')[0];
                ViewBag.PhieuId = phieuId;
            }
            // Transfer all VNPay parameters to ViewBag for display
            foreach (var param in Request.Query)
            {
                if (param.Key.StartsWith("vnp_"))
                {
                    ViewData[param.Key] = param.Value.ToString();
                }
            }

            // Parse the amount (VNPay returns amount in smaller unit, needs to be divided by 100)
            if (Request.Query.ContainsKey("vnp_Amount") && long.TryParse(Request.Query["vnp_Amount"], out long amountValue))
            {
                ViewBag.Amount = amountValue / 100;
            }

            // Format payment date if available
            if (Request.Query.ContainsKey("vnp_PayDate") && !string.IsNullOrEmpty(Request.Query["vnp_PayDate"]))
            {
                string payDateStr = Request.Query["vnp_PayDate"];
                if (DateTime.TryParseExact(payDateStr, "yyyyMMddHHmmss",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime payDate))
                {
                    ViewBag.FormattedPayDate = payDate.ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                {
                    ViewBag.FormattedPayDate = payDateStr;
                }
            }

            if (responseModel.Success && responseModel.VnPayResponseCode == "00")
            {
                ViewBag.Message = "Thanh toán thành công";
                ViewBag.TransactionId = responseModel.TransactionId;

                if (!string.IsNullOrEmpty(phieuId))
                {
                    var phieuSuaChua = await _dbContext.PhieuSuaChuas
                        .FirstOrDefaultAsync(p => p.PhieuSuaChuaId == phieuId);

                    if (phieuSuaChua != null)
                    {
                        // Cập nhật trạng thái thanh toán
                        phieuSuaChua.TrangThai = Enums.TrangThaiPhieu.DaThanhToan.ToString();
                        await _dbContext.SaveChangesAsync();

                    }
                }
            }
            else if (!responseModel.Success)
            {
                ViewBag.Message = "Sai chữ ký (checksum)!";
            }
            else
            {
                ViewBag.Message = "Thanh toán không thành công";
                ViewBag.ErrorCode = responseModel.VnPayResponseCode;

                // Additional error details based on VnPay error codes
                string errorDetail = responseModel.VnPayResponseCode switch
                {
                    "01" => "Giao dịch đã tồn tại",
                    "02" => "Merchant không hợp lệ",
                    "03" => "Dữ liệu gửi sang không đúng định dạng",
                    "04" => "Khởi tạo GD không thành công do Website đang bị tạm khóa",
                    "05" => "Giao dịch không thành công do: Quý khách nhập sai mật khẩu thanh toán quá số lần quy định",
                    "06" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch",
                    "07" => "Giao dịch bị nghi ngờ là giao dịch gian lận",
                    "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa",
                    "10" => "Giao dịch không thành công do: Quý khách nhập sai mật khẩu xác thực giao dịch",
                    "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán",
                    "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                    _ => "Lỗi không xác định"
                };
                ViewBag.ErrorDetails = errorDetail;
            }

            return View();
        }
        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            string txnRef = vnpay.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            string vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config.HashSecret);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = txnRef,
                TransactionId = vnpay.GetResponseData("vnp_TransactionNo"),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }

    }
}
