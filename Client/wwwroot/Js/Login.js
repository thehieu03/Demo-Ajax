$(document).ready(function () {
    let emailValidationTimeOut;
    const emailRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;
    
  initializeAuth();
});

function initializeAuth() {
  $("#authToggle .btn").on("click", function () {
    const formType = $(this).data("form");
    $("#authToggle .btn").removeClass("active");
    $(this).addClass("active");
    $(".auth-form").hide();
    $(`#${formType}Form`).fadeIn(400);
  });
}

