'use client';

import React, { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Paper,
  Typography,
  InputAdornment,
  CircularProgress,
} from '@mui/material';
import { SearchRounded } from '@mui/icons-material';

interface SearchFormProps {
  onSubmit: (specId: string) => void;
  loading?: boolean;
  disabled?: boolean;
}

export const SearchForm: React.FC<SearchFormProps> = ({
  onSubmit,
  loading = false,
  disabled = false,
}) => {
  const [specId, setSpecId] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!specId.trim()) {
      setError('Please enter a Spec ID');
      return;
    }

    setError('');
    onSubmit(specId.trim());
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setSpecId(value);
    
    // Clear error when user starts typing
    if (error && value.trim()) {
      setError('');
    }
  };

  return (
    <Paper
      elevation={2}
      sx={{
        p: 4,
        mb: 4,
        background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
      }}
    >
      <Box sx={{ textAlign: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1" gutterBottom fontWeight="bold">
          Pokémon Pricing Oracle
        </Typography>
        <Typography variant="h6" color="text.secondary">
          Internal Reporting System
        </Typography>
      </Box>

      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{
          display: 'flex',
          flexDirection: { xs: 'column', sm: 'row' },
          gap: 2,
          alignItems: 'flex-start',
          maxWidth: 600,
          mx: 'auto',
        }}
      >
        <TextField
          fullWidth
          label="Spec ID"
          placeholder="Enter Spec ID (e.g., BASE-SET-001-EN)"
          value={specId}
          onChange={handleInputChange}
          error={!!error}
          helperText={error || 'Enter the Spec ID to fetch all prediction records'}
          disabled={disabled || loading}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchRounded color="action" />
              </InputAdornment>
            ),
          }}
          sx={{
            '& .MuiOutlinedInput-root': {
              backgroundColor: 'background.paper',
            },
          }}
        />
        
        <Button
          type="submit"
          variant="contained"
          size="large"
          disabled={disabled || loading || !specId.trim()}
          sx={{
            minWidth: { xs: '100%', sm: 140 },
            height: 56,
            fontWeight: 'bold',
            fontSize: '1.1rem',
          }}
          startIcon={loading ? <CircularProgress size={20} color="inherit" /> : undefined}
        >
          {loading ? 'Searching...' : 'Search'}
        </Button>
      </Box>

      <Box sx={{ mt: 2, textAlign: 'center' }}>
        <Typography variant="caption" color="text.secondary">
          Try sample Spec IDs: BASE-SET-001-EN, BASE-SET-002-EN, BASE-SET-003-EN
        </Typography>
      </Box>
    </Paper>
  );
}; 