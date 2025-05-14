document.addEventListener('DOMContentLoaded', function() {
    // Xử lý tất cả các thông báo
    const notifications = document.querySelectorAll('.notification');
    
    notifications.forEach(notification => {
        // Tự động ẩn sau 5 giây
        setTimeout(() => {
            hideNotification(notification);
        }, 5000);

        // Xử lý nút đóng
        const closeButton = notification.querySelector('.notification-close');
        if (closeButton) {
            closeButton.addEventListener('click', () => {
                hideNotification(notification);
            });
        }
    });

    function hideNotification(notification) {
        notification.classList.add('hide');
        // Xóa phần tử sau khi animation kết thúc
        setTimeout(() => {
            notification.remove();
        }, 500);
    }
});
