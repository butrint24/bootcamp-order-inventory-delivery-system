-- Delivery
CREATE EXTENSION IF NOT EXISTS pgcrypto;


CREATE TYPE delivery_status AS ENUM (
  'PENDING',
  'PROCESSING',
  'DELIVERED'
);


CREATE TABLE IF NOT EXISTS delivery (
  delivery_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  status delivery_status,
  eta TIMESTAMP,
  order_id UUID,   
  user_id UUID     
);

INSERT INTO delivery (status, eta, order_id, user_id)
VALUES ('PENDING', now() + interval '3 days', gen_random_uuid(), gen_random_uuid());



-- Order

CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE order_status AS ENUM (
  'PENDING',
  'PROCESSING',
  'CANCELLED',
  'COMPLETED',
  'CONFIRMED'
);

CREATE TABLE IF NOT EXISTS "order" (
  order_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  price NUMERIC(12,2),
  status order_status,
  address TEXT,
  user_id UUID  
);
CREATE TABLE IF NOT EXISTS order_item (
  order_item_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  order_id UUID,     
  product_id UUID,   
  quantity INT
);

INSERT INTO "order" (price, status, address, user_id)
VALUES (99.99, 'PENDING', 'Rr. Prishtina', gen_random_uuid());




-- Invetory

CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE product_category AS ENUM (
  'ELECTRONICS',
  'CLOTHING',
  'HEALTH_AND_BEAUTY',
  'ACCESSORIES',
  'FURNITURE',
  'ENTERTAINMENT'
);

CREATE TABLE IF NOT EXISTS product (
  product_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL,
  stock INT DEFAULT 0,
  origin TEXT,
  category product_category,
  price NUMERIC(12,2)
);

INSERT INTO product (name, stock, origin, category, price)
VALUES ('Phone X', 25, 'China', 'ELECTRONICS', 499.99);


-- Auth


CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE role_type AS ENUM ('USER', 'ADMIN');

CREATE TABLE IF NOT EXISTS "user" (
  user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(100),
  surname VARCHAR(100),
  date_of_birth DATE,
  tel VARCHAR(50),
  address TEXT,
  role role_type
);
