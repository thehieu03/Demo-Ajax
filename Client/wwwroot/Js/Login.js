document.getElementById("email").addEventListener("input", function () {
  const emailError = document.getElementById("emailError");
  const email = this.value;
  const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailPattern.test(email)) {
    emailError.textContent = "Invalid email format";
    emailError.className = "errorMessage";
  } else {
    emailError.textContent = "Email looks good";
    emailError.className = "goodMessage";
  }
});
document.getElementById("password").addEventListener("input", function () {
  const passwordError = document.getElementById("passwordError");
  const password = this.value;
  if (password.length < 2) {
    passwordError.textContent = "Password must be at least 2 characters";
    passwordError.className = "errorMessage";
  } else {
    passwordError.textContent = "Password looks good";
    passwordError.className = "goodMessage";
  }
});
