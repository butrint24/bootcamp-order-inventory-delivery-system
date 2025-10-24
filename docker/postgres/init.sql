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
CREATE TABLE "user" (
  user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(50) NOT NULL,
  surname VARCHAR(50) NOT NULL,
  date_of_birth DATE,
  tel VARCHAR(50) UNIQUE NOT NULL,
  email VARCHAR(100) UNIQUE NOT NULL,
  address TEXT NOT NULL,
  is_active BOOLEAN DEFAULT TRUE,
  role VARCHAR(20) DEFAULT 'User'
);

CREATE DATABASE order_db;

\c order_db
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE "order" (
  order_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  price NUMERIC(12,2),
  status VARCHAR(20) DEFAULT 'PENDING',
  address TEXT,
  user_id UUID,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP,
  is_deleted BOOLEAN DEFAULT FALSE
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

 
 CREATE TABLE delivery (
   delivery_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
   status VARCHAR(20) DEFAULT 'PENDING',
   eta TIMESTAMP,
   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
   is_active BOOLEAN DEFAULT TRUE,
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
