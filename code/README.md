# OrderHub - Order Management & Delivery Tracking System

A modern, responsive order management and delivery tracking application built with Next.js, React, and Tailwind CSS. Designed for both admins and users with role-based access control.

## Features

### For Admins
- **Dashboard**: View KPIs, orders per day, and status distribution charts
- **Inventory Management**: Add, edit, delete inventory items with stock level monitoring
- **Orders Management**: View all orders, filter by status, update order statuses
- **Delivery Management**: Manage all deliveries, assign drivers, update location and ETA

### For Users
- **Dashboard**: Track orders and deliveries with visual statistics
- **Orders**: Create new orders, view order history, filter by status
- **Order Details**: View complete order information and associated delivery tracking
- **Delivery Tracking**: Real-time delivery updates, driver information, and tracking timeline

## Tech Stack

- **Framework**: Next.js 16+ (App Router)
- **Frontend**: React 19+ with TypeScript
- **Styling**: Tailwind CSS v4
- **UI Components**: shadcn/ui
- **Data Fetching**: Axios with interceptors
- **Charts**: Recharts
- **State Management**: React Context + localStorage
- **Authentication**: JWT-based with refresh tokens

## Project Structure

\`\`\`
app/
├── admin/
│   ├── dashboard/
│   ├── inventory/
│   ├── orders/
│   └── deliveries/
├── (user)/
│   ├── dashboard/
│   ├── orders/
│   │   ├── page.tsx
│   │   └── [id]/
│   └── deliveries/
│       ├── page.tsx
│       └── [id]/
├── login/
├── register/
└── layout.tsx

lib/
├── api-client.ts        # Axios client with interceptors
├── auth-context.tsx     # Auth context and hooks
└── utils.ts             # Utility functions

components/
├── ui/                  # shadcn/ui components
├── protected-route.tsx  # Route protection HOC
└── navbar.tsx           # Navigation component
\`\`\`

## Getting Started

### Prerequisites
- Node.js 18+ and npm/yarn/pnpm
- Backend API running (see configuration below)

### Installation

1. **Download the project**:
   \`\`\`bash
   # Use the v0 CLI or download the ZIP from v0.app
   npm install  # or yarn/pnpm install
   \`\`\`

2. **Configure environment variables**:
   Create a `.env.local` file in the project root:
   \`\`\`
   NEXT_PUBLIC_API_BASE_URL=https://your-backend-url.com
   NEXT_PUBLIC_DEV_SUPABASE_REDIRECT_URL=http://localhost:3000
   \`\`\`

3. **Run development server**:
   \`\`\`bash
   npm run dev
   \`\`\`
   Open [http://localhost:3000](http://localhost:3000) in your browser.

## API Integration

The application expects the following API endpoints from your backend:

### Authentication
- `POST /api/account/register` - Register new user
- `POST /api/account/login` - Login user (returns JWT token)
- `POST /api/account/refresh` - Refresh JWT token

### Orders
- `GET /api/orders` - Get all orders (admin)
- `GET /api/orders/my-orders` - Get user's orders
- `GET /api/orders/{id}` - Get order details
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}` - Update order status

### Inventory (Admin only)
- `GET /api/inventory` - Get all inventory items
- `POST /api/inventory` - Create inventory item
- `PUT /api/inventory/{id}` - Update inventory item
- `DELETE /api/inventory/{id}` - Delete inventory item

### Deliveries
- `GET /api/deliveries` - Get all deliveries (admin)
- `GET /api/deliveries/my-deliveries` - Get user's deliveries
- `GET /api/deliveries/{id}` - Get delivery details
- `PUT /api/deliveries/{id}` - Update delivery (admin)

### Dashboard
- `GET /api/dashboard/admin-stats` - Get admin dashboard stats
- `GET /api/dashboard/user-stats` - Get user dashboard stats

## Role-Based Access

### Admin Role
- Full access to admin dashboard
- Inventory management (CRUD operations)
- View and manage all orders
- Manage deliveries and assign drivers
- Update delivery status and tracking information

### User Role
- Access to personal dashboard
- Create and view own orders
- Track own deliveries in real-time
- View order details and delivery information

## Development

### Build for Production
\`\`\`bash
npm run build
npm run start
\`\`\`

### Lint Code
\`\`\`bash
npm run lint
\`\`\`

## Deployment

### Deploy to Vercel (Recommended)

1. Push your code to GitHub
2. Go to [vercel.com](https://vercel.com)
3. Import your repository
4. Add environment variables in the Vercel dashboard
5. Deploy

**Important**: Ensure CORS is properly configured on your backend to allow requests from your Vercel domain.

## Configuration

### CORS Configuration

If your backend and frontend are on different domains, ensure your backend CORS configuration allows:
- Origin: Your Vercel deployment URL (e.g., https://orderhub.vercel.app)
- Methods: GET, POST, PUT, DELETE
- Headers: Content-Type, Authorization

### Backend Requirements

Your backend API should:
1. Accept JWT tokens in `Authorization: Bearer {token}` header
2. Return tokens with user role information
3. Support refresh token endpoint for token renewal
4. Implement proper error handling with meaningful error messages
5. Use appropriate HTTP status codes (401 for unauthorized, 403 for forbidden)

## Key Features

### Real-Time Updates
- Delivery tracking refreshes every 15-30 seconds
- Live status updates for orders and deliveries
- Chart data updates on dashboard

### Security
- JWT-based authentication
- Automatic token refresh with interceptors
- Protected routes with role-based access control
- Secure localStorage token management

### User Experience
- Responsive mobile-first design
- Loading states and skeletons
- Error handling with user-friendly messages
- Status badges with color coding
- Filter and search capabilities

### Performance
- Optimized Recharts for data visualization
- Lazy component loading
- Efficient API calls with caching
- Mobile-responsive grid layouts

## Troubleshooting

### "401 Unauthorized" Error
- Check if your token is valid and not expired
- Verify the token is being sent in the Authorization header
- Check your backend authentication implementation

### CORS Errors
- Verify your backend CORS configuration allows your frontend domain
- Check the browser console for specific CORS error messages
- Ensure API_BASE_URL is correctly set in environment variables

### API Connection Issues
- Verify NEXT_PUBLIC_API_BASE_URL is correct
- Check if your backend is running and accessible
- Test the API endpoint directly using curl or Postman

## License

This project is open source and available under the MIT License.

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review your backend API implementation
3. Verify environment variables are correctly set
4. Check browser console for error messages

---
JSON.parse(localStorage.getItem("user"))
