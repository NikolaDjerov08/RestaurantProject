// Lumière — AJAX menu filtering (progressive enhancement)

(function () {
    'use strict';

    var grid, statusLine, searchInput, sortSelect, pills, hiddenCategory;
    var state = { searchTerm: '', categoryId: null, sorting: 0, currentPage: 1, pageSize: 9 };
    var debounceTimer = null;

    function init() {
        grid = document.querySelector('[data-menu-grid]');
        if (!grid) return; // not on the menu page

        statusLine = document.querySelector('[data-menu-status]');
        searchInput = document.getElementById('SearchTerm');
        sortSelect = document.getElementById('Sorting');
        hiddenCategory = document.getElementById('CategoryId');
        pills = document.querySelectorAll('.category-pill');

        // Seed state from the rendered page so deep links stay consistent
        if (searchInput) state.searchTerm = searchInput.value || '';
        if (sortSelect) state.sorting = parseInt(sortSelect.value, 10) || 0;
        if (hiddenCategory && hiddenCategory.value) state.categoryId = parseInt(hiddenCategory.value, 10);

        bindEvents();
    }

    function bindEvents() {
        if (searchInput) {
            searchInput.addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(function () {
                    state.searchTerm = searchInput.value;
                    state.currentPage = 1;
                    load();
                }, 300);
            });
        }

        if (sortSelect) {
            // Replace the server submit-on-change with AJAX
            sortSelect.removeAttribute('onchange');
            sortSelect.addEventListener('change', function () {
                state.sorting = parseInt(sortSelect.value, 10) || 0;
                state.currentPage = 1;
                load();
            });
        }

        pills.forEach(function (pill) {
            pill.addEventListener('click', function (e) {
                e.preventDefault();
                pills.forEach(function (p) { p.classList.remove('active'); });
                pill.classList.add('active');
                var cat = pill.getAttribute('data-category-id');
                state.categoryId = cat ? parseInt(cat, 10) : null;
                state.currentPage = 1;
                load();
            });
        });

        // Intercept the Apply button (form submit) too
        var form = document.getElementById('filterForm');
        if (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();
                state.searchTerm = searchInput ? searchInput.value : '';
                state.currentPage = 1;
                load();
            });
        }
    }

    function buildQuery() {
        var p = new URLSearchParams();
        if (state.searchTerm) p.set('searchTerm', state.searchTerm);
        if (state.categoryId) p.set('categoryId', state.categoryId);
        p.set('sorting', state.sorting);
        p.set('currentPage', state.currentPage);
        p.set('pageSize', state.pageSize);
        return p.toString();
    }

    async function load() {
        grid.style.opacity = '0.4';
        try {
            var res = await fetch('/api/menu?' + buildQuery(), { credentials: 'same-origin' });
            if (!res.ok) throw new Error('Could not load menu.');
            var data = await res.json();
            render(data);
            syncUrl();
        } catch (err) {
            window.showToast(err.message, 'error');
        } finally {
            grid.style.opacity = '1';
        }
    }

    function esc(s) {
        var d = document.createElement('div');
        d.textContent = s == null ? '' : s;
        return d.innerHTML;
    }

    function money(n) { return '$' + Number(n).toFixed(2); }

    function render(data) {
        if (!data.items || data.items.length === 0) {
            grid.innerHTML = '<div class="empty-state" style="grid-column:1/-1;">' +
                '<div class="big">Nothing on the menu matches</div>' +
                '<p>Try clearing the search or choosing a different course.</p></div>';
            if (statusLine) statusLine.textContent = '';
            renderPager({ totalPages: 0 });
            return;
        }

        var html = data.items.map(function (item, i) {
            var img = item.imageUrl
                ? '<img src="' + esc(item.imageUrl) + '" alt="' + esc(item.name) + '" loading="lazy" />'
                : '<div class="no-img">' + esc(item.name) + '</div>';
            return '' +
              '<article class="food-card" style="animation-delay:' + (i * 50) + 'ms">' +
                '<a href="/Menu/Details/' + item.id + '" class="img-wrap d-block">' + img +
                  '<span class="cat-tag">' + esc(item.categoryName) + '</span></a>' +
                '<div class="body">' +
                  '<h3><a href="/Menu/Details/' + item.id + '" style="color:var(--text);">' + esc(item.name) + '</a></h3>' +
                  '<p class="desc">' + esc(item.description) + '</p>' +
                  '<div class="row">' +
                    '<span class="price">' + money(item.price) + '</span>' +
                    '<form action="/Cart/Add" method="post">' +
                      '<input type="hidden" name="menuItemId" value="' + item.id + '" />' +
                      '<input type="hidden" name="quantity" value="1" />' +
                      '<button type="submit" class="btn btn-outline-gold btn-sm">Add to cart</button>' +
                    '</form>' +
                  '</div>' +
                '</div>' +
              '</article>';
        }).join('');

        grid.innerHTML = html;

        if (statusLine) {
            var from = (data.currentPage - 1) * data.pageSize + 1;
            var to = Math.min(data.currentPage * data.pageSize, data.totalCount);
            statusLine.textContent = 'Showing ' + from + '–' + to + ' of ' + data.totalCount + ' dishes';
        }

        renderPager(data);

        // Re-bind add-to-cart on the freshly rendered forms
        if (window.rebindCartForms) window.rebindCartForms();
    }

    function renderPager(data) {
        var pager = document.querySelector('[data-menu-pager]');
        if (!pager) return;
        if (!data.totalPages || data.totalPages <= 1) { pager.innerHTML = ''; return; }

        var html = '';
        html += data.hasPrevious
            ? '<a href="#" data-page="' + (data.currentPage - 1) + '">‹ Prev</a>'
            : '<span class="disabled">‹ Prev</span>';
        for (var i = 1; i <= data.totalPages; i++) {
            html += i === data.currentPage
                ? '<span class="current">' + i + '</span>'
                : '<a href="#" data-page="' + i + '">' + i + '</a>';
        }
        html += data.hasNext
            ? '<a href="#" data-page="' + (data.currentPage + 1) + '">Next ›</a>'
            : '<span class="disabled">Next ›</span>';
        pager.innerHTML = html;

        pager.querySelectorAll('a[data-page]').forEach(function (a) {
            a.addEventListener('click', function (e) {
                e.preventDefault();
                state.currentPage = parseInt(a.getAttribute('data-page'), 10);
                load();
                window.scrollTo({ top: grid.offsetTop - 120, behavior: 'smooth' });
            });
        });
    }

    function syncUrl() {
        var url = window.location.pathname + '?' + buildQuery();
        window.history.replaceState({}, '', url);
    }

    document.addEventListener('DOMContentLoaded', init);
})();
