export interface PokemonPredictionRecord {
  cardName: string;
  nameSet: string;
  cardType: string;
  grade: string;
  gradingCompany: string;
  lastListingPrice: number;
  predictedPrice: number;
  errorDollar: number;
  errorPercent: number;
  predictionConfidence: number;
  daysSinceLastSold: number;
  certNumber: string;
  specId: string;
  mongoObjectId: string;
}

export interface ApiResponse {
  success: boolean;
  data: PokemonPredictionRecord[];
  message?: string;
  error?: string;
}

export class ApiError extends Error {
  public status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
  }
} 