CREATE DATABASE user_db;
\c user_db
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE role_type AS ENUM (
  'USER',
  'ADMIN'
);

CREATE TABLE "user" (
  user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(50),
  surname VARCHAR(50),
  date_of_birth DATE,
  tel VARCHAR(50),
  address TEXT,
  role role_type
);


CREATE DATABASE order_db;
\c order_db
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE order_status AS ENUM (
  'PENDING',
  'PROCESSING',
  'CANCELLED',
  'COMPLETED',
  'CONFIRMED'
  );

CREATE TABLE "order" (
  order_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  price NUMERIC(12,2),
  status order_status,
  address TEXT,
  user_id UUID
);

CREATE TABLE order_item (
  order_item_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  order_id UUID,
  product_id UUID,
  quantity INT
);


CREATE DATABASE delivery_db;
\c delivery_db
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE delivery_status AS ENUM (
  'PENDING',
  'PROCESSING',
  'DELIVERED'
  );

CREATE TABLE delivery (
  delivery_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  status delivery_status,
  eta TIMESTAMP,
  order_id UUID,
  user_id UUID
);

CREATE DATABASE inventory_db;
\c inventory_db
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE product_category AS ENUM (
  'ELECTRONICS',
  'CLOTHING',
  'HEALTH_AND_BEAUTY',
  'ACCESSORIES',
  'FURNITURE',
  'ENTERTAINMENT'
);

CREATE TABLE product (
  product_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL,
  stock INT DEFAULT 0,
  origin TEXT,
  category product_category,
  price NUMERIC(12,2)
);
