$(document).ready(function () {
  console.log("Document ready, initializing auth...");
  initializeAuth();
});

// Fallback if jQuery is not loaded
if (typeof $ === 'undefined') {
  console.error("jQuery is not loaded!");
  document.addEventListener('DOMContentLoaded', function() {
    console.log("DOM loaded, but jQuery not available");
  });
}

function initializeAuth() {
  console.log("Initializing auth functions...");
  
  $("#authToggle .btn").on("click", function () {
    console.log("Auth toggle clicked");
    const formType = $(this).data("form");
    $("#authToggle .btn").removeClass("active");
    $(this).addClass("active");
    $(".auth-form").hide();
    $(`#${formType}Form`).fadeIn(400);
  });

  $("#loginFormElement").on("submit", function (e) {
    console.log("Login form submitted");
    e.preventDefault();
    handleLogin();
  });

  $("#registerFormElement").on("submit", function (e) {
    console.log("Register form submitted");
    e.preventDefault();
    handleRegister();
  });

  $("#registerPassword").on("input", function () {
    checkPasswordStrength($(this).val());
  });

  $("#registerConfirmPassword").on("input", function () {
    checkPasswordMatch();
  });
  
  console.log("Auth functions initialized");
}

function handleLogin() {
  const email = $("#loginEmail").val();
  const password = $("#loginPassword").val();
  const rememberMe = $("#rememberMe").is(":checked");

  if (!email || !password) {
    showToast("Please fill in all required fields", "error");
    return;
  }

  const submitBtn = $('#loginFormElement button[type="submit"]');
  submitBtn.addClass("btn-loading").prop("disabled", true);

  setTimeout(() => {
    $.ajax({
      url: "/api/auth/login",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        email: email,
        password: password,
        rememberMe: rememberMe,
      }),
      success: function (response) {
        showToast("Login successful! Redirecting...", "success");
        setTimeout(() => {
          window.location.href = "/Home";
        }, 1500);
      },
      error: function (xhr) {
        showToast("Login failed. Please check your credentials.", "error");
      },
      complete: function () {
        submitBtn.removeClass("btn-loading").prop("disabled", false);
      },
    });
  }, 1000);
}

function handleRegister() {
  console.log("handleRegister called");
  const username = $("#registerUsername").val();
  const email = $("#registerEmail").val();
  const password = $("#registerPassword").val();
  const confirmPassword = $("#registerConfirmPassword").val();
  const agreeTerms = $("#agreeTerms").is(":checked");

  console.log("Form data:", { username, email, password: "***", confirmPassword: "***", agreeTerms });

  if (!username || !email || !password || !confirmPassword) {
    console.log("Validation failed: missing fields");
    showToast("Please fill in all required fields", "error");
    return;
  }

  if (password !== confirmPassword) {
    showToast("Passwords do not match", "error");
    return;
  }

  if (!agreeTerms) {
    showToast("Please agree to the terms and conditions", "error");
    return;
  }

  const submitBtn = $('#registerFormElement button[type="submit"]');
  submitBtn.addClass("btn-loading").prop("disabled", true);

  setTimeout(() => {
    $.ajax({
      url: "/api/auth/register",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        username: username,
        email: email,
        password: password,
      }),
      success: function (response) {
        showToast(
          "Registration successful! Please check your email for verification.",
          "success"
        );
        $('#authToggle .btn[data-form="login"]').click();
        $("#registerFormElement")[0].reset();
      },
      error: function (xhr) {
        showToast("Registration failed. Please try again.", "error");
      },
      complete: function () {
        submitBtn.removeClass("btn-loading").prop("disabled", false);
      },
    });
  }, 1000);
}

function togglePassword(inputId) {
  const input = $(`#${inputId}`);
  const icon = $(`#${inputId}Icon`);

  if (input.attr("type") === "password") {
    input.attr("type", "text");
    icon.removeClass("fa-eye").addClass("fa-eye-slash");
  } else {
    input.attr("type", "password");
    icon.removeClass("fa-eye-slash").addClass("fa-eye");
  }
}

function checkPasswordStrength(password) {
  let strength = 0;
  let feedback = "";

  if (password.length >= 8) strength++;
  if (/[a-z]/.test(password)) strength++;
  if (/[A-Z]/.test(password)) strength++;
  if (/[0-9]/.test(password)) strength++;
  if (/[^A-Za-z0-9]/.test(password)) strength++;

  const $container = $(".password-strength");
  const $text = $("#passwordStrengthText");

  $container.removeClass("weak fair good strong");

  switch (strength) {
    case 0:
    case 1:
      $container.addClass("weak");
      feedback = "Very weak";
      break;
    case 2:
      $container.addClass("fair");
      feedback = "Fair";
      break;
    case 3:
      $container.addClass("good");
      feedback = "Good";
      break;
    case 4:
    case 5:
      $container.addClass("strong");
      feedback = "Strong";
      break;
  }

  $text.text(feedback);
}

function checkPasswordMatch() {
  const password = $("#registerPassword").val();
  const confirmPassword = $("#registerConfirmPassword").val();
  const $confirmInput = $("#registerConfirmPassword");

  if (confirmPassword && password !== confirmPassword) {
    $confirmInput.addClass("is-invalid");
    $confirmInput.removeClass("is-valid");
  } else if (confirmPassword) {
    $confirmInput.addClass("is-valid");
    $confirmInput.removeClass("is-invalid");
  } else {
    $confirmInput.removeClass("is-valid is-invalid");
  }
}

function showToast(message, type = "info") {
  const toastId = "toast-" + Date.now();
  const bgClass =
    type === "error"
      ? "bg-danger"
      : type === "success"
      ? "bg-success"
      : "bg-info";

  const toastHtml = `
        <div class="toast ${bgClass} text-white" id="${toastId}" role="alert">
            <div class="toast-body">
                <i class="fas fa-${
                  type === "error"
                    ? "exclamation-triangle"
                    : type === "success"
                    ? "check-circle"
                    : "info-circle"
                } me-2"></i>
                ${message}
            </div>
        </div>
    `;

  $("#toastContainer").append(toastHtml);
  const toast = new bootstrap.Toast(document.getElementById(toastId));
  toast.show();

  $(`#${toastId}`).on("hidden.bs.toast", function () {
    $(this).remove();
  });
}
