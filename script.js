
(function () {
    'use strict';

    function formatCurrency(value) {
        try { return new Intl.NumberFormat(undefined, { style: 'currency', currency: 'USD' }).format(value); } catch { return `${Number(value).toFixed(2)}`; }
    }

    function el(tag, className, text) {
        const e = document.createElement(tag);
        if (className) e.className = className;
        if (text != null) e.textContent = text;
        return e;
    }

    async function loadInvoices() {
        const container = document.getElementById('invoice-container');
        container.innerHTML = '';
        container.appendChild(el('div', 'loading', 'Loading invoices...'));
        try {
            const resp = await fetch('/api/invoice');
            if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
            const data = await resp.json();

            container.innerHTML = '';
            if (!Array.isArray(data) || data.length === 0) {
                container.appendChild(el('div', 'empty', 'No invoices found'));
                return;
            }

            const tmpl = document.getElementById('invoice-template');
            data.forEach(inv => {
                const node = tmpl.content.firstElementChild.cloneNode(true);
                node.querySelector('.invoice-number').textContent = `Invoice #${inv.invoiceId} — ${inv.customerName}`;
                const date = inv.invoiceDate ? new Date(inv.invoiceDate) : null;
                node.querySelector('.invoice-date').textContent = date ? date.toLocaleString() : '';
                node.querySelector('.invoice-total').textContent = formatCurrency(inv.totalAmount || 0);

                const ul = node.querySelector('.invoice-items');
                if (Array.isArray(inv.items)) {
                    inv.items.forEach(it => {
                        const li = document.createElement('li');
                        const left = el('span', null, `${it.name} × ${it.quantity}`);
                        const right = el('span', null, formatCurrency((it.price || 0) * (it.quantity || 1)));
                        li.appendChild(left);
                        li.appendChild(right);
                        ul.appendChild(li);
                    });
                }

                container.appendChild(node);
            });
        } catch (err) {
            container.innerHTML = '';
            container.appendChild(el('div', 'error', `Failed to load invoices: ${err.message}`));
            console.error('Failed to load invoices', err);
        }
    }

    document.addEventListener('DOMContentLoaded', loadInvoices);
})();
