$(document).ready(function () {
  let pageSize = 6;
  let currentPage = 1;
  let allArticles = [];
  let categories = [];

  // Lấy dữ liệu từ API
  function loadData() {
    $.when($.getJSON("/api/NewsArticle"), $.getJSON("/api/Category")).done(
      function (articlesRes, categoriesRes) {
        allArticles = articlesRes[0];
        categories = categoriesRes[0];

        updateStats();
        renderCategories();
        renderArticles();
        renderPager();
      }
    );
  }

  // Render thống kê
  function updateStats() {
    $("#totalArticles").text(allArticles.length);
    $("#totalCategories").text(categories.length);
    $("#totalAuthors").text(
      [...new Set(allArticles.map((a) => a.accountName))].length
    );
    $("#activeArticles").text(allArticles.filter((a) => a.newsStatus).length);
  }

  // Render nút category
  function renderCategories() {
    let html = `<button type="button" class="btn btn-outline-primary active" data-filter="all">All</button>`;
    categories.slice(0, 5).forEach((c) => {
      html += `<button type="button" class="btn btn-outline-primary" data-filter="${c.categoryId}">
                        <i class="fas fa-tag me-1"></i>${c.categoryName}
                     </button>`;
    });
    $("#categoryButtons").html(html);

    $("#categoryButtons button").on("click", function () {
      $("#categoryButtons button").removeClass("active");
      $(this).addClass("active");
      currentPage = 1;
      renderArticles();
      renderPager();
    });
  }

  // Render articles
  function renderArticles() {
    let filter = $("#categoryButtons button.active").data("filter");
    let list =
      filter === "all"
        ? allArticles
        : allArticles.filter((a) => a.categoryId == filter);

    let start = (currentPage - 1) * pageSize;
    let paged = list.slice(start, start + pageSize);

    let html = "";
    if (paged.length === 0) {
      html = `<div class="col-12 text-center py-5 text-muted">No articles</div>`;
    } else {
      paged.forEach((a) => {
        html += `
                <div class="col">
                    <div class="card h-100 shadow-sm border-0 rounded-3 article-card">
                        <div class="card-header bg-primary text-white border-0 rounded-top-3">
                            <h6 class="mb-0 fw-semibold">
                                <i class="fas fa-tag me-2"></i>${
                                  a.categoryName ?? "Uncategorized"
                                }
                            </h6>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title fw-bold text-dark mb-3">${
                              a.newsTitle
                            }</h5>
                            <p class="card-text text-muted flex-grow-1">
                                ${
                                  a.headline?.length > 100
                                    ? a.headline.substring(0, 100) + "..."
                                    : a.headline ?? ""
                                }
                            </p>
                            <div class="d-flex justify-content-between align-items-center mt-3">
                                <div class="d-flex align-items-center">
                                    <div class="article-author-circle bg-primary text-white me-2">
                                        <small class="fw-bold">${getInitials(
                                          a.accountName
                                        )}</small>
                                    </div>
                                    <div>
                                        <div class="fw-semibold small">${
                                          a.accountName ?? "Anonymous"
                                        }</div>
                                        <div class="text-muted small">${
                                          a.newsStatus ? "Active" : "Inactive"
                                        }</div>
                                    </div>
                                </div>
                                <a href="#" class="btn bg-primary btn-sm">
                                    <i class="fas fa-arrow-right me-1"></i>Read More
                                </a>
                            </div>
                        </div>
                    </div>
                </div>`;
      });
    }
    $("#articlesContainer").html(html);
  }

  // Render phân trang
  function renderPager() {
    let filter = $("#categoryButtons button.active").data("filter");
    let list =
      filter === "all"
        ? allArticles
        : allArticles.filter((a) => a.categoryId == filter);
    let totalPages = Math.ceil(list.length / pageSize);

    let html = "";
    for (let i = 1; i <= totalPages; i++) {
      html += `<li class="page-item ${i === currentPage ? "active" : ""}">
                        <a class="page-link" href="#">${i}</a>
                     </li>`;
    }
    $("#pager").html(html);

    $("#pager .page-link").on("click", function (e) {
      e.preventDefault();
      currentPage = parseInt($(this).text());
      renderArticles();
      renderPager();
    });
  }

  // Lấy initials từ tên tác giả
  function getInitials(name) {
    if (!name) return "AN";
    let words = name.split(" ");
    return words.length >= 2
      ? (words[0][0] + words[1][0]).toUpperCase()
      : name.substring(0, 2).toUpperCase();
  }

  loadData();
});
