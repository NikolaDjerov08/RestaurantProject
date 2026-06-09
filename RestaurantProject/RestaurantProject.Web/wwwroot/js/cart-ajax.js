// Jerry's — AJAX cart (progressive enhancement over the server-rendered forms)

(function () {
    'use strict';

    function jsonHeaders() {
        return {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': window.getAntiForgeryToken()
        };
    }

    async function api(method, url, body) {
        var opts = { method: method, headers: jsonHeaders(), credentials: 'same-origin' };
        if (body !== undefined) { opts.body = JSON.stringify(body); }
        var res = await fetch(url, opts);
        if (!res.ok) {
            var msg = 'Something went wrong.';
            try { var data = await res.json(); if (data && data.message) msg = data.message; } catch (e) {}
            throw new Error(msg);
        }
        return res.status === 204 ? null : res.json();
    }

    // Exposed so dynamically-rendered cards (menu AJAX) can re-wire their forms
    window.rebindCartForms = function () {
        document.querySelectorAll('form').forEach(function (form) {
            var action = (form.getAttribute('action') || '').toLowerCase();
            if (action.indexOf('/cart/add') !== -1) { bindAddForm(form); }
        });
    };

    function bindAddForm(form) {
        if (form.__cartBound) return;
        form.__cartBound = true;
        form.addEventListener('submit', async function (e) {
            e.preventDefault();
            var idEl = form.querySelector('[name="menuItemId"]');
            var qtyEl = form.querySelector('[name="quantity"]');
            if (!idEl) return;
            var menuItemId = parseInt(idEl.value, 10);
            var quantity = qtyEl ? parseInt(qtyEl.value, 10) || 1 : 1;

            var btn = form.querySelector('button[type="submit"]');
            var original = btn ? btn.textContent : '';
            if (btn) { btn.disabled = true; btn.textContent = 'Adding…'; }

            try {
                var cart = await api('POST', '/api/cart', { menuItemId: menuItemId, quantity: quantity });
                window.updateCartBadge(cart.itemCount);
                window.showToast('Added to your cart.', 'success');
            } catch (err) {
                window.showToast(err.message, 'error');
            } finally {
                if (btn) { btn.disabled = false; btn.textContent = original; }
            }
        });
    }

    // ---- Add-to-cart buttons (menu cards + details page) ----
    function initAddToCart() {
        window.rebindCartForms();
    }

    // ---- Cart page: live quantity / remove / clear ----
    function initCartPage() {
        var container = document.querySelector('[data-cart-page]');
        if (!container) return;

        // JS is active — hide the no-JS manual update buttons
        container.querySelectorAll('[data-nojs-update]').forEach(function (b) { b.style.display = 'none'; });

        container.addEventListener('change', async function (e) {
            var input = e.target.closest('[data-cart-qty]');
            if (!input) return;
            var id = parseInt(input.getAttribute('data-menu-item-id'), 10);
            var qty = parseInt(input.value, 10);
            if (isNaN(qty) || qty < 1) qty = 1;
            try {
                var cart = await api('PUT', '/api/cart', { menuItemId: id, quantity: qty });
                refreshCart(cart);
            } catch (err) { window.showToast(err.message, 'error'); }
        });

        container.addEventListener('click', async function (e) {
            var rm = e.target.closest('[data-cart-remove]');
            if (rm) {
                e.preventDefault();
                var id = parseInt(rm.getAttribute('data-menu-item-id'), 10);
                try {
                    var cart = await api('DELETE', '/api/cart/' + id);
                    refreshCart(cart);
                    window.showToast('Item removed.');
                } catch (err) { window.showToast(err.message, 'error'); }
            }

            var clr = e.target.closest('[data-cart-clear]');
            if (clr) {
                e.preventDefault();
                try {
                    var cart = await api('DELETE', '/api/cart');
                    refreshCart(cart);
                    window.showToast('Cart cleared.');
                } catch (err) { window.showToast(err.message, 'error'); }
            }
        });
    }

    function money(n) { return '$' + Number(n).toFixed(2); }

    function refreshCart(cart) {
        window.updateCartBadge(cart.itemCount);

        // Update each row's subtotal, or reload if structure changed (removal/empty)
        var rowsWrap = document.querySelector('[data-cart-rows]');
        if (!rowsWrap) { window.location.reload(); return; }

        if (cart.itemCount === 0 || cart.items.length === 0) {
            window.location.reload();
            return;
        }

        cart.items.forEach(function (line) {
            var row = rowsWrap.querySelector('[data-row-id="' + line.menuItemId + '"]');
            if (row) {
                var sub = row.querySelector('[data-row-subtotal]');
                if (sub) sub.textContent = money(line.subtotal);
            }
        });

        // Remove rows no longer present
        rowsWrap.querySelectorAll('[data-row-id]').forEach(function (row) {
            var id = parseInt(row.getAttribute('data-row-id'), 10);
            if (!cart.items.some(function (l) { return l.menuItemId === id; })) {
                row.remove();
            }
        });

        // Update summary totals
        var itemCountEl = document.querySelector('[data-summary-count]');
        var subtotalEl = document.querySelector('[data-summary-subtotal]');
        var totalEl = document.querySelector('[data-summary-total]');
        if (itemCountEl) itemCountEl.textContent = cart.itemCount;
        if (subtotalEl) subtotalEl.textContent = money(cart.total);
        if (totalEl) totalEl.textContent = money(cart.total);
    }

    document.addEventListener('DOMContentLoaded', function () {
        initAddToCart();
        initCartPage();
    });
})();
