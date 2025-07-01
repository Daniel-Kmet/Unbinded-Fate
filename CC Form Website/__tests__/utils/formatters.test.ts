import {
  formatCurrency,
  formatPercentage,
  formatNumber,
  formatConfidence,
  isHighError,
} from '@/utils/formatters';

describe('Formatters', () => {
  describe('formatCurrency', () => {
    it('should format currency correctly', () => {
      expect(formatCurrency(1234.56)).toBe('$1,234.56');
      expect(formatCurrency(0)).toBe('$0.00');
      expect(formatCurrency(-123.45)).toBe('-$123.45');
    });
  });

  describe('formatPercentage', () => {
    it('should format percentage correctly', () => {
      expect(formatPercentage(25.5)).toBe('25.50%');
      expect(formatPercentage(0)).toBe('0.00%');
      expect(formatPercentage(-15.25)).toBe('-15.25%');
    });
  });

  describe('formatNumber', () => {
    it('should format numbers with commas', () => {
      expect(formatNumber(1234)).toBe('1,234');
      expect(formatNumber(1234567)).toBe('1,234,567');
      expect(formatNumber(0)).toBe('0');
    });
  });

  describe('formatConfidence', () => {
    it('should ensure confidence is between 0 and 100', () => {
      expect(formatConfidence(50)).toBe(50);
      expect(formatConfidence(150)).toBe(100);
      expect(formatConfidence(-10)).toBe(0);
    });
  });

  describe('isHighError', () => {
    it('should identify high errors correctly', () => {
      expect(isHighError(30)).toBe(true);
      expect(isHighError(-30)).toBe(true);
      expect(isHighError(20)).toBe(false);
      expect(isHighError(25)).toBe(false);
      expect(isHighError(25.1)).toBe(true);
    });
  });
}); 