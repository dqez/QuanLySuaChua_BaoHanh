$(document).ready(function() {
    // Handle order cancellation
    $('.cancel-order').click(function() {
        const orderId = $(this).data('id');
        if (confirm('Bạn có chắc chắn muốn hủy đơn sửa chữa này?')) {
            $.ajax({
                url: '/KhachHang/DonSuaChua/Cancel',
                type: 'POST',
                data: { id: orderId },
                success: function(result) {
                    if (result.success) {
                        toastr.success('Đã hủy đơn sửa chữa thành công');
                        setTimeout(() => {
                            location.reload();
                        }, 1500);
                    } else {
                        toastr.error(result.message || 'Có lỗi xảy ra khi hủy đơn');
                    }
                },
                error: function() {
                    toastr.error('Có lỗi xảy ra khi hủy đơn');
                }
            });
        }
    });

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();
    
    // Initialize toastr options
    toastr.options = {
        "closeButton": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "timeOut": "3000"
    };
});
