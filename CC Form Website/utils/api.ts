import { ApiResponse, ApiError, PokemonPredictionRecord } from '@/types/pokemon';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3000/api';

export class ApiClient {
  private static async fetchWithError(url: string, options?: RequestInit): Promise<Response> {
    try {
      const response = await fetch(url, {
        ...options,
        headers: {
          'Content-Type': 'application/json',
          ...options?.headers,
        },
      });

      if (!response.ok) {
        throw new ApiError(`HTTP error! status: ${response.status}`, response.status);
      }

      return response;
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Network error occurred', 0);
    }
  }

  static async getPredictionsBySpecId(specId: string): Promise<PokemonPredictionRecord[]> {
    const url = `${API_BASE_URL}/predictions/${encodeURIComponent(specId)}`;
    
    try {
      const response = await this.fetchWithError(url);
      const data: ApiResponse = await response.json();
      
      if (!data.success) {
        throw new ApiError(data.error || 'Failed to fetch predictions', response.status);
      }
      
      return data.data;
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Failed to fetch predictions', 500);
    }
  }
}

export { ApiError }; 