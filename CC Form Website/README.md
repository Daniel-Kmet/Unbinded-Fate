# Pokémon Pricing Oracle - Internal Reporting System

A production-ready internal reporting system for the Pokémon Pricing Oracle ML pipeline. This application allows staff to query prediction records by Spec ID and view comprehensive reports with filtering, sorting, and export capabilities.

## 🚀 Features

- **Spec ID Search**: Input any Spec ID to fetch all associated prediction records
- **Comprehensive Data Table**: View all prediction fields with filtering, sorting, and search
- **Real-time Predictions**: Display current pricing predictions with confidence metrics
- **Error Highlighting**: Visual indicators for high prediction errors (>25%)
- **Data Export**: Export results to CSV/Excel formats
- **Raw JSON View**: Power user feature to inspect complete API responses
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Production Ready**: Optimized for deployment on Vercel

## 🛠 Technology Stack

- **Framework**: Next.js 14 with TypeScript
- **UI Library**: Material-UI (MUI) with DataGrid
- **Styling**: MUI Theme system with custom styling
- **State Management**: React hooks and context
- **API Integration**: RESTful API with mock data for development
- **Deployment**: Vercel (one-click deploy)

## 📊 Data Fields

Each prediction record includes the following fields:

- **Card Name**: Name of the Pokémon card
- **Name Set**: Trading card set name
- **Card Type**: Pokémon type (Fire, Water, Grass, etc.)
- **Grade**: Card grade (PSA 10, BGS 9.5, etc.)
- **Grading Company**: PSA, BGS, CGC, etc.
- **Last Listing Price**: Most recent market price
- **Predicted Price**: ML model prediction
- **Error ($)**: Dollar difference between prediction and actual
- **Error (%)**: Percentage error with highlighting for >25%
- **Prediction Confidence**: ML model confidence (0-100%)
- **Days Since Last Sold**: Time since last market transaction
- **CERT NUMBER**: Grading certificate number
- **SPEC ID**: Unique card specification identifier
- **Mongo Object ID**: Database record identifier

## 🏗 Project Structure

```
/
├── app/                    # Next.js 13+ app directory
│   ├── globals.css        # Global styles
│   ├── layout.tsx         # Root layout with MUI theme
│   └── page.tsx           # Main application page
├── components/            # React components
│   ├── ErrorMessage.tsx   # Error display component
│   ├── PredictionTable.tsx # Main data table with MUI DataGrid
│   └── SearchForm.tsx     # Spec ID input form
├── hooks/                 # Custom React hooks
│   └── usePredictions.ts  # Data fetching and state management
├── pages/api/             # API routes (mock data)
│   └── predictions/
│       └── [specId].ts    # Mock API endpoint
├── theme/                 # MUI theme configuration
│   └── theme.ts          # Custom theme with colors and typography
├── types/                 # TypeScript type definitions
│   └── pokemon.ts        # Data models and API types
└── utils/                 # Utility functions
    ├── api.ts            # API client with error handling
    └── formatters.ts     # Data formatting utilities
```

## 🚀 Quick Start

### Prerequisites

- Node.js 18+ 
- npm or yarn package manager

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd pokemon-pricing-oracle
   ```

2. **Install dependencies**
   ```bash
   npm install
   # or
   yarn install
   ```

3. **Set up environment variables**
   ```bash
   cp .env.example .env.local
   ```
   
   Configure the following variables in `.env.local`:
   ```env
   NEXT_PUBLIC_API_BASE_URL=https://your-api-endpoint.com
   API_SECRET_KEY=your-secret-key
   ```

4. **Run the development server**
   ```bash
   npm run dev
   # or
   yarn dev
   ```

5. **Open your browser**
   Navigate to [http://localhost:3000](http://localhost:3000) to see the application.

## 🧪 Development

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build production application
- `npm run start` - Start production server
- `npm run lint` - Run ESLint for code quality
- `npm run type-check` - Run TypeScript type checking
- `npm run test` - Run unit tests
- `npm run test:watch` - Run tests in watch mode

### Testing the Application

The application includes mock data for development. Try these sample Spec IDs:

- `BASE-SET-001-EN` - Charizard records (3 different grades)
- `BASE-SET-002-EN` - Blastoise record
- `BASE-SET-003-EN` - Venusaur record

### Code Quality

The project uses:
- **ESLint**: Code linting with TypeScript rules
- **Prettier**: Code formatting
- **TypeScript**: Strict type checking
- **Husky**: Git hooks for pre-commit checks (optional)

## 🌐 API Integration

### Development (Mock API)

The application includes a mock API at `/api/predictions/[specId]` that returns sample data for development.

### Production API

For production, update the `NEXT_PUBLIC_API_BASE_URL` environment variable to point to your FastAPI backend.

Expected API endpoint:
```
GET /predictions/{specId}
```

Expected response format:
```json
{
  "success": true,
  "data": [
    {
      "cardName": "Charizard",
      "nameSet": "Base Set",
      "cardType": "Fire",
      "grade": "PSA 10",
      "gradingCompany": "PSA",
      "lastListingPrice": 12500.00,
      "predictedPrice": 13200.00,
      "errorDollar": 700.00,
      "errorPercent": 5.6,
      "predictionConfidence": 92.5,
      "daysSinceLastSold": 14,
      "certNumber": "PSA123456789",
      "specId": "BASE-SET-001-EN",
      "mongoObjectId": "507f1f77bcf86cd799439011"
    }
  ],
  "message": "Found 1 prediction record(s)"
}
```

## 🚀 Deployment

### Vercel (Recommended)

1. **Connect your repository to Vercel**
   - Visit [vercel.com](https://vercel.com)
   - Import your Git repository
   - Vercel will auto-detect Next.js and configure build settings

2. **Set environment variables**
   In your Vercel dashboard:
   - Go to Project Settings → Environment Variables
   - Add your production API endpoint and secrets

3. **Deploy**
   ```bash
   vercel deploy
   ```

### Manual Deployment

1. **Build the application**
   ```bash
   npm run build
   ```

2. **Start the production server**
   ```bash
   npm start
   ```

## 🔧 Configuration

### Environment Variables

- `NEXT_PUBLIC_API_BASE_URL`: Backend API base URL
- `API_SECRET_KEY`: Secret key for API authentication (server-side only)
- `NODE_ENV`: Environment (development/production)

### MUI Theme Customization

Modify `theme/theme.ts` to customize:
- Color palette
- Typography
- Component styles
- Breakpoints

## 🛡 Security

- Environment variables are properly configured for client/server separation
- API routes include input validation and error handling
- No sensitive data is exposed to the client-side
- Production build includes security headers

## 📱 Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## 🤝 Contributing

This is an internal company project. For questions or improvements:

1. Follow the established code style (ESLint + Prettier)
2. Add TypeScript types for new features
3. Test thoroughly before deployment
4. Update documentation for new features

## 📝 License

Internal use only - Pokémon Pricing Oracle ML Pipeline Team

---

**Note**: This application is designed for internal company use only and should not be made publicly accessible. All deployment should be restricted to internal networks or authenticated users. 