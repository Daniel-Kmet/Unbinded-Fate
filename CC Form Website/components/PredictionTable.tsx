'use client';

import React, { useMemo } from 'react';
import {
  DataGrid,
  GridColDef,
  GridRowsProp,
  GridToolbarContainer,
  GridToolbarExport,
  GridToolbarFilterButton,
  GridToolbarQuickFilter,
} from '@mui/x-data-grid';
import {
  Box,
  Chip,
  LinearProgress,
  Tooltip,
  Typography,
  alpha,
} from '@mui/material';
import { PokemonPredictionRecord } from '@/types/pokemon';
import {
  formatCurrency,
  formatPercentage,
  formatNumber,
  formatConfidence,
  isHighError,
} from '@/utils/formatters';

interface PredictionTableProps {
  data: PokemonPredictionRecord[];
  loading?: boolean;
}

// Custom toolbar component
function CustomToolbar() {
  return (
    <GridToolbarContainer sx={{ justifyContent: 'space-between', p: 1 }}>
      <Box sx={{ display: 'flex', gap: 1 }}>
        <GridToolbarFilterButton />
        <GridToolbarExport />
      </Box>
      <GridToolbarQuickFilter />
    </GridToolbarContainer>
  );
}

// Confidence progress bar component
const ConfidenceBar: React.FC<{ value: number }> = ({ value }) => {
  const confidence = formatConfidence(value);
  const color = confidence >= 80 ? 'success' : confidence >= 60 ? 'warning' : 'error';
  
  return (
    <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
      <LinearProgress
        variant="determinate"
        value={confidence}
        color={color}
        sx={{ width: '60%', mr: 1 }}
      />
      <Typography variant="caption" color="text.secondary">
        {confidence.toFixed(1)}%
      </Typography>
    </Box>
  );
};

export const PredictionTable: React.FC<PredictionTableProps> = ({ data, loading = false }) => {
  const columns: GridColDef[] = useMemo(() => [
    {
      field: 'cardName',
      headerName: 'Card Name',
      width: 150,
      filterable: true,
      sortable: true,
    },
    {
      field: 'nameSet',
      headerName: 'Name Set',
      width: 120,
      filterable: true,
      sortable: true,
    },
    {
      field: 'cardType',
      headerName: 'Card Type',
      width: 100,
      filterable: true,
      sortable: true,
      renderCell: (params) => (
        <Chip
          label={params.value}
          size="small"
          variant="outlined"
          sx={{ borderRadius: 1 }}
        />
      ),
    },
    {
      field: 'grade',
      headerName: 'Grade',
      width: 100,
      filterable: true,
      sortable: true,
      renderCell: (params) => (
        <Chip
          label={params.value}
          size="small"
          color="primary"
          sx={{ fontWeight: 'bold' }}
        />
      ),
    },
    {
      field: 'gradingCompany',
      headerName: 'Grading Company',
      width: 140,
      filterable: true,
      sortable: true,
    },
    {
      field: 'lastListingPrice',
      headerName: 'Last Listing Price',
      width: 150,
      sortable: true,
      type: 'number',
      renderCell: (params) => (
        <Typography variant="body2" fontWeight="medium">
          {formatCurrency(params.value)}
        </Typography>
      ),
    },
    {
      field: 'predictedPrice',
      headerName: 'Predicted Price',
      width: 150,
      sortable: true,
      type: 'number',
      renderCell: (params) => (
        <Typography variant="body2" fontWeight="medium" color="primary">
          {formatCurrency(params.value)}
        </Typography>
      ),
    },
    {
      field: 'errorDollar',
      headerName: 'Error ($)',
      width: 120,
      sortable: true,
      type: 'number',
      renderCell: (params) => {
        const isPositive = params.value >= 0;
        return (
          <Typography
            variant="body2"
            color={isPositive ? 'success.main' : 'error.main'}
            fontWeight="medium"
          >
            {isPositive ? '+' : ''}{formatCurrency(params.value)}
          </Typography>
        );
      },
    },
    {
      field: 'errorPercent',
      headerName: 'Error (%)',
      width: 120,
      sortable: true,
      type: 'number',
      renderCell: (params) => {
        const isHigh = isHighError(params.value);
        const isPositive = params.value >= 0;
        return (
          <Tooltip title={isHigh ? 'High prediction error' : ''}>
            <Typography
              variant="body2"
              color={isPositive ? 'success.main' : 'error.main'}
              fontWeight="medium"
              sx={{
                backgroundColor: isHigh ? alpha('#ff9800', 0.1) : 'transparent',
                px: 0.5,
                borderRadius: 0.5,
              }}
            >
              {isPositive ? '+' : ''}{formatPercentage(params.value)}
            </Typography>
          </Tooltip>
        );
      },
    },
    {
      field: 'predictionConfidence',
      headerName: 'Prediction Confidence',
      width: 180,
      sortable: true,
      type: 'number',
      renderCell: (params) => <ConfidenceBar value={params.value} />,
    },
    {
      field: 'daysSinceLastSold',
      headerName: 'Days Since Last Sold',
      width: 160,
      sortable: true,
      type: 'number',
      renderCell: (params) => (
        <Typography variant="body2">
          {formatNumber(params.value)} days
        </Typography>
      ),
    },
    {
      field: 'certNumber',
      headerName: 'CERT NUMBER',
      width: 140,
      filterable: true,
      sortable: true,
      renderCell: (params) => (
        <Typography variant="body2" fontFamily="monospace">
          {params.value}
        </Typography>
      ),
    },
    {
      field: 'specId',
      headerName: 'SPEC ID',
      width: 140,
      filterable: true,
      sortable: true,
      renderCell: (params) => (
        <Typography variant="body2" fontFamily="monospace" fontWeight="medium">
          {params.value}
        </Typography>
      ),
    },
    {
      field: 'mongoObjectId',
      headerName: 'Mongo Object ID',
      width: 200,
      filterable: true,
      sortable: true,
      renderCell: (params) => (
        <Tooltip title={params.value}>
          <Typography
            variant="body2"
            fontFamily="monospace"
            sx={{
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            }}
          >
            {params.value}
          </Typography>
        </Tooltip>
      ),
    },
  ], []);

  const rows: GridRowsProp = useMemo(() => 
    data.map((record, index) => ({
      id: record.mongoObjectId || index,
      ...record,
    })), [data]
  );

  return (
    <Box sx={{ height: 600, width: '100%' }}>
      <DataGrid
        rows={rows}
        columns={columns}
        loading={loading}
        slots={{
          toolbar: CustomToolbar,
        }}
        initialState={{
          pagination: {
            paginationModel: { page: 0, pageSize: 25 },
          },
          sorting: {
            sortModel: [{ field: 'predictionConfidence', sort: 'desc' }],
          },
        }}
        pageSizeOptions={[10, 25, 50, 100]}
        disableRowSelectionOnClick
        sx={{
          '& .MuiDataGrid-cell': {
            borderColor: 'divider',
          },
          '& .MuiDataGrid-columnHeaders': {
            backgroundColor: 'background.paper',
            borderBottom: 2,
            borderColor: 'divider',
          },
          '& .MuiDataGrid-row:hover': {
            backgroundColor: alpha('#1976d2', 0.04),
          },
        }}
      />
    </Box>
  );
}; 