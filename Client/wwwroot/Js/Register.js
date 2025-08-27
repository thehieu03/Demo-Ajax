document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('registerForm');
    const accountNameInput = document.getElementById('accountName');
    const accountEmailInput = document.getElementById('accountEmail');
    const accountPasswordInput = document.getElementById('accountPassword');
    
    let emailCheckTimeout = null;
    let lastCheckedEmail = '';
    
    const baseUrl = window.apiBaseUrl || 'http://localhost:5173/api';

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

    async function checkEmailExists(email) {
        try {
            const checkingSpan = document.createElement('span');
            checkingSpan.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang kiểm tra...';
            checkingSpan.className = 'text-info small ms-2';
            checkingSpan.id = 'emailCheckingSpinner';
            
            accountEmailInput.parentNode.appendChild(checkingSpan);
            
            const apiUrl = `${baseUrl}/account/getAccountByEmail?email=${encodeURIComponent(email)}`;
            
            const response = await fetch(apiUrl);
            
            const spinner = document.getElementById('emailCheckingSpinner');
            if (spinner) {
                spinner.remove();
            }
            
            if (response.ok) {
                const data = await response.json();
                return data !== null && data !== undefined;
            } else if (response.status === 404) {
                return false;
            } else {
                return false;
            }
        } catch (error) {
            const spinner = document.getElementById('emailCheckingSpinner');
            if (spinner) {
                spinner.remove();
            }
            return false;
        }
    }

    async function validateEmailWithDuplicateCheck() {
        const basicValidation = validateEmail();
        if (!basicValidation) {
            return false;
        }

        const email = accountEmailInput.value.trim();
        
        if (email === lastCheckedEmail) {
            return true;
        }

        if (emailCheckTimeout) {
            clearTimeout(emailCheckTimeout);
        }

        return new Promise((resolve) => {
            emailCheckTimeout = setTimeout(async () => {
                try {
                    const exists = await checkEmailExists(email);
                    lastCheckedEmail = email;
                    
                    if (exists) {
                        showError(accountEmailInput, 'Email này đã được sử dụng');
                        resolve(false);
                    } else {
                        clearError(accountEmailInput);
                        resolve(true);
                    }
                } catch (error) {
                    clearError(accountEmailInput);
                    resolve(true);
                }
            }, 800);
        });
    }

    function validatePassword() {
        const value = accountPasswordInput.value.trim();
        if (value.length < 6) {
            showError(accountPasswordInput, 'Mật khẩu phải có ít nhất 6 ký tự');
            return false;
        }
        if (value.length > 50) {
            showError(accountPasswordInput, 'Mật khẩu không được quá 50 ký tự');
            return false;
        }
        clearError(accountPasswordInput);
        return true;
    }

    function showError(input, message) {
        clearError(input);
        
        const errorSpan = document.createElement('span');
        errorSpan.className = 'text-danger small error-message';
        errorSpan.textContent = message;
        
        input.parentNode.appendChild(errorSpan);
        input.classList.add('is-invalid');
    }

    function clearError(input) {
        const errorMessages = input.parentNode.querySelectorAll('.error-message');
        errorMessages.forEach(msg => msg.remove());
        
        input.classList.remove('is-invalid');
        input.classList.add('is-valid');
    }

    accountNameInput.addEventListener('blur', validateAccountName);
    accountPasswordInput.addEventListener('blur', validatePassword);

    accountEmailInput.addEventListener('blur', async function() {
        await validateEmailWithDuplicateCheck();
    });

    accountEmailInput.addEventListener('input', function() {
        if (emailCheckTimeout) {
            clearTimeout(emailCheckTimeout);
        }
        
        const spinner = document.getElementById('emailCheckingSpinner');
        if (spinner) {
            spinner.remove();
        }
        
        if (accountEmailInput.value.trim() !== lastCheckedEmail) {
            const errorMessages = accountEmailInput.parentNode.querySelectorAll('.error-message');
            errorMessages.forEach(msg => {
                if (msg.textContent.includes('đã được sử dụng')) {
                    msg.remove();
                }
            });
        }
    });

    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const nameValid = validateAccountName();
        const passwordValid = validatePassword();
        const emailValid = await validateEmailWithDuplicateCheck();
        
        if (nameValid && emailValid && passwordValid) {
            form.submit();
        }
    });
});