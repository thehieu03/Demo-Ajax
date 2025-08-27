document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('registerForm');
    const accountNameInput = document.getElementById('accountName');
    const accountEmailInput = document.getElementById('accountEmail');
    const accountPasswordInput = document.getElementById('accountPassword');
    
    // Biến để tránh gọi API quá nhiều lần
    let emailCheckTimeout = null;
    let lastCheckedEmail = '';
    
    // Lấy base URL từ window object hoặc sử dụng default
    const baseUrl = window.apiBaseUrl || 'http://localhost:5173/api';
    console.log('API Base URL:', baseUrl);

    // Validation functions
    function validateAccountName() {
        const value = accountNameInput.value.trim();
        if (value.length < 2) {
            showError(accountNameInput, 'Tên phải có ít nhất 2 ký tự');
            return false;
        }
        if (value.length > 50) {
            showError(accountNameInput, 'Tên không được quá 50 ký tự');
            return false;
        }
        clearError(accountNameInput);
        return true;
    }

    function validateEmail() {
        const value = accountEmailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        
        if (!value) {
            showError(accountEmailInput, 'Email không được để trống');
            return false;
        }
        if (!emailRegex.test(value)) {
            showError(accountEmailInput, 'Email không hợp lệ');
            return false;
        }
        clearError(accountEmailInput);
        return true;
    }

    // Hàm kiểm tra email trùng lặp bằng fetch API
    async function checkEmailExists(email) {
        try {
            // Hiển thị trạng thái đang kiểm tra
            const checkingSpan = document.createElement('span');
            checkingSpan.className = 'text-info small';
            checkingSpan.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i>Đang kiểm tra email...';
            accountEmailInput.parentElement.parentElement.appendChild(checkingSpan);
            
            // Sử dụng URL đúng từ configuration
            const apiUrl = `${baseUrl}/account/getAccountByEmail?email=${encodeURIComponent(email)}`;
            console.log('Gọi API kiểm tra email:', apiUrl);
            
            const response = await fetch(apiUrl);
            
            console.log('Response status:', response.status);
            console.log('Response ok:', response.ok);
            
            // Xóa trạng thái đang kiểm tra
            checkingSpan.remove();
            
            if (response.ok) {
                // Email đã tồn tại
                console.log('Email đã tồn tại');
                return true;
            } else if (response.status === 404) {
                // Email chưa tồn tại
                console.log('Email chưa tồn tại');
                return false;
            } else {
                // Có lỗi khác
                console.error('Lỗi khi kiểm tra email:', response.statusText);
                return false;
            }
        } catch (error) {
            console.error('Lỗi khi gọi API kiểm tra email:', error);
            return false;
        }
    }

    // Hàm validate email với kiểm tra trùng lặp
    async function validateEmailWithDuplicateCheck() {
        const value = accountEmailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        
        if (!value) {
            showError(accountEmailInput, 'Email không được để trống');
            return false;
        }
        if (!emailRegex.test(value)) {
            showError(accountEmailInput, 'Email không hợp lệ');
            return false;
        }

        // Nếu email đã được kiểm tra trước đó và giống nhau, không cần kiểm tra lại
        if (lastCheckedEmail === value) {
            return true;
        }

        // Kiểm tra email trùng lặp
        const emailExists = await checkEmailExists(value);
        if (emailExists) {
            showError(accountEmailInput, 'Email đã được sử dụng. Vui lòng chọn email khác.');
            return false;
        }

        // Lưu email đã kiểm tra
        lastCheckedEmail = value;
        clearError(accountEmailInput);
        return true;
    }

    function validatePassword() {
        const value = accountPasswordInput.value;
        if (value.length < 2) {
            showError(accountPasswordInput, 'Mật khẩu phải có ít nhất 2 ký tự');
            return false;
        }
        if (value.length > 100) {
            showError(accountPasswordInput, 'Mật khẩu không được quá 100 ký tự');
            return false;
        }
        clearError(accountPasswordInput);
        return true;
    }

    function showError(input, message) {
        const errorSpan = input.parentElement.nextElementSibling;
        if (errorSpan && errorSpan.classList.contains('text-danger')) {
            errorSpan.textContent = message;
        } else {
            const span = document.createElement('span');
            span.className = 'text-danger small';
            span.textContent = message;
            input.parentElement.parentElement.appendChild(span);
        }
        input.classList.add('is-invalid');
    }

    function clearError(input) {
        const errorSpan = input.parentElement.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.remove();
        }
        input.classList.remove('is-invalid');
    }

    // Event listeners for real-time validation
    accountNameInput.addEventListener('blur', validateAccountName);
    accountEmailInput.addEventListener('blur', validateEmailWithDuplicateCheck);
    accountPasswordInput.addEventListener('blur', validatePassword);
    
    // Thêm event listener cho email input để kiểm tra real-time
    accountEmailInput.addEventListener('input', function() {
        const value = this.value.trim();
        
        // Clear timeout cũ nếu có
        if (emailCheckTimeout) {
            clearTimeout(emailCheckTimeout);
        }
        
        // Clear error nếu user đang nhập
        if (value && !lastCheckedEmail.includes(value)) {
            clearError(this);
        }
        
        // Đặt timeout để kiểm tra email sau khi user ngừng nhập 500ms
        if (value && value.includes('@')) {
            emailCheckTimeout = setTimeout(async () => {
                await validateEmailWithDuplicateCheck();
            }, 500);
        }
    });

    // Form submission
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const isNameValid = validateAccountName();
        const isEmailValid = await validateEmailWithDuplicateCheck();
        const isPasswordValid = validatePassword();

        if (isNameValid && isEmailValid && isPasswordValid) {
            // Show loading state
            const submitBtn = form.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerHTML;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang tạo tài khoản...';
            submitBtn.disabled = true;

            // Submit form
            form.submit();
        }
    });

    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert.parentElement) {
                alert.remove();
            }
        }, 5000);
    });
});
