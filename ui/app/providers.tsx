'use client';

import { User } from '@/types/User';
import { ReactNode, createContext, useContext, useEffect, useState } from 'react';
import { ThemeProvider } from '@/components/theme-provider';
import { customFetch, setClientToken } from '@/lib/utils';

const UserContext = createContext<any>(null);

export function Providers({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    login();
  }, []);

  function login() {
    customFetch({ pathName: 'auth/me' })
      .then((data) => setUser(data))
      .catch(() => setUser(null));
  }

  function applyAuth(token: string, userData: User) {
    setClientToken(token);
    setUser(userData);
  }

  function logout() {
    setClientToken(null);
    setUser(null);
  }

  return (
    <UserContext.Provider value={{ user, login, logout, applyAuth }}>
      <ThemeProvider attribute="class" defaultTheme="dark" enableSystem disableTransitionOnChange>
        {children}
      </ThemeProvider>
    </UserContext.Provider>
  );
}

export function useUser() {
  return useContext(UserContext);
}
