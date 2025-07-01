'use client';

import { useState, useCallback } from 'react';
import { PokemonPredictionRecord, ApiError } from '@/types/pokemon';
import { ApiClient } from '@/utils/api';

interface UsePredictionsState {
  data: PokemonPredictionRecord[];
  loading: boolean;
  error: string | null;
}

interface UsePredictionsReturn extends UsePredictionsState {
  fetchPredictions: (specId: string) => Promise<void>;
  clearData: () => void;
  clearError: () => void;
}

export const usePredictions = (): UsePredictionsReturn => {
  const [state, setState] = useState<UsePredictionsState>({
    data: [],
    loading: false,
    error: null,
  });

  const fetchPredictions = useCallback(async (specId: string) => {
    if (!specId.trim()) {
      setState({
        data: [],
        loading: false,
        error: 'Please enter a valid Spec ID',
      });
      return;
    }

    setState(prev => ({
      ...prev,
      loading: true,
      error: null,
    }));

    try {
      const data = await ApiClient.getPredictionsBySpecId(specId.trim());
      setState({
        data,
        loading: false,
        error: null,
      });
    } catch (error) {
      const errorMessage = error instanceof ApiError 
        ? error.message 
        : 'An unexpected error occurred while fetching predictions';
      
      setState({
        data: [],
        loading: false,
        error: errorMessage,
      });
    }
  }, []);

  const clearData = useCallback(() => {
    setState({
      data: [],
      loading: false,
      error: null,
    });
  }, []);

  const clearError = useCallback(() => {
    setState(prev => ({
      ...prev,
      error: null,
    }));
  }, []);

  return {
    ...state,
    fetchPredictions,
    clearData,
    clearError,
  };
}; 