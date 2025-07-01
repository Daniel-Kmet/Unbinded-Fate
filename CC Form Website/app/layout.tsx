import { Inter } from 'next/font/google';
import { ThemeProvider } from '@mui/material/styles';
import { CssBaseline, Container } from '@mui/material';
import { theme } from '@/theme/theme';
import './globals.css';

const inter = Inter({ subsets: ['latin'] });

export const metadata = {
  title: 'Pokémon Pricing Oracle - Internal Reporting System',
  description: 'Internal reporting system for the Pokémon Pricing Oracle ML pipeline',
  robots: 'noindex, nofollow', // Internal use only
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <Container maxWidth={false} disableGutters>
            {children}
          </Container>
        </ThemeProvider>
      </body>
    </html>
  );
} 