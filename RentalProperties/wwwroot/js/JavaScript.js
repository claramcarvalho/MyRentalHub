const togglePasswordButton = document.getElementById("togglePassword");
const passwordInput = document.getElementById("passwordInput");
const eyeIcon = document.getElementById("eyeIcon");
const toggleConfPasswordButton = document.getElementById("toggleConfirmPassword");
const confPasswordInput = document.getElementById("confirmPasswordInput");
const eyeIconConfPass = document.getElementById("eyeIconConfPass");

togglePasswordButton.addEventListener("click", function () {
    if (passwordInput.type === "password") {
    passwordInput.type = "text";
eyeIcon.classList.remove("fa-eye-slash");
eyeIcon.classList.add("fa-eye");
    } else {
    passwordInput.type = "password";
eyeIcon.classList.remove("fa-eye");
eyeIcon.classList.add("fa-eye-slash");
    }
});

toggleConfPasswordButton.addEventListener("click", function () {
    if (confPasswordInput.type === "password") {
        confPasswordInput.type = "text";
        eyeIconConfPass.classList.remove("fa-eye-slash");
        eyeIconConfPass.classList.add("fa-eye");
    } else {
        confPasswordInput.type = "password";
        eyeIconConfPass.classList.remove("fa-eye");
        eyeIconConfPass.classList.add("fa-eye-slash");
    }
});