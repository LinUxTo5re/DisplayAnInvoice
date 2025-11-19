CREATE TABLE IF NOT EXISTS invoices (
    invoice_id SERIAL PRIMARY KEY,
    customer_name VARCHAR(100) NOT NULL,
    invoice_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    total_amount DECIMAL(10, 2) DEFAULT 0
);

CREATE TABLE IF NOT EXISTS invoice_items (
    item_id SERIAL PRIMARY KEY,
    invoice_id INTEGER NOT NULL REFERENCES invoices(invoice_id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    quantity INTEGER DEFAULT 1
);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_invoice_items_invoice_id ON invoice_items(invoice_id);

-- Insert sample data
INSERT INTO invoices (customer_name, invoice_date, total_amount) 
VALUES ('John Doe', CURRENT_TIMESTAMP, 99.97)
ON CONFLICT DO NOTHING;

INSERT INTO invoice_items (invoice_id, name, price, quantity)
SELECT invoice_id, 'Widget A', 19.99, 2 FROM invoices WHERE customer_name = 'John Doe'
ON CONFLICT DO NOTHING;

INSERT INTO invoice_items (invoice_id, name, price, quantity)
SELECT invoice_id, 'Widget B', 29.99, 1 FROM invoices WHERE customer_name = 'John Doe'
ON CONFLICT DO NOTHING;

INSERT INTO invoice_items (invoice_id, name, price, quantity)
SELECT invoice_id, 'Service Fee', 10.00, 1 FROM invoices WHERE customer_name = 'John Doe'
ON CONFLICT DO NOTHING;
