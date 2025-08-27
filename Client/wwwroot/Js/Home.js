$(function () {
  loadArticles();
});
function loadArticles() {
  $.ajax({
    url: "/api/NewsArticle",
    method: "GET",
    dataType: "json",
    success: function (res) {
      renderArticles(res);
    },
    error: function (err) {
      console.error("Lỗi khi gọi API:", err);
    },
  });
}
