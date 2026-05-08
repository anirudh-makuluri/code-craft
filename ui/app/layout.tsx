import type { Metadata } from 'next';
import './globals.css';
import { Toaster } from '@/components/ui/toaster';
import { Providers } from './providers';

export const metadata: Metadata = {
  title: 'CodeCraft',
  description: 'CodeCraft live playground'
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang='en'>
      <body className='font-sans'>
        <Providers>
          {children}
          <Toaster />
        </Providers>
      </body>
    </html>
  );
}
