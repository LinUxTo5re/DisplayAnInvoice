const API_BASE_URL = '/api/invoice';
let currentInvoiceId = null;

// DOM Elements
const invoiceContainer = document.getElementById('invoice-container');
const refreshBtn = document.getElementById('refreshBtn');
const createBtn = document.getElementById('createBtn');
const createModal = document.getElementById('createModal');
const itemModal = document.getElementById('itemModal');
const createForm = document.getElementById('createForm');
const itemForm = document.getElementById('itemForm');
const closeButtons = document.querySelectorAll('.close');

// Event Listeners
document.addEventListener('DOMContentLoaded', function () {
    loadInvoices();
});

refreshBtn.addEventListener('click', loadInvoices);
createBtn.addEventListener('click', () => openModal(createModal));

createForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const customerName = document.getElementById('customerName').value;
    await createInvoice(customerName);
    createForm.reset();
    closeModal(createModal);
});

itemForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const itemName = document.getElementById('itemName').value;
    const itemPrice = parseFloat(document.getElementById('itemPrice').value);
    const itemQuantity = parseInt(document.getElementById('itemQuantity').value);
    await addItemToInvoice(currentInvoiceId, itemName, itemPrice, itemQuantity);
    itemForm.reset();
    closeModal(itemModal);
});

closeButtons.forEach(btn => {
    btn.addEventListener('click', (e) => {
        e.target.closest('.modal').style.display = 'none';
    });
});

window.addEventListener('click', (e) => {
    if (e.target.classList.contains('modal')) {
        e.target.style.display = 'none';
    }
});

// API Functions
async function loadInvoices() {
    try {
        invoiceContainer.innerHTML = '<p class="loading">Loading invoices...</p>';
        const response = await fetch(API_BASE_URL);

        if (!response.ok) {
            if (response.status === 404) {
                invoiceContainer.innerHTML = '<p class="loading">No invoices found. Create one to get started!</p>';
                return;
            }
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        displayInvoices(data);
    } catch (error) {
        console.error('Failed to load invoices:', error);
        invoiceContainer.innerHTML = `<div class="error">Failed to load invoices: ${error.message}</div>`;
    }
}

function displayInvoices(invoices) {
    if (!invoices || invoices.length === 0) {
        invoiceContainer.innerHTML = '<p class="loading">No invoices found. Create one to get started!</p>';
        return;
    }

    invoiceContainer.innerHTML = invoices.map(invoice => `
        <div class="invoice-card">
            <div class="invoice-header">
                <div class="invoice-id">Invoice #${invoice.invoiceId}</div>
                <div class="customer-name">${escapeHtml(invoice.customerName)}</div>
                <div class="invoice-date">${new Date(invoice.invoiceDate).toLocaleDateString()}</div>
            </div>
            
            <div class="invoice-items">
                <div class="items-title">Items:</div>
                ${invoice.items && invoice.items.length > 0 ? invoice.items.map(item => `
                    <div class="item">
                        <div class="item-name">${escapeHtml(item.name)}</div>
                        <div class="item-details">
                            <div class="item-quantity">Qty: ${item.quantity}</div>
                            <div class="item-price">$${(item.price * item.quantity).toFixed(2)}</div>
                        </div>
                    </div>
                `).join('') : '<p style="color: #7f8c8d; font-style: italic;">No items added yet</p>'}
            </div>
            
            <div class="invoice-total">
                <span class="total-label">Total:</span>
                <span class="total-amount">$${invoice.totalAmount.toFixed(2)}</span>
            </div>
            
            <div class="invoice-actions">
                <button class="btn btn-secondary" onclick="openAddItemModal(${invoice.invoiceId})">Add Item</button>
                <button class="btn btn-danger" onclick="deleteInvoice(${invoice.invoiceId})">Delete</button>
            </div>
        </div>
    `).join('');
}

async function createInvoice(customerName) {
    try {
        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ customerName })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        showSuccess('Invoice created successfully!');
        loadInvoices();
    } catch (error) {
        console.error('Failed to create invoice:', error);
        showError(`Failed to create invoice: ${error.message}`);
    }
}

async function addItemToInvoice(invoiceId, name, price, quantity) {
    try {
        const response = await fetch(`${API_BASE_URL}/${invoiceId}/items`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name, price, quantity })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        showSuccess('Item added successfully!');
        loadInvoices();
    } catch (error) {
        console.error('Failed to add item:', error);
        showError(`Failed to add item: ${error.message}`);
    }
}

async function deleteInvoice(invoiceId) {
    if (!confirm('Are you sure you want to delete this invoice?')) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/${invoiceId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        showSuccess('Invoice deleted successfully!');
        loadInvoices();
    } catch (error) {
        console.error('Failed to delete invoice:', error);
        showError(`Failed to delete invoice: ${error.message}`);
    }
}

// UI Helper Functions
function openModal(modal) {
    modal.style.display = 'block';
}

function closeModal(modal) {
    modal.style.display = 'none';
}

function openAddItemModal(invoiceId) {
    currentInvoiceId = invoiceId;
    openModal(itemModal);
}

function showSuccess(message) {
    const div = document.createElement('div');
    div.className = 'success';
    div.textContent = message;
    invoiceContainer.insertBefore(div, invoiceContainer.firstChild);
    setTimeout(() => div.remove(), 3000);
}

function showError(message) {
    const div = document.createElement('div');
    div.className = 'error';
    div.textContent = message;
    invoiceContainer.insertBefore(div, invoiceContainer.firstChild);
    setTimeout(() => div.remove(), 5000);
}

function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}
