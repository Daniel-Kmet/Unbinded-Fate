'use client';

import React, { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Divider,
} from '@mui/material';
import { Code, Lightbulb } from '@mui/icons-material';

import { SearchForm } from '@/components/SearchForm';
import { PredictionTable } from '@/components/PredictionTable';
import { ErrorMessage } from '@/components/ErrorMessage';
import { usePredictions } from '@/hooks/usePredictions';

export default function HomePage() {
  const { data, loading, error, fetchPredictions, clearError } = usePredictions();
  const [showRawData, setShowRawData] = useState(false);

  const handleSearch = async (specId: string) => {
    await fetchPredictions(specId);
  };

  const handleRetry = () => {
    // Could implement retry logic here if needed
    clearError();
  };

  const ResultsSection = () => {
    if (data.length === 0 && !loading && !error) {
      return (
        <Paper
          elevation={1}
          sx={{
            p: 6,
            textAlign: 'center',
            backgroundColor: 'background.paper',
            borderRadius: 2,
          }}
        >
          <Lightbulb sx={{ fontSize: 48, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary" gutterBottom>
            Ready to Search
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Enter a Spec ID above to fetch prediction records for Pokémon cards.
          </Typography>
        </Paper>
      );
    }

    if (data.length > 0) {
      return (
        <Paper elevation={1} sx={{ p: 3, borderRadius: 2 }}>
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              mb: 3,
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Typography variant="h6" fontWeight="bold">
                Prediction Results
              </Typography>
              <Chip
                label={`${data.length} record${data.length !== 1 ? 's' : ''}`}
                color="primary"
                size="small"
              />
            </Box>
            <Button
              variant="outlined"
              startIcon={<Code />}
              onClick={() => setShowRawData(true)}
              size="small"
            >
              View Raw JSON
            </Button>
          </Box>
          
          <PredictionTable data={data} loading={loading} />
        </Paper>
      );
    }

    return null;
  };

  return (
    <Box sx={{ minHeight: '100vh', backgroundColor: 'background.default', py: 4 }}>
      <Container maxWidth="xl">
        <SearchForm onSubmit={handleSearch} loading={loading} />
        
        <ErrorMessage
          error={error || ''}
          onRetry={handleRetry}
          onDismiss={clearError}
          title="Search Failed"
        />

        <ResultsSection />

        {/* Raw JSON Dialog */}
        <Dialog
          open={showRawData}
          onClose={() => setShowRawData(false)}
          maxWidth="md"
          fullWidth
          PaperProps={{
            sx: { maxHeight: '80vh' },
          }}
        >
          <DialogTitle>
            Raw JSON Data
            <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
              Complete API response for power users
            </Typography>
          </DialogTitle>
          <Divider />
          <DialogContent>
            <Paper
              elevation={0}
              sx={{
                p: 2,
                backgroundColor: 'grey.50',
                borderRadius: 1,
                border: '1px solid',
                borderColor: 'divider',
              }}
            >
              <Typography
                component="pre"
                variant="body2"
                sx={{
                  fontFamily: 'Monaco, Consolas, "Courier New", monospace',
                  fontSize: '0.75rem',
                  lineHeight: 1.4,
                  whiteSpace: 'pre-wrap',
                  wordBreak: 'break-word',
                  maxHeight: '60vh',
                  overflow: 'auto',
                }}
              >
                {JSON.stringify(data, null, 2)}
              </Typography>
            </Paper>
          </DialogContent>
          <DialogActions>
            <Button
              onClick={() => {
                navigator.clipboard.writeText(JSON.stringify(data, null, 2));
              }}
              variant="outlined"
            >
              Copy to Clipboard
            </Button>
            <Button onClick={() => setShowRawData(false)}>Close</Button>
          </DialogActions>
        </Dialog>
      </Container>
    </Box>
  );
} 