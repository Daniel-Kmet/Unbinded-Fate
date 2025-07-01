import { NextApiRequest, NextApiResponse } from 'next';
import { ApiResponse, PokemonPredictionRecord } from '@/types/pokemon';

// Mock data for development
const mockData: PokemonPredictionRecord[] = [
  {
    cardName: 'Charizard',
    nameSet: 'Base Set',
    cardType: 'Fire',
    grade: 'PSA 10',
    gradingCompany: 'PSA',
    lastListingPrice: 12500.00,
    predictedPrice: 13200.00,
    errorDollar: 700.00,
    errorPercent: 5.6,
    predictionConfidence: 92.5,
    daysSinceLastSold: 14,
    certNumber: 'PSA123456789',
    specId: 'BASE-SET-001-EN',
    mongoObjectId: '507f1f77bcf86cd799439011'
  },
  {
    cardName: 'Charizard',
    nameSet: 'Base Set',
    cardType: 'Fire',
    grade: 'PSA 9',
    gradingCompany: 'PSA',
    lastListingPrice: 8500.00,
    predictedPrice: 8200.00,
    errorDollar: -300.00,
    errorPercent: -3.5,
    predictionConfidence: 88.7,
    daysSinceLastSold: 7,
    certNumber: 'PSA987654321',
    specId: 'BASE-SET-001-EN',
    mongoObjectId: '507f1f77bcf86cd799439012'
  },
  {
    cardName: 'Charizard',
    nameSet: 'Base Set',
    cardType: 'Fire',
    grade: 'BGS 9.5',
    gradingCompany: 'BGS',
    lastListingPrice: 10200.00,
    predictedPrice: 11800.00,
    errorDollar: 1600.00,
    errorPercent: 15.7,
    predictionConfidence: 85.3,
    daysSinceLastSold: 28,
    certNumber: 'BGS456123789',
    specId: 'BASE-SET-001-EN',
    mongoObjectId: '507f1f77bcf86cd799439013'
  },
  {
    cardName: 'Blastoise',
    nameSet: 'Base Set',
    cardType: 'Water',
    grade: 'PSA 10',
    gradingCompany: 'PSA',
    lastListingPrice: 6800.00,
    predictedPrice: 5200.00,
    errorDollar: -1600.00,
    errorPercent: -23.5,
    predictionConfidence: 76.2,
    daysSinceLastSold: 35,
    certNumber: 'PSA111222333',
    specId: 'BASE-SET-002-EN',
    mongoObjectId: '507f1f77bcf86cd799439014'
  },
  {
    cardName: 'Venusaur',
    nameSet: 'Base Set',
    cardType: 'Grass',
    grade: 'CGC 10',
    gradingCompany: 'CGC',
    lastListingPrice: 4200.00,
    predictedPrice: 5800.00,
    errorDollar: 1600.00,
    errorPercent: 38.1,
    predictionConfidence: 69.4,
    daysSinceLastSold: 42,
    certNumber: 'CGC789456123',
    specId: 'BASE-SET-003-EN',
    mongoObjectId: '507f1f77bcf86cd799439015'
  }
];

export default function handler(req: NextApiRequest, res: NextApiResponse<ApiResponse>) {
  if (req.method !== 'GET') {
    return res.status(405).json({
      success: false,
      data: [],
      error: 'Method not allowed'
    });
  }

  const { specId } = req.query;

  if (!specId || typeof specId !== 'string') {
    return res.status(400).json({
      success: false,
      data: [],
      error: 'Spec ID is required'
    });
  }

  // Simulate API delay
  setTimeout(() => {
    // Filter mock data by spec ID
    const filteredData = mockData.filter(record => 
      record.specId.toLowerCase() === specId.toLowerCase()
    );

    if (filteredData.length === 0) {
      return res.status(404).json({
        success: false,
        data: [],
        error: `No predictions found for Spec ID: ${specId}`
      });
    }

    res.status(200).json({
      success: true,
      data: filteredData,
      message: `Found ${filteredData.length} prediction record(s) for Spec ID: ${specId}`
    });
  }, 1000); // 1 second delay to simulate real API
} 