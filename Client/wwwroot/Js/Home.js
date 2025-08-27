let currentCategoryId = null;
let currentPage = 1;
const pageSize = 6;



function filterArticles(categoryId) {
    $('#categoryFilters button').removeClass('active');
    $(`#categoryFilters button[data-filter="${categoryId}"]`).addClass('active');
    
    currentCategoryId = categoryId === 'all' ? null : categoryId;
    currentPage = 1;
    
    loadArticles();
}

function loadStats() {
    console.log('Loading stats...');
    $.ajax({
        url: '/Home/GetStats',
        type: 'GET',
        dataType: 'json',
        success: function(data) {
            console.log('Stats response:', data);
            if (data.error) {
                console.error('Stats error:', data.error);
                $('#totalArticlesCount').text('0');
                $('#totalAuthorsCount').text('0');
                $('#activeArticlesCount').text('0');
                return;
            }
            
            $('#totalArticlesCount').text(data.totalArticles || 0);
            $('#totalAuthorsCount').text(data.totalAuthors || 0);
            $('#activeArticlesCount').text(data.activeArticles || 0);
            console.log('Stats updated successfully');
        },
        error: function(xhr, status, error) {
            console.error('Stats AJAX error:', status, error);
            console.error('Response:', xhr.responseText);
            $('#totalArticlesCount').text('0');
            $('#totalAuthorsCount').text('0');
            $('#activeArticlesCount').text('0');
        }
    });
}

function loadArticles(page = 1) {
    showLoading(true);
    hideNoArticlesMessage();
    hidePagination();
    
    let params = {
        page: page,
        pageSize: pageSize
    };
    
    if (currentCategoryId && currentCategoryId !== 'all') {
        params.categoryId = currentCategoryId;
    }
    
    $.ajax({
        url: '/Home/GetNewsArticles',
        type: 'GET',
        data: params,
        dataType: 'json',
        success: function(data) {
            if (data.error) {
                showError(data.error);
                return;
            }
            
            currentPage = data.currentPage;
            
            renderArticles(data.articles);
            
            if (data.totalPages > 1) {
                renderPagination(data);
                showPagination();
            }
            
            if (data.articles.length === 0) {
                showNoArticlesMessage();
            }
        },
        error: function(xhr, status, error) {
            let errorMessage = 'Có lỗi xảy ra khi tải bài viết. Vui lòng thử lại.';
            
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.error) {
                    errorMessage = response.error;
                }
            } catch (e) {
            }
            
            showError(errorMessage);
        },
        complete: function() {
            showLoading(false);
        }
    });
}

function renderArticles(articles) {
    const $container = $('#articlesContainer');
    
    if (!articles || articles.length === 0) {
        $container.empty();
        return;
    }
    
    let html = '';
    
    $.each(articles, function(index, article) {
        const categoryColor = getCategoryColor(article.categoryName);
        const initials = getInitials(article.accountName);
        const headline = article.headline && article.headline.length > 100 
            ? article.headline.substring(0, 100) + '...' 
            : (article.headline || '');
        
        html += `
            <div class="col" data-category="${article.categoryId}">
                <div class="card h-100 shadow-sm border-0 rounded-3 article-card">
                    <div class="card-header ${categoryColor} text-white border-0 rounded-top-3">
                        <h6 class="mb-0 fw-semibold">
                            <i class="fas fa-tag me-2"></i>${article.categoryName || 'Uncategorized'}
                        </h6>
                    </div>
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title fw-bold text-dark mb-3">${article.newsTitle}</h5>
                        <p class="card-text text-muted flex-grow-1">
                            ${headline}
                        </p>
                        <div class="d-flex justify-content-between align-items-center mt-3">
                            <div class="d-flex align-items-center">
                                <div class="${categoryColor} text-white rounded-circle d-flex align-items-center justify-content-center me-2"
                                     style="width: 32px; height: 32px;">
                                    <small class="fw-bold">${initials}</small>
                                </div>
                                <div>
                                    <div class="fw-semibold small">${article.accountName || 'Anonymous'}</div>
                                    <div class="text-muted small">${article.newsStatus ? 'Active' : 'Inactive'}</div>
                                </div>
                            </div>
                            <a href="#" class="btn ${categoryColor} btn-sm">
                                <i class="fas fa-arrow-right me-1"></i>Read More
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        `;
    });
    
    $container.html(html);
}

function renderPagination(data) {
    const $paginationList = $('#paginationList');
    let html = '';
    
    const prevDisabled = !data.hasPreviousPage ? 'disabled' : '';
    html += `
        <li class="page-item ${prevDisabled}">
            <a class="page-link" href="#" onclick="loadArticlesWithScroll(${data.currentPage - 1})" ${prevDisabled ? 'tabindex="-1" aria-disabled="true"' : ''}>
                <i class="fas fa-chevron-left"></i>
            </a>
        </li>
    `;
    
    const startPage = Math.max(1, data.currentPage - 2);
    const endPage = Math.min(data.totalPages, data.currentPage + 2);
    
    for (let i = startPage; i <= endPage; i++) {
        const active = i === data.currentPage ? 'active' : '';
        html += `
            <li class="page-item ${active}">
                <a class="page-link" href="#" onclick="loadArticlesWithScroll(${i})">${i}</a>
            </li>
        `;
    }
    
    const nextDisabled = !data.hasNextPage ? 'disabled' : '';
    html += `
        <li class="page-item ${nextDisabled}">
            <a class="page-link" href="#" onclick="loadArticlesWithScroll(${data.currentPage + 1})" ${nextDisabled ? 'tabindex="-1" aria-disabled="true"' : ''}>
                <i class="fas fa-chevron-right"></i>
            </a>
        </li>
    `;
    
    $paginationList.html(html);
}

function getCategoryColor(categoryName) {
    if (!categoryName) return 'bg-primary';
    
    switch (categoryName.toLowerCase()) {
        case 'technology': return 'bg-primary';
        case 'health':
        case 'healthcare': return 'bg-success';
        case 'business': return 'bg-secondary';
        case 'education': return 'bg-warning';
        case 'environment': return 'bg-info';
        case 'sports': return 'bg-danger';
        default: return 'bg-primary';
    }
}

function getInitials(name) {
    if (!name) return 'AN';
    
    const words = name.split(' ').filter(word => word.length > 0);
    if (words.length >= 2) {
        return `${words[0][0]}${words[1][0]}`.toUpperCase();
    } else if (words.length === 1) {
        return words[0].substring(0, Math.min(2, words[0].length)).toUpperCase();
    } else {
        return 'AN';
    }
}

function showLoading(show) {
    if (show) {
        $('#loadingSpinner').show();
    } else {
        $('#loadingSpinner').hide();
    }
}

function showNoArticlesMessage() {
    $('#noArticlesMessage').show();
}

function hideNoArticlesMessage() {
    $('#noArticlesMessage').hide();
}

function showPagination() {
    $('#paginationContainer').show();
}

function hidePagination() {
    $('#paginationContainer').hide();
}

function showError(message) {
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show m-3" role="alert">
            <i class="fas fa-exclamation-triangle me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    $('.articles-section .container').prepend(alertHtml);
    
    setTimeout(function() {
        $('.articles-section .container .alert').first().fadeOut(function() {
            $(this).remove();
        });
    }, 5000);
}

$(document).on('click', '.page-link', function(e) {
    e.preventDefault();
});

function scrollToTop() {
    $('html, body').animate({
        scrollTop: $('.articles-section').offset().top - 100
    }, 500);
}

function loadArticlesWithScroll(page) {
    loadArticles(page);
    if (page !== currentPage) {
        scrollToTop();
    }
}