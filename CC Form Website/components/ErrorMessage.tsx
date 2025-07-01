'use client';

import React from 'react';
import {
  Alert,
  AlertTitle,
  Box,
  Button,
  Collapse,
} from '@mui/material';
import { ErrorOutline, Refresh } from '@mui/icons-material';

interface ErrorMessageProps {
  error: string;
  onRetry?: () => void;
  onDismiss?: () => void;
  title?: string;
  severity?: 'error' | 'warning' | 'info';
}

export const ErrorMessage: React.FC<ErrorMessageProps> = ({
  error,
  onRetry,
  onDismiss,
  title = 'Error',
  severity = 'error',
}) => {
  if (!error) return null;

  return (
    <Collapse in={!!error}>
      <Box sx={{ mb: 3 }}>
        <Alert
          severity={severity}
          icon={<ErrorOutline />}
          action={
            <Box sx={{ display: 'flex', gap: 1 }}>
              {onRetry && (
                <Button
                  color="inherit"
                  size="small"
                  onClick={onRetry}
                  startIcon={<Refresh />}
                >
                  Retry
                </Button>
              )}
              {onDismiss && (
                <Button
                  color="inherit"
                  size="small"
                  onClick={onDismiss}
                >
                  Dismiss
                </Button>
              )}
            </Box>
          }
          sx={{
            '& .MuiAlert-message': {
              width: '100%',
            },
          }}
        >
          <AlertTitle sx={{ fontWeight: 'bold' }}>{title}</AlertTitle>
          {error}
        </Alert>
      </Box>
    </Collapse>
  );
}; 